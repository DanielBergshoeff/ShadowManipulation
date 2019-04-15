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
        SwitchCamera(farVirtualCam);
    }

    public void SwitchCurveCamera() {
        SwitchCamera(curveCamera0);
    }

    public void SwitchCamera(GameObject cam) {
        bool preSwitch = cam.activeSelf;
        foreach (GameObject vcam in virtualCams) {
            vcam.SetActive(false);
        }
        cam.SetActive(!preSwitch);
    }
}
