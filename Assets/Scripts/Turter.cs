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
    public float ViewRange = 20.0f;
    public float AttackRange = 1.0f;
    public float RunTime = 5.0f;

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

        float distLight = Vector3.Distance(transform.position, TurterTarget.transform.position);
        float distPlayer = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);

        if (distLight < ViewRange || distPlayer < ViewRange) {
            float distPlayerFromLight = Vector3.Distance(TurterTarget.transform.position, GameManager.Instance.Player.transform.position);

            if (scared && LightManager.Instance.SizeShadow > Size * 3.0f) {
                var targetHeading = LightManager.Instance.AveragePos - transform.position;
                var targetDirection = targetHeading / (targetHeading.magnitude);

                myNavMeshAgent.SetDestination(transform.position - targetDirection);
            }
            else if(distPlayerFromLight < LightManager.Instance.light.intensity * LightManager.Instance.lightRange){
                myNavMeshAgent.SetDestination(TurterTarget.transform.position);
            }
            else {
                myNavMeshAgent.SetDestination(GameManager.Instance.Player.transform.position);
            }
        }
    }

    private void Unscare() {
        scared = false;
    }

    public void Scare() {
        if (!scared) {
            scared = true;
            Invoke("Unscare", RunTime);
        }
    }

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
