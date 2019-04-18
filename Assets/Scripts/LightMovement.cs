using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMovement : MonoBehaviour
{
    public GameObject lightMovementCubesParent;
    public List<LightPathNode> lightPathNodes;
    public int Health = 3;
    public bool StayOnPlayer = false;

    private int currentCubeTarget = 0;
    private bool lastCubeReached = false;
    private float startIntensity;
    private int startHealth;
    public Light sphereLight;
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

        sphereLight = GetComponentInChildren<Light>();
        startIntensity = sphereLight.intensity;
        startHealth = Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (StayOnPlayer) {
            transform.position = GameManager.Instance.Player.transform.position;

            float f = Input.GetAxis("HorizontalTurn");
            if (f > 0.5f) {
                transform.Rotate(0f, -3.0f, 0f);
            }
            else if (f < -0.5f) {
                transform.Rotate(0f, 3.0f, 0f);
            }
        }
        else {

            if (lastCubeReached || wait)
                return;

            Vector3 heading = lightPathNodes[currentCubeTarget].transform.position - transform.position;
            float distance = heading.magnitude;

            transform.position = Vector3.MoveTowards(transform.position, lightPathNodes[currentCubeTarget].transform.position, lightPathNodes[currentCubeTarget].speedTowardsNode * Time.deltaTime);

            if (distance < 0.01f) {
                if (lightPathNodes.Count > currentCubeTarget + 1) {
                    currentCubeTarget++;
                    wait = true;
                    Invoke("NextNode", lightPathNodes[currentCubeTarget].timePauseAtNode);

                }
                else
                    lastCubeReached = true;
            }
        }
    }

    private void NextNode() {
        wait = false;
    }

    public void TakeDamage(int dmg) {
        if (Health - dmg >= 0) {
            Health -= dmg;
            sphereLight.intensity = startIntensity * (float)Health / startHealth;
        }
        else {
            Health = 0;
            sphereLight.intensity = 0f;
        }
    }

    public void AddHealth(int health) {
        if (Health + health <= startHealth)
            Health += health;
        else
            Health = startHealth;

        sphereLight.intensity = startIntensity * (float)Health / startHealth;
    }
}
