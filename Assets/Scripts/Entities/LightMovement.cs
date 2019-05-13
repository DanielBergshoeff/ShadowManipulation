using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class LightMovement : Attackable
{
    public GameObject lightMovementCubesParent;
    private List<LightPathNode> lightPathNodes;
    public int Health = 3;
    public List<float> intensityLevels;
    public bool StayOnPlayer = false;

    private int currentCubeTarget = 0;
    private bool lastCubeReached = false;
    public HDAdditionalLightData sphereLight;
    private bool wait = false;

    private Vector3 lightPosition;

    public float Height = 1.0f;
    public float minHeight = 1.0f;
    public float maxHeight = 5.0f;
    public float verticalSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        lightPathNodes = new List<LightPathNode>();
        // Add all the children of lightMovementCubesParent to lightMovementCubes list
        for (int i = 0; i < lightMovementCubesParent.transform.childCount; i++) {
            lightPathNodes.Add(lightMovementCubesParent.transform.GetChild(i).GetComponent<LightPathNode>());
            MeshRenderer temp = lightPathNodes[i].GetComponent<MeshRenderer>();
            if(temp != null) {
                temp.enabled = false;
            }
        }

        sphereLight = GetComponentInChildren<HDAdditionalLightData>();
        SetIntensity();

        lightPosition = Vector3.forward * 1.0f + Vector3.up * Height;
    }

    // Update is called once per frame
    void Update()
    {
        if (StayOnPlayer) {
            //transform.position = GameManager.Instance.Player.transform.position + Vector3.up * 1.0f;

            float horizontal = Input.GetAxis("HorizontalTurn");
            float vertical = Input.GetAxis("VerticalTurn");
            float maxLength = 5.0f;

            bool rb = Input.GetButton("RB");
            bool rt = false;
            float rtbtn = Input.GetAxis("RT");
            if(rtbtn > 0.2f) {
                rt = true;
            }

            if (rb && !rt && Height > minHeight) {
                Height -= Time.deltaTime * verticalSpeed;
            }
            else if(rt && !rb && Height < maxHeight){
                Height += Time.deltaTime * verticalSpeed;
            }

            Vector3 heading = Camera.main.transform.position - transform.position;
            heading = new Vector3(heading.x, 0, heading.z);
            float magnitude = heading.magnitude;

            Vector3 direction = heading / magnitude;
            Vector3 right = Quaternion.Euler(0, 90f, 0) * direction;

            horizontal *= maxLength;
            vertical *= maxLength;

            Vector3 lightPos = direction * vertical - right * horizontal;
            if (lightPos.magnitude != 0f) {
                if (lightPos.magnitude < 1.0f) {
                    lightPos.Normalize();
                }
                lightPosition = lightPos + Vector3.up * Height;
            }

            transform.position = GameManager.Instance.Player.transform.position + lightPosition;
        }
        else {

            if (lastCubeReached || wait)
                return;

            if(currentCubeTarget > 0) {
                if (lightPathNodes[currentCubeTarget - 1].waitForPlayer)
                    return;
            }

            Vector3 heading = lightPathNodes[currentCubeTarget].transform.position - transform.position;
            float distance = heading.magnitude;

            transform.position = Vector3.MoveTowards(transform.position, lightPathNodes[currentCubeTarget].transform.position, lightPathNodes[currentCubeTarget].speedTowardsNode * Time.deltaTime);

            if (distance < 0.01f) {
                if (lightPathNodes.Count > currentCubeTarget + 1) {
                    Invoke("NextNode", lightPathNodes[currentCubeTarget].timePauseAtNode);
                    currentCubeTarget++;
                    wait = true;
                }
                else
                    lastCubeReached = true;
            }
        }
    }

    private void NextNode() {
        wait = false;
    }

    public override void TakeDamage(int dmg) {
        if (Health - dmg >= 0) {
            Health -= dmg;
        }
        else {
            Health = 0;
        }
        SetIntensity();
    }

    public void AddHealth(int health) {
        if (Health + health < intensityLevels.Count)
            Health += health;
        else
            Health = intensityLevels.Count - 1;

        SetIntensity();
    }

    public void SetIntensity() {
        if (intensityLevels != null && sphereLight != null) {
            if (intensityLevels.Count > Health && Health >= 0) {
                sphereLight.intensity = intensityLevels[Health];
            }
        }
    }
}
