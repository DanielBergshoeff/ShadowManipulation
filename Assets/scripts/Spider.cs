using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
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
            float dist = Vector2.Distance(transform.position, LightManager.Instance.AveragePos);
                Vector3 target = new Vector3(LightManager.Instance.AveragePos.x, LightManager.Instance.AveragePos.y, transform.position.z);
                var heading = target - transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;
            
                //transform.LookAt(target);
                transform.position = transform.position + direction * Time.deltaTime;
        }
    }
}
