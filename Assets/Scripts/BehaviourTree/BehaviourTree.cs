using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering.HDPipeline;


public class BehaviourTree : MonoBehaviour
{
    public float Size = 3.0f;
    public float ViewRange = 20.0f;
    public float AttackRange = 1.0f;
    public int Damage = 1;
    public float AttackTime = 0.5f;
    public float Speed = 3.0f;

    private float attackCharge = 0.0f;

    private Attackable Target;

    private Selector RootSelector;
    private Inverter SetTargetSelector;

    private ActionNode SetPlayerTargetAction;
    private ActionNode SetLightTargetAction;
    private ActionNode AttackTargetAction;
    private ActionNode SneakToTargetAction;
    private ActionNode MoveToTargetAction;

    private Selector AttackSelector;
    private Selector MoveTowardsSelector;

    private NavMeshAgent myNavMeshAgent;
    
    // Start is called before the first frame update
    void Start()
    {
        SetPlayerTargetAction = new ActionNode(SetPlayerTarget);
        SetLightTargetAction = new ActionNode(SetLightTarget);
        AttackTargetAction = new ActionNode(AttackTarget);
        SneakToTargetAction = new ActionNode(SneakToTarget);
        MoveToTargetAction = new ActionNode(MoveToTarget);

        Selector setTargetSelectorNode = new Selector(new List<Node>() { SetPlayerTargetAction, SetLightTargetAction });
        SetTargetSelector = new Inverter(setTargetSelectorNode);
        AttackSelector = new Selector(new List<Node>() { AttackTargetAction });
        MoveTowardsSelector = new Selector(new List<Node>() { SneakToTargetAction, MoveToTargetAction });

        RootSelector = new Selector(new List<Node>() { SetTargetSelector, AttackSelector, MoveTowardsSelector });


        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        RootSelector.Evaluate();
    }

    NodeStates SetPlayerTarget() {
        float distPlayer = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);
        float distPlayerFromLight = Vector3.Distance(GameManager.Instance.LightObject.transform.position, GameManager.Instance.Player.transform.position);
        if (distPlayerFromLight > LightManager.Instance.light.GetComponent<HDAdditionalLightData>().intensity * LightManager.Instance.lightRange && distPlayer <= ViewRange) {
            Target = GameManager.Instance.Player.GetComponent<PlayerBehaviour>();
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    NodeStates SetLightTarget() {
        float distLight = Vector3.Distance(transform.position, GameManager.Instance.LightObject.transform.position);
        if(distLight <= ViewRange) {
            Target = GameManager.Instance.LightMovementScript;
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    NodeStates AttackTarget() {
        if ((Target.transform.position - transform.position).magnitude < AttackRange) {
            attackCharge += Time.deltaTime;
            if (attackCharge >= AttackTime) {
                Target.TakeDamage(Damage);
                Destroy(gameObject);
                return NodeStates.SUCCESS;
            }
            else {
                return NodeStates.RUNNING;
            }
        }
        attackCharge = 0.0f;
        return NodeStates.FAILURE;
    }

    NodeStates SneakToTarget() {
        float distShadow = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);
        float speedCalculation = Size * distShadow - LightManager.Instance.SizeShadow;
        speedCalculation = Mathf.Clamp(speedCalculation / 50, -Speed, Speed);
        bool positive = speedCalculation >= 0;
        speedCalculation = Mathf.Clamp(Mathf.Abs(speedCalculation), Speed / 2, Speed);

        if (positive) {
            myNavMeshAgent.SetDestination(Target.transform.position);
        }
        else {
            var targetHeading = GameManager.Instance.Player.transform.position - transform.position;
            var targetDirection = targetHeading / (targetHeading.magnitude);

            myNavMeshAgent.SetDestination(transform.position - targetDirection);
        }


        myNavMeshAgent.speed = speedCalculation;

        return NodeStates.SUCCESS;
    }

    NodeStates MoveToTarget() {
        throw new System.NotImplementedException();
    }
}
