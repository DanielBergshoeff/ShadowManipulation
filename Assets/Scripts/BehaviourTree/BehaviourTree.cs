using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering.HDPipeline;


public class BehaviourTree : MonoBehaviour {
    public float AngerLevel = 0f;
    public int AngerStage = 0;
    public StageInformation[] stageInformation;

    private float attackCharge = 0.0f;

    private Attackable Target;

    private Selector RootSelector;

    private ActionNode SetPlayerTargetAction;
    private ActionNode SetLightTargetAction;
    private ActionNode AttackTargetAction;
    private ActionNode CheckIfScaredCondition;
    private ActionNode MoveToTargetAction;
    private ActionNode CircleAroundTargetAction;
    private ActionNode EscapeAction;
    
    private NavMeshAgent myNavMeshAgent;

    // Start is called before the first frame update
    void Start() {
        SetPlayerTargetAction = new ActionNode(SetPlayerTarget);
        SetLightTargetAction = new ActionNode(SetLightTarget);
        AttackTargetAction = new ActionNode(AttackTarget);
        CheckIfScaredCondition = new ActionNode(CheckIfScared);
        MoveToTargetAction = new ActionNode(MoveToTarget);
        EscapeAction = new ActionNode(Escape);
        CircleAroundTargetAction = new ActionNode(CircleAroundTarget);

        Selector setTargetSelectorNode = new Selector(new List<Node>() { SetPlayerTargetAction, SetLightTargetAction }); //If the player is not within range of the light, set player as target. Otherwise, set the light as target.
        Selector AttackSelector = new Selector(new List<Node>() { AttackTargetAction }); //If this unit is attacking, choose the appropriate attack
        Selector MoveTowardsSelector = new Selector(new List<Node>() {CircleAroundTargetAction, MoveToTargetAction }); //If the target is close, circle around the target, if the target is far, move in the direction of the target
        Selector AttackOrMoveSelector = new Selector(new List<Node>() { AttackSelector, MoveTowardsSelector }); //If the choice is to attack, choose between attacking (if close) or moving towards the target
        Sequence ScaredSequence = new Sequence(new List<Node>() { CheckIfScaredCondition, EscapeAction}); //If the choice is to retreat, do the escape action
        Selector AttackOrRetreat = new Selector(new List<Node>() { ScaredSequence, AttackOrMoveSelector }); //If a target has been set, choose whether to attack or retreat based on the size of the shadow
        Inverter SetTargetSelector = new Inverter(setTargetSelectorNode); //Set target, return false if target is set

        RootSelector = new Selector(new List<Node>() { SetTargetSelector, AttackOrRetreat }); //Set target, if target is set, check whether to attack or retreat


        myNavMeshAgent = GetComponent<NavMeshAgent>();

        EnemyManager.AddEnemy(this);
    }

    // Update is called once per frame
    void Update() {
        RootSelector.Evaluate();
    }

    NodeStates SetPlayerTarget() {
        float distPlayer = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);
        float distPlayerFromLight = Vector3.Distance(GameManager.Instance.LightObject.transform.position, GameManager.Instance.Player.transform.position);
        if (distPlayerFromLight > LightManager.Instance.light.GetComponent<HDAdditionalLightData>().intensity * LightManager.Instance.lightRange && distPlayer <= stageInformation[AngerStage].ViewRange) {
            Target = GameManager.Instance.Player.GetComponent<PlayerBehaviour>();
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    NodeStates SetLightTarget() {
        float distLight = Vector3.Distance(transform.position, GameManager.Instance.LightObject.transform.position);
        if (distLight <= stageInformation[AngerStage].ViewRange) {
            Target = GameManager.Instance.LightMovementScript;
            return NodeStates.SUCCESS;
        }

        return NodeStates.FAILURE;
    }

    NodeStates AttackTarget() {
        if ((Target.transform.position - transform.position).magnitude < stageInformation[AngerStage].AttackRange) {
            attackCharge += Time.deltaTime;
            if (attackCharge >= stageInformation[AngerStage].AttackTime) {
                Target.TakeDamage(stageInformation[AngerStage].Damage);
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

    NodeStates CheckIfScared() {
        float distShadow = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);
        float speedCalculation = stageInformation[AngerStage].Size * distShadow - LightManager.Instance.SizeShadow;
        speedCalculation = Mathf.Clamp(speedCalculation / 50, -stageInformation[AngerStage].Speed, stageInformation[AngerStage].Speed);
        bool positive = speedCalculation >= 0;

        if (positive)
            return NodeStates.SUCCESS;

        return NodeStates.FAILURE;
    }

    NodeStates MoveToTarget() {
        myNavMeshAgent.speed = stageInformation[AngerStage].Speed;
        myNavMeshAgent.SetDestination(Target.transform.position);
        return NodeStates.RUNNING;
    }

    NodeStates Escape() {
        var targetHeading = LightManager.Instance.AveragePos - transform.position;
        var targetDirection = targetHeading / (targetHeading.magnitude);
        myNavMeshAgent.speed = stageInformation[AngerStage].Speed;

        myNavMeshAgent.SetDestination(transform.position - targetDirection);
        return NodeStates.RUNNING;
    }

    NodeStates CircleAroundTarget() {
        Vector3 start = Target.transform.position - transform.position;
        if(start.sqrMagnitude <= stageInformation[AngerStage].DistanceToCircleAt) {
            Vector3 target = Quaternion.AngleAxis(1f, Vector3.up) * start.normalized;
            Vector3 pos = Target.transform.position + target * (stageInformation[AngerStage].DistanceToCircleAt - 1f);
            myNavMeshAgent.speed = stageInformation[AngerStage].Speed;
            myNavMeshAgent.SetDestination(pos);
            return NodeStates.RUNNING;
        }

        return NodeStates.FAILURE;
    }

    public void IncreaseAnger(float amt) {
        AngerLevel += amt;
        if (stageInformation.Length - 1 > AngerStage) {
            if (AngerLevel > stageInformation[AngerStage+1].AngerToStage) {
                AngerStage++;
                AngerLevel = 0f;
            }
        }
    }
}
