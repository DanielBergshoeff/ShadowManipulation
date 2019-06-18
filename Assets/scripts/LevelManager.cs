using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string sceneToSwitchTo;
    public bool allowSwitch;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (allowSwitch) {
                SceneManager.LoadScene(sceneToSwitchTo, LoadSceneMode.Single);
            }
        }
    }
}
