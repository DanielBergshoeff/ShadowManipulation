using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour
{
    public GameObject primaryVirtualCam;

    public List<GameObject> virtualCams;

    public static VirtualCameraManager Instance;

    // Start is called before the first frame update
    void Start() {
        Instance = this;

        virtualCams = new List<GameObject>();

        foreach (GameObject vcam in virtualCams) {
            vcam.SetActive(false);
        }
    }

    public void SwitchCamera(GameObject cam) {
        bool preSwitch = cam.activeSelf;
        foreach (GameObject vcam in virtualCams) {
            vcam.SetActive(false);
        }
        cam.SetActive(!preSwitch);
    }

    public void AddCameraToList(GameObject cam) {
        if(cam != primaryVirtualCam) {
            virtualCams.Add(cam);
            cam.SetActive(false);
        }
    }
}
