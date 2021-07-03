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
    public bool startAtFirstCheckPoint = true;

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
            startFunction = false;
            if (checkPoints.Count > 0) {
                currentCheckPoint = checkPoints[currentCheckPointNr];
                if(startAtFirstCheckPoint)
                    Player.transform.position = currentCheckPoint.SpawnPoint.transform.position;
                Checkpoint.CheckpointsActive = true;
            }
        }
    }

    public void Respawn(float waitTime) {
        Invoke("ReloadScene", waitTime);
    }

    public void ReloadScene() {
        Checkpoint.CheckpointsActive = false;
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
