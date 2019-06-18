using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowLightRotation : MonoBehaviour
{
    public Vector3 Rotation;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.Euler(Rotation * Time.deltaTime);
        transform.Rotate(Rotation * Time.deltaTime);
       // Debug.Log(transform.rotation.y);
        // if (transform.rotation.eulerAngles.y == -360)
        // {
        //     transform.rotation = Quaternion.Euler(9.14f,0,0);
        // }

       // if (transform.rotation.y >= 120)
       // { 
           // Debug.Log("hi");
           // transform.rotation = Quaternion.Euler(9.14f, 40f, 0);
            
        //}
    }
}
