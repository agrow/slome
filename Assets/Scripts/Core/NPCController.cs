using TL.UtilityAI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace TL.Core
{
    public class NPCController : MonoBehaviour
    {
        [Header("AI Components")]
        public AIBrain aiBrain { get; set; }
        public NPCInventory Inventory { get; set; }
        public Stats stats { get; set; }
        public Context context { get; set; }  
        public EmotionalState emotionalState { get; set; }

        [Header("Movement Settings")]
        public float speed = 3f;
        
        [Header("Visual Components")]
        public Rigidbody rb;
        public SpriteRenderer sr;
        public Animator anim;
        
        [Header("Navigation")]
        public NavMeshAgent agent;

        private NavMeshMoverWrapper _mover;
        public NavMeshMoverWrapper mover 
        { 
            get 
            {
                if (_mover == null && agent != null)
                {
                    _mover = new NavMeshMoverWrapper(agent);
                }
                return _mover;
            }
        }

        public enum State
        {
            idle,
            move,
            execute,
            decide
        }
        
        private Vector2 lastMove;
        public State currentState { get; set; }

        void Start()
        {
            Debug.Log($"=== {name} NPC Starting Initialization ===");
            
            currentState = State.decide;
            
            // Initialize visual components
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (anim == null) anim = GetComponent<Animator>();
            
            // Initialize NavMeshAgent
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = speed;
                agent.stoppingDistance = 1f;
                agent.angularSpeed = 360f;
                agent.acceleration = 8f;
                
                // Prevent NPC from pushing player
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                agent.avoidancePriority = 99;
                
                Debug.Log($"{name}: NavMeshAgent initialized - Speed: {agent.speed}");
                
                // Auto-warp to NavMesh if not properly positioned
                if (!agent.isOnNavMesh)
                {
                    Debug.LogError($"{name}: NPC is NOT on NavMesh!");
                    
                    UnityEngine.AI.NavMeshHit hit;
                    if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        transform.position = hit.position;
                        Debug.Log($"{name}: Auto-warped to NavMesh at {hit.position}");
                    }
                }
                else
                {
                    Debug.Log($"{name}: ✅ NavMeshAgent is properly on NavMesh!");
                }

                _mover = new NavMeshMoverWrapper(agent);
            }
            else
            {
                Debug.LogError($"{name}: CRITICAL - NavMeshAgent component missing!");
            }
            
            // Get AI components
            aiBrain = GetComponent<AIBrain>();
            Inventory = GetComponent<NPCInventory>();
            stats = GetComponent<Stats>();
            
            // Find Context in scene
            if (context == null)
            {
                Context sceneContext = FindAnyObjectByType<Context>();
                if (sceneContext != null)
                {
                    context = sceneContext;
                    Debug.Log($"{name}: Found Context");
                }
                else
                {
                    Debug.LogWarning($"{name}: No Context found in scene!");
                }
            }
            
            // Get emotional state
            if (stats != null)
            {
                emotionalState = stats.GetEmotionalState();
            }
            
            // Check critical components
            if (aiBrain == null)
            {
                Debug.LogError($"{name}: CRITICAL - AIBrain component missing!");
                return;
            }
            
            if (stats == null)
            {
                Debug.LogError($"{name}: CRITICAL - Stats component missing!");
                return;
            }
            
            if (agent == null)
            {
                Debug.LogError($"{name}: CRITICAL - NavMeshAgent component missing!");
                return;
            }
            
            Debug.Log($"{name}: Initial state: {currentState}");
            Debug.Log($"=== {name} NPC Initialization Complete ===\n");
        }

        void Update()
        {
            FSMTick();
        }

        void LateUpdate()
        {
            HandleAnimations();
        }

        public void FSMTick()
        {
            if (aiBrain == null)
            {
                Debug.LogError($"{name}: Cannot tick FSM - AIBrain is null!");
                return;
            }

            if (currentState == State.decide)
            {
                Debug.Log($"\n--- {name}: DECIDE STATE ---");
                Debug.Log($"{name}: Stats - Energy: {stats?.energy ?? -1}, Hunger: {stats?.hunger ?? -1}, Money: {stats?.money ?? -1}");
                
                aiBrain.DecideBestAction();
                
                if (aiBrain.bestAction == null)
                {
                    Debug.LogError($"{name}: No best action found!");
                    return;
                }
                
                Debug.Log($"{name}: Best action: {aiBrain.bestAction.Name} (score: {aiBrain.bestAction.score})");
                
                aiBrain.bestAction.SetRequiredDestination(this);
                
                if (aiBrain.bestAction.RequiredDestination == null)
                {
                    Debug.LogError($"{name}: No destination set for action {aiBrain.bestAction.Name}!");
                    return;
                }
                
                if (agent != null)
                {
                    Vector3 destination = aiBrain.bestAction.RequiredDestination.position;
                    agent.SetDestination(destination);
                    agent.isStopped = false;
                    
                    Debug.Log($"{name}: NavMeshAgent destination set to: {destination}");
                    
                    float distance = Vector3.Distance(destination, transform.position);
                    Debug.Log($"{name}: Distance to target: {distance:F2}");
                    
                    if (distance < agent.stoppingDistance + 0.5f)
                    {
                        currentState = State.execute;
                        Debug.Log($"{name}: Close enough, switching to EXECUTE");
                    }
                    else
                    {
                        currentState = State.move;
                        Debug.Log($"{name}: Moving to destination");
                    }
                }
            }
            else if (currentState == State.move)
            {
                if (agent != null)
                {
                    float remainingDistance = agent.remainingDistance;
                    
                    if (!agent.pathPending && remainingDistance < agent.stoppingDistance + 0.5f)
                    {
                        currentState = State.execute;
                        agent.isStopped = true;
                        Debug.Log($"{name}: Reached destination! Switching to EXECUTE");
                    }
                }
            }
            else if (currentState == State.execute)
            {
                
                if (aiBrain.finishedExecutingBestAction == false)
                {
                    aiBrain.bestAction.Execute(this);
                }
                else if (aiBrain.finishedExecutingBestAction == true)
                {
                    if (agent != null)
                    {
                        agent.isStopped = false;
                    }
                    
                    currentState = State.decide;
                }
            }
        }

    private void HandleAnimations()
{
    if (anim == null || agent == null) return;

    Camera cam = Camera.main;
    if (cam == null) return;

    // USE FSM STATE FOR RELIABLE MOVEMENT DETECTION
    bool isMoving = (currentState == State.move) && !agent.isStopped && agent.hasPath;
    
    // Fallback to velocity if FSM says we should be moving
    if (!isMoving && currentState == State.move)
    {
        Vector3 velocity = agent.velocity;
        isMoving = velocity.magnitude > 0.05f;
    }
    
    if (isMoving)
    {
        // Get movement direction from NavMeshAgent
        Vector3 velocity = agent.velocity;
        if (velocity.magnitude > 0.01f)
        {
            lastMove = new Vector2(velocity.x, velocity.z).normalized;
        }
        
        // Convert to camera space
        Vector3 worldSpaceMovement = new Vector3(lastMove.x, 0, lastMove.y);
        Vector3 cameraRelativeMovement = cam.transform.InverseTransformDirection(worldSpaceMovement);
        
        
       
    }
    else
    {
        // Keep last direction in camera space
        Vector3 worldSpaceLastMove = new Vector3(lastMove.x, 0, lastMove.y);
        Vector3 cameraRelativeLastMove = cam.transform.InverseTransformDirection(worldSpaceLastMove);
        
        anim.SetFloat("lastMoveX", cameraRelativeLastMove.x);
        anim.SetFloat("lastMoveY", cameraRelativeLastMove.z);
        
    }
}    
        public void DoSleep(int duration)
        {
            Debug.Log($"{name}: DoSleep called - Duration: {duration} seconds");
            StartCoroutine(SleepCoroutine(duration));
        }

        private System.Collections.IEnumerator SleepCoroutine(int duration)
        {
            Debug.Log($"{name}: Starting to sleep for {duration} seconds");
            
            if (anim != null)
            {
                anim.SetBool("isSleeping", true);
            }
            
            yield return new WaitForSeconds(duration);
            
            if (stats != null)
            {
                int oldEnergy = stats.energy;
                stats.energy += 30;
                Debug.Log($"{name}: Finished sleeping - Energy: {oldEnergy} → {stats.energy}");
            }
            
            if (anim != null)
            {
                anim.SetBool("isSleeping", false);
            }
            
            aiBrain.finishedExecutingBestAction = true;
        }

        public void DoWork(int duration)
        {
            //Debug.Log($"{name}: DoWork called - Duration: {duration} seconds");
            StartCoroutine(WorkCoroutine(duration));
        }

        private System.Collections.IEnumerator WorkCoroutine(int duration)
        {
            //Debug.Log($"{name}: Starting to work for {duration} seconds");
            
            yield return new WaitForSeconds(duration);
            
            if (stats != null)
            {
                int oldMoney = stats.money;
                int oldEnergy = stats.energy;
                stats.money += 1;
                stats.energy -= 1;
                
                //Debug.Log($"{name}: Finished working - Money: {oldMoney} → {stats.money}, Energy: {oldEnergy} → {stats.energy}");
            }
            
            aiBrain.finishedExecutingBestAction = true;
        }
    }

    [System.Serializable]
    public class NavMeshMoverWrapper
    {
        private NavMeshAgent agent;
        
        public NavMeshMoverWrapper(NavMeshAgent navAgent)
        {
            agent = navAgent;
        }
        
        public Transform destination 
        { 
            get { return null; }
            set 
            { 
                if (agent != null && value != null)
                {
                    agent.SetDestination(value.position);
                    Debug.Log($"NavMeshMoverWrapper: Setting destination to {value.position}");
                }
                else if (agent == null)
                {
                    Debug.LogError("NavMeshMoverWrapper: NavMeshAgent is null!");
                }
                else if (value == null)
                {
                    Debug.LogError("NavMeshMoverWrapper: Destination transform is null!");
                }
            }
        }
    }
}