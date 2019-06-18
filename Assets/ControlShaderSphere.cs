using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlShaderSphere : MonoBehaviour
{
    public Material mat;
    public float currentSpeed;
    public float minSpeed = 0.1f;
    public float maxSpeed = 0.3f;
    public float minSize = 0.5f;
    public float maxSize = 3.0f;

    private bool defendPeter = false;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        currentSpeed = minSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            defendPeter = !defendPeter;
        if (defendPeter && currentSpeed < maxSpeed) {
            if (currentSpeed + Time.deltaTime < maxSpeed) {
                currentSpeed += Time.deltaTime;
            }
            else {
                currentSpeed = maxSpeed;
            }

            float f = (currentSpeed - minSpeed) / (maxSpeed - minSpeed) * (maxSize - minSize) + minSize;
            transform.localScale = new Vector3(f, f, f);
            mat.SetFloat("_Lerp", (currentSpeed - minSpeed) / (maxSpeed - minSpeed) * (1.0f - 0f) + 0f);
            mat.SetFloat("_Speed", currentSpeed);
        }
        else if(!defendPeter && currentSpeed > minSpeed) {
            if (currentSpeed - Time.deltaTime > minSpeed) {
                currentSpeed -= Time.deltaTime;
            }
            else {
                currentSpeed = minSpeed;
            }
            float f = (currentSpeed - minSpeed) / (maxSpeed - minSpeed) * (maxSize - minSize) + minSize;
            transform.localScale = new Vector3(f, f, f);
            mat.SetFloat("_Lerp", (currentSpeed - minSpeed) / (maxSpeed - minSpeed) * (1.0f - 0f) + 0f);
            mat.SetFloat("_Speed", currentSpeed);
        }
    }
}
