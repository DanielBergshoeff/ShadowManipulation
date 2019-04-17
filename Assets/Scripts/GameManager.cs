using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public LightMovement LightMovementScript;
    public GameObject LightObject;
    public GameObject Player;
    public GameObject SpawnPoint;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        Player.transform.position = SpawnPoint.transform.position;

        LightMovementScript = LightObject.GetComponent<LightMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Respawn() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
