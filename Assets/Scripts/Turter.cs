using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Turter : MonoBehaviour
{
    public static GameObject TurterTarget;
    public static LightMovement TurterTargetLight;


    public GameObject LocalTarget;

    public float Size = 3.0f;
    public float Speed = 1.0f;
    public float ViewRange = 50.0f;
    public float AttackRange = 1.0f;

    private NavMeshAgent myNavMeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        if (TurterTarget == null && LocalTarget != null)
            TurterTarget = LocalTarget;

        if(TurterTargetLight == null && TurterTarget != null)
            TurterTargetLight = TurterTarget.GetComponent<LightMovement>();

        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CheckAttack();
    }

    private void Move() {
        if (LightManager.Instance.ShadowFound) {
            float dist = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);
            if (dist <= ViewRange) {
                Vector3 target = new Vector3(LightManager.Instance.AveragePos.x, transform.position.y, LightManager.Instance.AveragePos.z);
                var heading = target - transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;

                if (LightManager.Instance.SizeShadow > Size * 3.0f + dist / 10.0f) { //If the shadow is big
                    myNavMeshAgent.SetDestination(transform.position - direction);
                }
                else {
                    //transform.LookAt(target);
                    myNavMeshAgent.SetDestination(TurterTarget.transform.position); //If the shadow is relatively small
                }
            }
        }
    }

    private void CheckAttack() {
        if((new Vector3(TurterTarget.transform.position.x, transform.position.y, TurterTarget.transform.position.z) - transform.position).magnitude < AttackRange){
            Attack();
        }
    }

    private void Attack() {
        TurterTargetLight.TakeDamage();
    }
}
