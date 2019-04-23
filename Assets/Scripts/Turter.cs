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
    public bool oldSystem = false;

    private NavMeshAgent myNavMeshAgent;

    // Start is called before the first frame update
    void Start()
    {        
        myNavMeshAgent = GetComponent<NavMeshAgent>();
        myNavMeshAgent.speed = Speed;
    }

    // Update is called once per frame
    void Update() {
        //Move();
        if (CheckAttackLight())
            return;

        float distLight = Vector3.Distance(transform.position, GameManager.Instance.LightObject.transform.position);
        float distPlayer = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);
        float distShadow = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);
        

        if (oldSystem) {
            if (distLight < ViewRange || distPlayer < ViewRange) {
                float distPlayerFromLight = Vector3.Distance(GameManager.Instance.LightObject.transform.position, GameManager.Instance.Player.transform.position);

                if (scared /* && LightManager.Instance.SizeShadow > Size * 3.0f*/) {
                    //var targetHeading = LightManager.Instance.AveragePos - transform.position;

                    var targetHeading = GameManager.Instance.Player.transform.position - transform.position;
                    var targetDirection = targetHeading / (targetHeading.magnitude);

                    myNavMeshAgent.SetDestination(transform.position - targetDirection);
                }
                else if (distPlayerFromLight < LightManager.Instance.light.intensity * LightManager.Instance.lightRange) {
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
        else {
            if (distLight < ViewRange || distPlayer < ViewRange) { //If the light is within range of the turtle
                float speedCalculation = Size * distShadow - LightManager.Instance.SizeShadow;
                speedCalculation = Mathf.Clamp(speedCalculation / 50, -1f, 1f);

                if (speedCalculation >= 0) { //If the speed calculated is positive
                    speedCalculation = Mathf.Clamp(speedCalculation, 0.5f, 1.0f);

                    myNavMeshAgent.speed = Speed * speedCalculation;
                float distPlayerFromLight = Vector3.Distance(GameManager.Instance.LightObject.transform.position, GameManager.Instance.Player.transform.position);

                    if(distPlayer < ViewRange && distPlayerFromLight > LightManager.Instance.light.intensity * LightManager.Instance.lightRange) { //If the player is within range and the player is out of range of the light
                        myNavMeshAgent.SetDestination(GameManager.Instance.Player.transform.position);
                        CheckAttackLight();
                    }
                    else {
                        myNavMeshAgent.SetDestination(GameManager.Instance.LightObject.transform.position);
                        CheckAttackPlayer();
                    }
                }
                else { //If the speed calculated is negative
                    speedCalculation = Mathf.Clamp(Mathf.Abs(speedCalculation), 0.5f, 1.0f);
                    myNavMeshAgent.speed = Speed * speedCalculation;

                    var targetHeading = LightManager.Instance.AveragePos - transform.position;
                    var targetDirection = targetHeading / (targetHeading.magnitude);

                    myNavMeshAgent.SetDestination(transform.position - targetDirection);
                }
            }
            else {
                myNavMeshAgent.isStopped = true;
                myNavMeshAgent.ResetPath();
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
