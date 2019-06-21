using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorProbe : MonoBehaviour
{
    public enum Directions { X, Y, Z};

    public Directions Orientation;

    public GameObject Mirror;
    public GameObject Player;

    private float offset;
    private Vector3 probePosition;
    private ReflectionProbe myReflectionProbe;

    private void Start() {
        myReflectionProbe = GetComponent<ReflectionProbe>();
        myReflectionProbe.resolution = 1024;
        myReflectionProbe.hdr = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Orientation == Directions.X) {
            offset = Mirror.transform.position.x - Player.transform.position.x;

            probePosition.x = Mirror.transform.position.x - offset;
            probePosition.y = Player.transform.position.y;
            probePosition.z = Mirror.transform.position.z;
        }
        else if(Orientation == Directions.Y) {
            offset = Mirror.transform.position.y - Player.transform.position.y;

            probePosition.x = Player.transform.position.x;
            probePosition.y = Mirror.transform.position.y + offset;
            probePosition.z = Player.transform.position.z;
        }
        else if (Orientation == Directions.Z) {
            offset = Mirror.transform.position.z - Player.transform.position.z;

            probePosition.x = Player.transform.position.x;
            probePosition.y = Player.transform.position.y;
            probePosition.z = Mirror.transform.position.z + offset;
        }

        transform.position = probePosition;
    }
}
