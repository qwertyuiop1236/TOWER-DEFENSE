using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum AudioGroup
{
    Music,
    SFX,
    UISound   // для звуков интерфейса (клики кнопок и т.п.)
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEntry
    {
        public string key;
        public AudioClip clip;
        public AudioClip[] clips;
        public float baseVolume = 1f;
        public AudioGroup defaultGroup = AudioGroup.SFX;
    }

    [SerializeField] private SoundEntry[] sounds;
    private Dictionary<string, SoundEntry> soundDict = new Dictionary<string, SoundEntry>();

    // Отдельные источники
    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource uiSource; // для UI звуков

    // Текущие громкости (загружаются из файла)
    private float musicVolume = 0.5f;
    private float sfxVolume = 0.5f;
    private float uiVolume = 0.7f;

    // Путь к файлу сохранения
    private string settingsPath;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        settingsPath = Path.Combine(Application.persistentDataPath, "audio_settings.json");

        // Создаём источники
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.playOnAwake = false;

        // Заполняем словарь
        foreach (var entry in sounds)
        {
            if (!soundDict.ContainsKey(entry.key))
                soundDict.Add(entry.key, entry);
            else
                Debug.LogWarning($"Duplicate sound key: {entry.key}");
        }

        // Загружаем настройки
        LoadSettings();
        ApplyVolumes();
    }

    // Применяем громкости к источникам
    private void ApplyVolumes()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        uiSource.volume = uiVolume;
    }

    // Основной метод воспроизведения
    public void PlaySound(string key, float volume = 1f, bool randomPitch = false, 
                          Vector3? position = null, AudioGroup group = AudioGroup.SFX)
    {
        if (!soundDict.TryGetValue(key, out SoundEntry entry))
        {
            Debug.LogWarning($"Sound '{key}' not found!");
            return;
        }

        AudioClip clip = GetClip(entry);
        if (clip == null) return;

        float finalVolume = volume * entry.baseVolume;
        float pitch = randomPitch ? Random.Range(0.85f, 1.2f) : 1f;

        // Для 3D позиционированных звуков используем PlayClipAtPoint (они не регулируются отдельно? 
        // Лучше регулировать их громкостью SFX)
        if (position.HasValue)
        {
            // Для 3D звуков используем глобальную громкость SFX
            float globalSFX = sfxVolume;
            AudioSource.PlayClipAtPoint(clip, position.Value, finalVolume * globalSFX);
            return;
        }

        // Выбираем источник в зависимости от группы
        AudioSource source;
        float groupVolume;
        switch (group)
        {
            case AudioGroup.Music:
                source = musicSource;
                groupVolume = musicVolume;
                break;
            case AudioGroup.UISound:
                source = uiSource;
                groupVolume = uiVolume;
                break;
            default:
                source = sfxSource;
                groupVolume = sfxVolume;
                break;
        }

        source.pitch = pitch;
        source.PlayOneShot(clip, finalVolume * groupVolume);
    }

    // Специальный метод для фоновой музыки (останавливает предыдущую)
    public void PlayMusic(string key, bool loop = true)
    {
        if (!soundDict.TryGetValue(key, out SoundEntry entry))
        {
            Debug.LogWarning($"Music '{key}' not found!");
            return;
        }

        AudioClip clip = GetClip(entry);
        if (clip == null) return;

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // Геттеры/сеттеры громкости
    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp01(value);
            musicSource.volume = musicVolume;
            SaveSettings();
        }
    }

    public float SFXVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            sfxSource.volume = sfxVolume;
            SaveSettings();
        }
    }

    public float UIVolume
    {
        get => uiVolume;
        set
        {
            uiVolume = Mathf.Clamp01(value);
            uiSource.volume = uiVolume;
            SaveSettings();
        }
    }

    // Сохранение в JSON
    private void SaveSettings()
    {
        SettingsData data = new SettingsData
        {
            musicVolume = musicVolume,
            sfxVolume = sfxVolume,
            uiVolume = uiVolume
        };
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(settingsPath, json);
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            SettingsData data = JsonUtility.FromJson<SettingsData>(json);
            if (data != null)
            {
                musicVolume = data.musicVolume;
                sfxVolume = data.sfxVolume;
                uiVolume = data.uiVolume;
            }
        }
        // Иначе остаются значения по умолчанию (0.5, 0.5, 0.7)
    }

    private AudioClip GetClip(SoundEntry entry)
    {
        if (entry.clips != null && entry.clips.Length > 0)
            return entry.clips[Random.Range(0, entry.clips.Length)];
        return entry.clip;
    }
}

// Класс для сериализации настроек
[System.Serializable]
public class SettingsData
{
    public float musicVolume;
    public float sfxVolume;
    public float uiVolume;
}