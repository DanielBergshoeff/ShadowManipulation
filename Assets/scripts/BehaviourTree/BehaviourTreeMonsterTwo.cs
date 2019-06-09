using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class BehaviourTreeMonsterTwo : MonoBehaviour
{
    public float ViewRange;
    public float Speed;
    public float TimeLightDownToThrow = 10.0f;
    public float LightAttackDistance = 3.0f;
    public float PlayerAttackDistance = 2.0f;
    public float MaxLightHeight = 2.0f;
    public float AttackDistanceMargin = 0.1f;

    private float lightDownTimer = 0f;
    private Vector3 heightRemoved;
    private NavMeshAgent myNavMeshAgent;
    private bool attacking = false;
    private bool aboveGround = false;

    private Attackable Target;
    private Animator myAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.LightMovementScript.Height <= MaxLightHeight && Vector3.Distance(new Vector3(GameManager.Instance.LightObject.transform.position.x, transform.position.y, GameManager.Instance.LightObject.transform.position.z), transform.position) <= ViewRange && !attacking) {
            lightDownTimer += Time.deltaTime;
            if (lightDownTimer >= TimeLightDownToThrow) {
                Target = GameManager.Instance.LightObject.GetComponent<Attackable>();
            }
        }

        if (Target == null) {
            if(Vector3.Distance(transform.position, new Vector3(GameManager.Instance.Player.transform.position.x, transform.position.y, GameManager.Instance.Player.transform.position.z)) <= ViewRange) {
                Target = GameManager.Instance.LightObject.GetComponent<Attackable>();
            }
            else
                return;
        }

        heightRemoved = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);

        if (attacking) {
            transform.position = heightRemoved;
            transform.LookAt(heightRemoved);
            return;
        }


        if (Target.gameObject == GameManager.Instance.LightObject) { //If the Light is the target
            if(Vector3.Distance(transform.position, heightRemoved) > LightAttackDistance + AttackDistanceMargin || Vector3.Distance(transform.position, heightRemoved) < LightAttackDistance - AttackDistanceMargin) {
                Vector3 direction = (transform.position - heightRemoved).normalized;
                Vector3 targetPosition = heightRemoved + direction * LightAttackDistance;
                myNavMeshAgent.speed = Speed;
                myNavMeshAgent.SetDestination(targetPosition);
                myAnimator.SetFloat("Speed", myNavMeshAgent.speed);
            }
            else if(!attacking){
                myNavMeshAgent.speed = 0f;
                //myNavMeshAgent.enabled = false;
                myAnimator.SetFloat("Speed", myNavMeshAgent.speed);
                myAnimator.SetTrigger("HitUp");
                attacking = true;
            }
        }
        else { //If the Player is the target
            if (Vector3.Distance(transform.position, heightRemoved) > PlayerAttackDistance + AttackDistanceMargin || Vector3.Distance(transform.position, heightRemoved) < PlayerAttackDistance - AttackDistanceMargin) {
                Vector3 direction = (transform.position - heightRemoved).normalized;
                Vector3 targetPosition = heightRemoved + direction * PlayerAttackDistance;
                myNavMeshAgent.speed = Speed;
                myNavMeshAgent.SetDestination(targetPosition);
                myAnimator.SetFloat("Speed", myNavMeshAgent.speed);
            }
            else if(!attacking){
                myNavMeshAgent.speed = 0f;
                //myNavMeshAgent.enabled = false;
                myAnimator.SetFloat("Speed", myNavMeshAgent.speed);
                myAnimator.SetTrigger("GrabPlayer");
                attacking = true;
            }
        }
        
    }

    public void StopAttack() {
        attacking = false;
        lightDownTimer = 0f;
        Target = GameManager.Instance.Player.GetComponent<Attackable>();
    }


    public void ThrowLightUp() {
        GameManager.Instance.LightMovementScript.ThrowUp();
    }
}
