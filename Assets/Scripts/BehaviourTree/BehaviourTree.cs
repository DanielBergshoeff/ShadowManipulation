using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering.HDPipeline;


public class BehaviourTree : MonoBehaviour
{
    public float AngerLevel = 0f;
    public int AngerStage = 0;
    public StageInformation[] stageInformation;

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

        EnemyManager.AddEnemy(this);
    }

    // Update is called once per frame
    void Update()
    {
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
        if(distLight <= stageInformation[AngerStage].ViewRange) {
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

    NodeStates SneakToTarget() {
        float distShadow = Vector3.Distance(transform.position, LightManager.Instance.AveragePos);
        float speedCalculation = stageInformation[AngerStage].Size * distShadow - LightManager.Instance.SizeShadow;
        speedCalculation = Mathf.Clamp(speedCalculation / 50, -stageInformation[AngerStage].Speed, stageInformation[AngerStage].Speed);
        bool positive = speedCalculation >= 0;
        speedCalculation = Mathf.Clamp(Mathf.Abs(speedCalculation), stageInformation[AngerStage].Speed / 2, stageInformation[AngerStage].Speed);

        if (positive) {
            myNavMeshAgent.SetDestination(Target.transform.position);
        }
        else {
            var targetHeading = LightManager.Instance.AveragePos - transform.position;
            var targetDirection = targetHeading / (targetHeading.magnitude);
            /*
            var targetHeading = GameManager.Instance.Player.transform.position - transform.position;
            var targetDirection = targetHeading / (targetHeading.magnitude);
            */
            myNavMeshAgent.SetDestination(transform.position - targetDirection);
        }


        myNavMeshAgent.speed = speedCalculation;

        return NodeStates.SUCCESS;
    }

    NodeStates MoveToTarget() {
        throw new System.NotImplementedException();
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
