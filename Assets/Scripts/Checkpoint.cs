using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform SpawnPoint;
    public Material myMaterial;

    public GameObject showActivated;

    private new bool enabled = false;

    private void Start() {
        showActivated.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!enabled) {
                GameManager.Instance.CheckPointReached(this);
            }
        }
    }

    public void Activate() {
        enabled = true;
        showActivated.SetActive(true);
    }

    public void Deactivate() {
        enabled = false;
        showActivated.SetActive(false);
    }
}
