using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject cameraToSwitch;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            VirtualCameraManager.Instance.SwitchCamera(cameraToSwitch);
        }
    }
}
