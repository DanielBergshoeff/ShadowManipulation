using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraScript : MonoBehaviour
{
    private void Start() {
        VirtualCameraManager.Instance.AddCameraToList(this.gameObject);
    }
}
