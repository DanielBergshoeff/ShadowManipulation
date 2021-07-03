using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAudio : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip audioClip;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            audioSource.Stop();
            audioSource.volume = 1f;
            audioSource.PlayOneShot(audioClip);
            Invoke("DestroySelf", audioClip.length);
        }
    }

    private void DestroySelf() {
        gameObject.SetActive(false);
    }
}
