using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public LightMovement LightMovementScript;
    public GameObject LightObject;
    public GameObject Player;
    public Checkpoint currentCheckPoint;
    public List<Checkpoint> checkPoints;

    public static int currentCheckPointNr;

    private bool startFunction = true;


    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        
        LightMovementScript = LightObject.GetComponent<LightMovement>();
    }

    private void Update() {
        if (startFunction) {
            currentCheckPoint = checkPoints[currentCheckPointNr];
            Player.transform.position = currentCheckPoint.SpawnPoint.transform.position;

            startFunction = false;
        }
    }

    public void Respawn() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CheckPointReached(Checkpoint checkpoint) {
        foreach (Checkpoint cp in checkPoints) {
            if (cp == checkpoint)
                cp.Activate();
            else
                cp.Deactivate();
        }
        currentCheckPoint = checkpoint;
        currentCheckPointNr = checkPoints.IndexOf(currentCheckPoint);
    }
}
