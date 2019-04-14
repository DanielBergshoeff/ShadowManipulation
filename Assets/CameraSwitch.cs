using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        VirtualCameraManager.Instance.SwitchFarCamera();
    }
}
