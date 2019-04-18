using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    public GameObject lightMovementCubesParent;
    public List<LightPathNode> lightPathNodes;
    public int Health = 3;

    private int currentCubeTarget = 0;
    private bool lastCubeReached = false;
    private float startIntensity;
    private int startHealth;
    private Light light;
    private bool wait = false;

    // Start is called before the first frame update
    void Start()
    {
        // Add all the children of lightMovementCubesParent to lightMovementCubes list
        for (int i = 0; i < lightMovementCubesParent.transform.childCount; i++) {
            lightPathNodes.Add(lightMovementCubesParent.transform.GetChild(i).GetComponent<LightPathNode>());
            MeshRenderer temp = lightPathNodes[i].GetComponent<MeshRenderer>();
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
        if (lastCubeReached || wait)
            return;

        Vector3 heading = lightPathNodes[currentCubeTarget].transform.position - transform.position;
        float distance = heading.magnitude;

        transform.position = Vector3.MoveTowards(transform.position, lightPathNodes[currentCubeTarget].transform.position, lightPathNodes[currentCubeTarget].speedTowardsNode * Time.deltaTime);

        if(distance < 0.01f) {
            if (lightPathNodes.Count > currentCubeTarget + 1) {
                currentCubeTarget++;
                wait = true;
                Invoke("NextNode", lightPathNodes[currentCubeTarget].timePauseAtNode);

            }
            else
                lastCubeReached = true;
        }
    }

    private void NextNode() {
        wait = false;
    }

    public void TakeDamage(int dmg) {
        if (Health - dmg >= 0) {
            Health -= dmg;
            light.intensity = startIntensity * (float)Health / startHealth;
        }
        else {
            Health = 0;
            light.intensity = 0f;
        }
    }

    public void AddHealth(int health) {
        if (Health + health <= startHealth)
            Health += health;
        else
            Health = startHealth;

        light.intensity = startIntensity * (float)Health / startHealth;
    }
}
