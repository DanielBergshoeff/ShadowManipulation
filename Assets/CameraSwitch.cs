using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private bool farCam = false;

    private void OnTriggerEnter(Collider other) {
        if(farCam) {
            VirtualCameraManager.Instance.SwitchFarCamera();
        }
        else {
            VirtualCameraManager.Instance.SwitchCurveCamera();
        }
    }
}
