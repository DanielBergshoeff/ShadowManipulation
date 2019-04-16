using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    public GameObject lightMovementCubesParent;
    public float Speed = 1.0f;
    public List<Transform> lightMovementCubes;
    public int Health = 3;

    private int currentCubeTarget = 0;
    private bool lastCubeReached = false;
    private float startIntensity;
    private int startHealth;
    private Light light;

    // Start is called before the first frame update
    void Start()
    {
        // Add all the children of lightMovementCubesParent to lightMovementCubes list
        for (int i = 0; i < lightMovementCubesParent.transform.childCount; i++) {
            lightMovementCubes.Add(lightMovementCubesParent.transform.GetChild(i));
            MeshRenderer temp = lightMovementCubes[i].GetComponent<MeshRenderer>();
            if(temp != null) {
                temp.enabled = false;
            }
        }

        light = GetComponentInChildren<Light>();
        startIntensity = light.intensity;
        startHealth = Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastCubeReached)
            return;

        Vector3 heading = lightMovementCubes[currentCubeTarget].transform.position - transform.position;
        float distance = heading.magnitude;

        transform.position = Vector3.MoveTowards(transform.position, lightMovementCubes[currentCubeTarget].transform.position, Speed * Time.deltaTime);

        if(distance < 0.01f) {
            if (lightMovementCubes.Count > currentCubeTarget + 1)
                currentCubeTarget++;
            else
                lastCubeReached = true;
        }
    }

    public void TakeDamage() {
        Health--;
        light.intensity = startIntensity * (float)Health / startHealth;
    }
}
