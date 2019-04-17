using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupLight : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            GameManager.Instance.LightMovementScript.AddHealth(1);
            Destroy(gameObject);
        }
    }
}
