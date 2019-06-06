using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering.HDPipeline;


public class BehaviourTree : MonoBehaviour {
    public float AngerLevel = 0f;
    public int AngerStage = 0;
    public StageInformation[] stageInformation;
    public float animationTime = 3.0f;


    private float attackCharge = 0.0f;
    private float walkAroundTimer = 0.0f;
    private float attackTime = 0.0f;
    private float scaredTimer;
    private float jumpTimer = 0f;
    private bool jumpstart = true;
    private bool jumping = false;
    private bool preparingJump = false;
    private float jumpAnimationLength = 0f;
    private float jumpAnimationTimer = 0f;
    private Vector3 jumpStartingPosition;
    private Rigidbody myRigidbody;

    private Animator myAnimator;
    public LayerMask layerObstructions;

    private Attackable Target;
    private Vector3 targetPosition;

    private Node Root;
    private Sequence RootSequenceStageOne;
    private Sequence RootSequenceStageTwo;

    private ActionNode SetPlayerTargetAction;
    private ActionNode SetLightTargetAction;
    private ActionNode AttackTargetAction;
    private ActionNode CheckIfScaredCondition;
    private ActionNode MoveToTargetAction;
    private ActionNode CircleAroundTargetAction;
    private ActionNode EscapeAction;
    private ActionNode JumpAction;
    
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
        JumpAction = new ActionNode(Jump);
        
        //Stage 1
        Selector AttackSelector = new Selector(new List<Node>() { AttackTargetAction }); //If this unit is attacking, choose the appropriate attack
        Selector MoveTowardsSelector = new Selector(new List<Node>() {CircleAroundTargetAction, MoveToTargetAction }); //If the target is close, circle around the target, if the target is far, move in the direction of the target
        Selector AttackOrMoveSelector = new Selector(new List<Node>() { AttackSelector, MoveTowardsSelector }); //If the choice is to attack, choose between attacking (if close) or moving towards the target
        Sequence ScaredSequence = new Sequence(new List<Node>() { CheckIfScaredCondition, EscapeAction}); //If the choice is to retreat, do the escape action
        Selector AttackOrRetreat = new Selector(new List<Node>() { ScaredSequence, AttackOrMoveSelector }); //If a target has been set, choose whether to attack or retreat based on the size of the shadow
        Selector SetTargetSelector = new Selector(new List<Node>() { SetPlayerTargetAction, SetLightTargetAction }); //If the player is not within range of the light, set player as target. Otherwise, set the light as target.

        //Stage 2
        Selector MoveTowardsSelectorTwo = new Selector(new List<Node>() { JumpAction, CircleAroundTargetAction, MoveToTargetAction}); //If the target is close, circle around the target or jump towards the target. If the target is far, move in the direction of the target
        Selector AttackOrMoveSelectorTwo = new Selector(new List<Node>() { AttackSelector, MoveTowardsSelectorTwo}); //If the choice is to attack, choose between attacking (if close) or moving towards the target
        Selector AttackOrRetreatTwo = new Selector(new List<Node>() { ScaredSequence, AttackOrMoveSelectorTwo }); //If a target has been set, choose whether to attack or retreat based on the size of the shadow
        
        RootSequenceStageOne = new Sequence(new List<Node>() { SetTargetSelector, AttackOrRetreat }); //Set target, if target is set, check whether to attack or retreat
        RootSequenceStageTwo = new Sequence(new List<Node>() { SetTargetSelector, AttackOrRetreatTwo});

        if (AngerStage == 0)
            Root = RootSequenceStageOne;
        else if (AngerStage == 1)
            Root = RootSequenceStageTwo;

        attackTime = Random.Range(stageInformation[AngerStage].MinTimeTillAttack, stageInformation[AngerStage].MaxTimeTillAttack);
        myNavMeshAgent = GetComponent<NavMeshAgent>();

        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();

        jumpAnimationLength = GetClipLength("MonsterJumpDaniel");
        Debug.Log(jumpAnimationLength);

        EnemyManager.AddEnemy(this);
    }

    // Update is called once per frame
    void Update() {
        Root.Evaluate();

        if (Input.GetKey(KeyCode.E)) {
                transform.position = targetPosition;
        }
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
        Vector3 heightRemoved = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
        if ((heightRemoved - transform.position).sqrMagnitude < stageInformation[AngerStage].AttackRange * stageInformation[AngerStage].AttackRange) {
            attackCharge += Time.deltaTime;

            if (attackCharge >= stageInformation[AngerStage].AttackTime) {//If the attack has been charged up, apply damage and reset timers
                walkAroundTimer = 0.0f;
                attackTime = Random.Range(stageInformation[AngerStage].MinTimeTillAttack, stageInformation[AngerStage].MaxTimeTillAttack);
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

        if (positive) {
            scaredTimer = 0.0f;
            return NodeStates.FAILURE;
        }

            return NodeStates.SUCCESS;
    }

    NodeStates MoveToTarget() {
        myNavMeshAgent.speed = stageInformation[AngerStage].Speed;
        myNavMeshAgent.SetDestination(Target.transform.position);
        return NodeStates.RUNNING;
    }

    NodeStates Escape() {
        if (scaredTimer <= stageInformation[AngerStage].ScaredTime) { //Play scared animation for ScaredTime amt of seconds
            myNavMeshAgent.speed = 0f;
            scaredTimer += Time.deltaTime;
        }
        else { //After being scared start running away
            var targetHeading = LightManager.Instance.AveragePos - transform.position;
            var targetDirection = targetHeading / (targetHeading.magnitude);
            myNavMeshAgent.speed = stageInformation[AngerStage].Speed;
            myNavMeshAgent.SetDestination(transform.position - targetDirection);
        }
        return NodeStates.RUNNING;
    }

    NodeStates CircleAroundTarget() {
        Vector3 heightRemoved = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
        Vector3 start = heightRemoved - transform.position;

        if(start.sqrMagnitude <= stageInformation[AngerStage].DistanceToCircleAt * stageInformation[AngerStage].DistanceToCircleAt) { //If the Target is close enough to start circling around it

            if(walkAroundTimer >= attackTime) { //If this entity has been walking around for longer than the attackTime variable
                return NodeStates.FAILURE;
            }
            else {
                walkAroundTimer += Time.deltaTime;
                Vector3 target = (Quaternion.AngleAxis(10f, Vector3.up) * start.normalized).normalized;

                Vector3 pos = heightRemoved - target * (stageInformation[AngerStage].DistanceToCircleAt - 1f);

                RaycastHit hit;
                if (Physics.Raycast(heightRemoved, target, out hit, stageInformation[AngerStage].DistanceToCircleAt - 1f, layerObstructions)) {
                    pos = hit.point;
                }
                myNavMeshAgent.speed = stageInformation[AngerStage].Speed;
                myNavMeshAgent.SetDestination(pos);
            }

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
                if(AngerStage == 0) {
                    Root = RootSequenceStageOne;
                }
                else if(AngerStage == 1) {
                    Root = RootSequenceStageTwo;
                }
            }
        }
    }

    public void JumpOrNotJump() {
        jumping = !jumping;
        preparingJump = false;
        Debug.Log("Currently jumping: " + jumping.ToString());
    }

    NodeStates Jump() {
        if(jumpTimer == 0f) {
            jumpTimer = Random.Range(stageInformation[AngerStage].MinJumpTime, stageInformation[AngerStage].MaxJumpTime);
        }
        else if(jumpTimer < 0f) {
            if(jumpstart) {
                //Start jump preparing animation
                myAnimator.SetTrigger("Jump");
                Vector3 heading = transform.position - new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z);
                Vector3 direction = heading.normalized;
                Vector3 target = Vector3.zero;
                if(heading.magnitude - 1f <= stageInformation[AngerStage].MaxJumpLength) {
                    target = new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z) + direction * 1f;
                }
                else {
                    target = transform.position - direction * stageInformation[AngerStage].MaxJumpLength;
                }

                myNavMeshAgent.enabled = false;

                targetPosition = target;
                jumpstart = false;
                preparingJump = true;
                jumpAnimationTimer = 0f;
                jumpStartingPosition = transform.position;
                transform.LookAt(Target.transform);
            }
            else if (jumping) {
                //Play jumping animation
                transform.LookAt(Target.transform);
                jumpAnimationTimer += Time.deltaTime;
                jumpAnimationTimer = Mathf.Clamp(jumpAnimationTimer, 0f, jumpAnimationLength);
                Debug.Log(jumpAnimationTimer);
                transform.position = Vector3.Lerp(jumpStartingPosition, targetPosition, jumpAnimationTimer / jumpAnimationLength);
                Debug.Log(transform.position);
                Debug.Log((transform.position - new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z)).magnitude);
            }
            else if(!preparingJump){
                //End jump
                jumpTimer = 0f;
                jumpstart = true;
                transform.position = targetPosition;
                Debug.Log((transform.position - new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z)).magnitude);
                myNavMeshAgent.enabled = true;
                return NodeStates.SUCCESS;
            }
            return NodeStates.RUNNING;
        }
        else {
            jumpTimer -= Time.deltaTime;
        }

        return NodeStates.FAILURE;
    }

    private float GetClipLength(string clipName) {
        RuntimeAnimatorController ac = myAnimator.runtimeAnimatorController;    //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == clipName)        //If it has the same name as your clip
            {
                return ac.animationClips[i].length;
            }
        }

        return 0f;
    }
}
