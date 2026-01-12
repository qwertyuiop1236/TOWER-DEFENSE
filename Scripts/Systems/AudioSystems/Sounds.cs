using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] suonds;
    private AudioSource audio=> GetComponent<AudioSource>();

    public void PlaySound(AudioClip clip, float volume=1,bool isDestroy=false, float p1=0.85f,float p2=1.2f)
    {
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
}
