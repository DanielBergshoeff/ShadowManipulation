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

    private bool scared = false;

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
        //Move();
        if (CheckAttack())
            return;

        float dist = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);

        if (scared && LightManager.Instance.SizeShadow > Size * 3.0f) {
            var targetHeading = LightManager.Instance.AveragePos - transform.position;
            var targetDirection = targetHeading / (targetHeading.magnitude);

            myNavMeshAgent.SetDestination(transform.position - targetDirection);
        }
        else {
            myNavMeshAgent.SetDestination(TurterTarget.transform.position);
        }

        scared = false;
    }

    public void Scare() {
        scared = true;
    }


    /*
    private void Move() {
        if (LightManager.Instance.ShadowFound) {
            float dist = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);
            if (dist <= ViewRange) {
                Vector3 target = new Vector3(LightManager.Instance.AveragePos.x, transform.position.y, LightManager.Instance.AveragePos.z);
                var heading = target - transform.position;
                var distance = heading.magnitude;
                var direction = heading / distance;

                if (LightManager.Instance.SizeShadow > Size * 3.0f + dist / 10.0f) { //If the shadow is big
                    float closestPointDistance = float.PositiveInfinity;
                    Vector3 closestPoint = Vector3.zero;
                    foreach(Vector3 point in LightManager.Instance.allShadowPoints) {
                        float distancePoint = (point - transform.position).sqrMagnitude;
                        if (distancePoint < closestPointDistance) {
                            closestPointDistance = distancePoint;
                            closestPoint = point;
                        }
                    }
                    var targetHeading = closestPoint - transform.position;
                    var targetDirection = targetHeading / (targetHeading.magnitude);
                    if(closestPointDistance < LightManager.Instance.SizeShadow * 0.5f)
                        myNavMeshAgent.SetDestination(transform.position - targetDirection);
                    else
                        myNavMeshAgent.SetDestination(TurterTarget.transform.position);
                }
                else {
                    //transform.LookAt(target);
                    myNavMeshAgent.SetDestination(TurterTarget.transform.position); //If the shadow is relatively small
                }
            }
        }
    } */

    private bool CheckAttack() {
        if((new Vector3(TurterTarget.transform.position.x, transform.position.y, TurterTarget.transform.position.z) - transform.position).magnitude < AttackRange){
            Attack();
            return true;
        }
        return false;
    }

    private void Attack() {
        TurterTargetLight.TakeDamage();
        Destroy(gameObject);
    }
}
