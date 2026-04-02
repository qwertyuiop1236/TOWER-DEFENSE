using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        // Устанавливаем значения из AudioManager
        if (AudioManager.Instance != null)
        {
            musicSlider.value = AudioManager.Instance.MusicVolume;
            sfxSlider.value = AudioManager.Instance.SFXVolume;
        }

        // Подписываемся на события
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        if (closeButton != null)
            closeButton.onClick.AddListener(ClosePanel);
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.MusicVolume = value;
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SFXVolume = value;
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    // Можно добавить метод для открытия панели из другой кнопки
    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }
}