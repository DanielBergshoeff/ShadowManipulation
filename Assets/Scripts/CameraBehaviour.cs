using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public GameObject CameraBehaviourNodesParent;
    public List<CameraBehaviourNode> CameraBehaviourNodes;
    private CameraBehaviourNode closestNode;
    private CameraBehaviourNode secondClosestNode;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < CameraBehaviourNodesParent.transform.childCount; i++) {
            CameraBehaviourNode cameraBehaviourNode = CameraBehaviourNodesParent.transform.GetChild(i).GetComponent<CameraBehaviourNode>();
            if(cameraBehaviourNode != null) {
                CameraBehaviourNodes.Add(cameraBehaviourNode);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindClosestNodes();
        AdjustCameraPosition();
    }

    /// <summary>
    /// Adjusts the position of the camera based on the information of the two nodes closest to the player
    /// </summary>
    private void AdjustCameraPosition() {
        
    }


    /// <summary>
    /// Finds the two nodes closest to the player
    /// </summary>
    private void FindClosestNodes() {
        closestNode = CameraBehaviourNodes[0];
        float closestNodeDistance = float.PositiveInfinity;
        secondClosestNode = CameraBehaviourNodes[0];
        float secondClosestNodeDistance = float.PositiveInfinity;

        foreach (CameraBehaviourNode node in CameraBehaviourNodes) {//Check each node in the list of nodes
            Vector3 heading = node.transform.position - transform.position;
            float distance = heading.sqrMagnitude;

            if (distance < secondClosestNodeDistance) { //If this node is closer than the second closest node
                if (distance < closestNodeDistance) { //If it is also closer than the closest node

                    //Save current closest node information
                    CameraBehaviourNode tempNode = closestNode;
                    float tempDistance = closestNodeDistance;

                    //Set node as the closest node
                    closestNodeDistance = distance;
                    closestNode = node;

                    //Set previous closest node as second closest node
                    secondClosestNode = tempNode;
                    secondClosestNodeDistance = tempDistance;
                }
                else { //If it is not closer than the closest node
                    secondClosestNodeDistance = distance;
                    secondClosestNode = node;
                }
            }
        }
    }
}
