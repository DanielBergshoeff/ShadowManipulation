using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip MonsterSound1;
    public AudioClip MonsterSound2;
    public AudioClip HandGrab;
    public AudioClip Heal;
    public AudioClip HandGrowl;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }
}
