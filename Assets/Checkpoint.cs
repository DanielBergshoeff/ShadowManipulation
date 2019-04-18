using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform SpawnPoint;
    public Material myMaterial;

    private Color c;

    private bool enabled = false;

    private void Start() {
        GameManager.Instance.checkPoints.Add(this);
        c = GameManager.Instance.emissionColorCheckpoint;
        myMaterial = GetComponent<Renderer>().material;
        myMaterial.SetColor("_EmissionColor", new Vector4(c.r, c.g, c.b, 0) * 1.0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (!enabled) {
            GameManager.Instance.CheckPointReached(this);
        }
    }

    public void Activate() {
        myMaterial.SetColor("_EmissionColor", new Vector4(c.r, c.g, c.b, 0) * 2.0f);
        enabled = true;
    }

    public void Deactivate() {
        myMaterial.SetColor("_EmissionColor", new Vector4(c.r, c.g, c.b, 0) * 1.0f);
        enabled = false;
    }
}
