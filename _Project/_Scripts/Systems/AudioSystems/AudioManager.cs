using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEntry
    {
        public string key;           // строковый идентификатор
        public AudioClip clip;       // одиночный клип
        public AudioClip[] clips;    // массив для случайного выбора (если нужен рандом)
        public float baseVolume = 1f;
    }

    [SerializeField] private SoundEntry[] sounds; // настраивается в инспекторе
    private Dictionary<string, SoundEntry> soundDict = new Dictionary<string, SoundEntry>();

    private AudioSource sharedSource; // один источник для всех звуков (можно и без него, если использовать PlayOneShot)

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Заполняем словарь
        foreach (var entry in sounds)
        {
            if (!soundDict.ContainsKey(entry.key))
                soundDict.Add(entry.key, entry);
            else
                Debug.LogWarning($"Дубликат ключа звука: {entry.key}");
        }

        // Создаём общий AudioSource, если хотим управлять громкостью глобально
        sharedSource = gameObject.AddComponent<AudioSource>();
        sharedSource.playOnAwake = false;
    }

    /// <summary>
    /// Воспроизвести звук по ключу
    /// </summary>
    /// <param name="key">Идентификатор звука</param>
    /// <param name="volume">Множитель громкости (0-1)</param>
    /// <param name="randomPitch">Случайный шаг (от 0.85 до 1.2)</param>
    /// <param name="position">Позиция в мире (если нужно 3D звучание)</param>
    public void PlaySound(string key, float volume = 1f, bool randomPitch = false, Vector3? position = null)
    {
        if (!soundDict.TryGetValue(key, out SoundEntry entry))
        {
            Debug.LogWarning($"Звук с ключом {key} не найден!");
            return;
        }

        AudioClip clip = GetClip(entry);
        if (clip == null) return;

        float finalVolume = volume * entry.baseVolume;
        float pitch = randomPitch ? Random.Range(0.85f, 1.2f) : 1f;

        if (position.HasValue)
        {
            // Проигрываем в точке (3D звук)
            AudioSource.PlayClipAtPoint(clip, position.Value, finalVolume);
        }
        else
        {
            // Проигрываем через общий источник (2D звук)
            sharedSource.pitch = pitch;
            sharedSource.PlayOneShot(clip, finalVolume);
        }
    }

    private AudioClip GetClip(SoundEntry entry)
    {
        if (entry.clips != null && entry.clips.Length > 0)
            return entry.clips[Random.Range(0, entry.clips.Length)];
        return entry.clip;
    }
}