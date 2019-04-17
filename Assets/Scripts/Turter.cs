using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Turter : MonoBehaviour
{
    public static GameObject TurterTarget;
    public static LightMovement TurterTargetLight;
    public static GameObject PlayerTarget;


    public GameObject LocalTarget;
    public GameObject LocalPlayerTarget;

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

        if(PlayerTarget == null && LocalPlayerTarget != null)
            PlayerTarget = LocalPlayerTarget;
        
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move();
        if (CheckAttack())
            return;

        float distLight = Vector3.Distance(transform.position, TurterTarget.transform.position);
        float distPlayer = Vector3.Distance(transform.position, PlayerTarget.transform.position);

        if (distLight < ViewRange || distPlayer < ViewRange) {
            float distPlayerFromLight = Vector3.Distance(TurterTarget.transform.position, PlayerTarget.transform.position);

            if (scared && LightManager.Instance.SizeShadow > Size * 3.0f) {
                var targetHeading = LightManager.Instance.AveragePos - transform.position;
                var targetDirection = targetHeading / (targetHeading.magnitude);

                myNavMeshAgent.SetDestination(transform.position - targetDirection);
            }
            else if(distPlayerFromLight < LightManager.Instance.light.intensity * LightManager.Instance.lightRange){
                myNavMeshAgent.SetDestination(TurterTarget.transform.position);
            }
            else {
                myNavMeshAgent.SetDestination(PlayerTarget.transform.position);
            }
        }

        scared = false;
    }

    public void Scare() {
        scared = true;
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
