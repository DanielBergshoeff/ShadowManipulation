using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlane : MonoBehaviour
{
    LightPathNode pathNode;

    // Start is called before the first frame update
    void Start()
    {
        pathNode = GetComponentInParent<LightPathNode>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        pathNode.waitForPlayer = false;
    }
}
