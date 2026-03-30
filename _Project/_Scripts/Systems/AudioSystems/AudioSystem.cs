using UnityEngine;

/// <summary>
/// Устаревший компонент. Вместо него используйте AudioManager.
/// </summary>
public class AudioSystem : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private SoundArrays[] randSound;
    private AudioSource _audioSource => GetComponent<AudioSource>();
    
    public void PlaySound(int index, float volume = 1, bool random = false, bool isDestroy = false, float p1 = 0.85f, float p2 = 1.2f)
    {
        if (random ? randSound[index].soundArray[Random.Range(0, randSound[index].soundArray.Length)] : sounds[index] != null)
        {
            AudioClip clip = random ? randSound[index].soundArray[Random.Range(0, randSound[index].soundArray.Length)] : sounds[index];
            _audioSource.pitch = Random.Range(p1, p2);

            if (isDestroy)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position, volume);
            }
            else
            {
                _audioSource.PlayOneShot(clip, volume);
            }
        }
        else
        {
            Debug.Log("Нет звука под этим индексом");
        }
    }

    [System.Serializable]
    public class SoundArrays
    {
        public AudioClip[] soundArray;
    }
}