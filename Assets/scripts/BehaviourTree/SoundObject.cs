using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class SoundObject : MonoBehaviour
{
    public float Range = 10.0f;
    public float Loudness = 1.0f;
    public AudioClip Sound;

    private float waitForPlay = 0.0f;
    private AudioSource myAudioSource;

    private void Start() {
        myAudioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        if (waitForPlay > 0f)
            waitForPlay -= Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("Player")) {
            if (waitForPlay <= 0f) {
                EnemyManager.soundUnityEvent.Invoke(Range, Loudness, transform.position);
                myAudioSource.PlayOneShot(Sound);
                waitForPlay = Sound.length;
            }
        }
    }
}

public class SoundUnityEvent : UnityEvent<float, float, Vector3> { }
