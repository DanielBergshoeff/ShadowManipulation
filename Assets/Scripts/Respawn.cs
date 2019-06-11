using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        GameManager.Instance.Respawn(0f);
    }
}
