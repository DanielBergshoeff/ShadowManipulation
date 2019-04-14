using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turter : MonoBehaviour
{
    public float Size = 3.0f;
    public float Speed = 1.0f;
    public float ViewRange = 50.0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move() {
        if (LightManager.Instance.ShadowFound) {
            float dist = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);
            if (dist <= ViewRange) {
                Vector3 target = new Vector3(LightManager.Instance.AveragePos.x, transform.position.y, LightManager.Instance.AveragePos.z);
                var heading = target - transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;

                if (LightManager.Instance.SizeShadow > Size * 3.0f + dist / 10.0f) 
                    direction *= -1;

                //transform.LookAt(target);
                transform.position = transform.position + direction * Time.deltaTime;
            }
        }
    }
}
