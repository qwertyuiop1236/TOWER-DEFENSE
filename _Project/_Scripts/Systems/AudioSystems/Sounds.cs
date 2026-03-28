using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;
    [SerializeField] private SoundArrays[] randSound;
    private AudioSource audio=> GetComponent<AudioSource>();

    public void PlaySound(int index, float volume=1, bool random =false, bool isDestroy=false, float p1=0.85f,float p2=1.2f)
    {
        if(random ? randSound[index].suondArray[Random.Range(0,randSound[index].suondArray.Length)] : sounds[index] != null){

            AudioClip clip = random ? randSound[index].suondArray[Random.Range(0,randSound[index].suondArray.Length)] : sounds[index];
            audio.pitch=Random.Range(p1,p2);

            if (isDestroy)
            {
                AudioSource.PlayClipAtPoint(clip, transform.position,volume);
            }
            else
            {
                audio.PlayOneShot(clip,volume);
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
        public AudioClip[] suondArray;
    }
}