﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform SpawnPoint;
    public Material myMaterial;

    private Color c;

    private new bool enabled = false;

    private void Start() {
        c = GameManager.Instance.emissionColorCheckpoint;
        myMaterial = GetComponent<Renderer>().material;
        myMaterial.SetColor("_EmissionColor", new Vector4(c.r, c.g, c.b, 0) * 1.0f);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!enabled) {
                GameManager.Instance.CheckPointReached(this);
            }
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
