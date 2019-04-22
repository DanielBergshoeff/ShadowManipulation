using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Turter : MonoBehaviour
{
    
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
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move();
        if (CheckAttackLight())
            return;

        float distLight = Vector3.Distance(transform.position, GameManager.Instance.LightObject.transform.position);
        float distPlayer = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);

        if (distLight < ViewRange || distPlayer < ViewRange) {
            float distPlayerFromLight = Vector3.Distance(GameManager.Instance.LightObject.transform.position, GameManager.Instance.Player.transform.position);

            if (scared /* && LightManager.Instance.SizeShadow > Size * 3.0f*/) {
                //var targetHeading = LightManager.Instance.AveragePos - transform.position;

                var targetHeading = GameManager.Instance.Player.transform.position - transform.position;
                var targetDirection = targetHeading / (targetHeading.magnitude);

                myNavMeshAgent.SetDestination(transform.position - targetDirection);
            }
            else if(distPlayerFromLight < LightManager.Instance.light.intensity * LightManager.Instance.lightRange){
                myNavMeshAgent.SetDestination(GameManager.Instance.LightObject.transform.position);
            }
            else {
                myNavMeshAgent.SetDestination(GameManager.Instance.Player.transform.position);
                if (CheckAttackPlayer())
                    return;
            }
        }
        else {
            myNavMeshAgent.isStopped = true;
            myNavMeshAgent.ResetPath();
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

    private bool CheckAttackLight() {
        if((GameManager.Instance.LightObject.transform.position - transform.position).magnitude < AttackRange){
            GameManager.Instance.LightMovementScript.TakeDamage(1);
            Destroy(gameObject);
            return true;
        }
        return false;
    }

    private bool CheckAttackPlayer() {
        if ((GameManager.Instance.Player.transform.position - transform.position).magnitude < AttackRange) {
            GameManager.Instance.Respawn();
            return true;
        }
        return false;
    }

    private void AttackLight() {
        
    }
}
