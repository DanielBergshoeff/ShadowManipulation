using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualCameraManager : MonoBehaviour
{
    public GameObject primaryVirtualCam;
    public GameObject farVirtualCam;
    public GameObject closeVirtualCam;

    public static VirtualCameraManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;

        farVirtualCam.SetActive(false);
        //closeVirtualCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            farVirtualCam.SetActive(true);
        }
    }

    public void SwitchFarCamera() {
        farVirtualCam.SetActive(!farVirtualCam.activeSelf);
    }
}
