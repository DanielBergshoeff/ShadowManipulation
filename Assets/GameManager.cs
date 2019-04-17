using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject Player;
    public GameObject SpawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Player.transform.position = SpawnPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
