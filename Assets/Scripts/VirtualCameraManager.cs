using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour
{
    public GameObject primaryVirtualCam;
    public GameObject farVirtualCam;
    public GameObject curveCamera0;

    public List<GameObject> virtualCams;

    public static VirtualCameraManager Instance;

    // Start is called before the first frame update
    void Start() {
        Instance = this;

        virtualCams.AddRange(new GameObject[] { farVirtualCam, curveCamera0 });

        foreach (GameObject vcam in virtualCams) {
            vcam.SetActive(false);
        }
    }

        public void SwitchFarCamera() {
        foreach(GameObject vcam in virtualCams) {
            vcam.SetActive(false);
        }

        farVirtualCam.SetActive(!farVirtualCam.activeSelf);
    }

    public void SwitchCurveCamera() {
        foreach (GameObject vcam in virtualCams) {
            vcam.SetActive(false);
        }

        curveCamera0.SetActive(true);
    }
}
