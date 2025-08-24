using TL.UtilityAI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
        [SerializeField, Range(1f, 20f)] // This creates a slider in the Inspector
        private float interactionRange = 5f;


        [Header("Action States")]
        public bool isFlirting = false;

        public bool isSocializing = false; 
        public bool isWorking = false;

        public bool isExecutingAction = false; 

        private NavMeshMoverWrapper _mover;

        // What does this NavNesgWrapper do?
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
            decide,
            active
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
                
                // Debug.Log($"{name}: NavMeshAgent initialized - Speed: {agent.speed}");
                
                // Auto-warp to NavMesh if not properly positioned
                if (!agent.isOnNavMesh)
                {
                    //Debug.LogError($"{name}: NPC is NOT on NavMesh!");
                    
                    UnityEngine.AI.NavMeshHit hit; // what does this do?
                    if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        transform.position = hit.position;
                        //Debug.Log($"{name}: Auto-warped to NavMesh at {hit.position}");
                    }
                }
                else
                {
                    Debug.Log($"{name}:  NavMeshAgent is properly on NavMesh!");
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

        // This is where the differiation between emotional (active) and autommonus (passive) systems are decided
        /* 
            NPC Controller should almost differiate between when the two systems will be used:
            - Active: When the player chooses to interacte with the NPC, NPC should stop, listen, do respond from players action.
            - Passive: When the NPC is not interacting with the player doing what it needs to do automoiusly
        */
        
        public void FSMTick()
        {
            if (aiBrain == null)
            {
                Debug.LogError($"{name}: Cannot tick FSM - AIBrain is null!");
                return;
            }

            if (currentState == State.active)
            {
                // The Emotional AI Brain should be listening for players response
                // Interpret the action of the player
                // Enter the Emotional Utility System 
                // When player does interacts, the state is now listening to the player.
                // maybe we need ShouldUseEmotionalActions() as a constant listening port to see if we need to go into
                // emotional / interaction state 
                // right now... player has to be in perfect timing for the decide state, however deciding can be instanteously?


    
            }
        
            if (currentState == State.decide)
            {
                Debug.Log($"\n--- {name}: DECIDE STATE ---");
                Debug.Log($"{name}: Stats - Energy: {stats?.energy ?? -1}, Hunger: {stats?.hunger ?? -1}, Money: {stats?.money ?? -1}");
                
                // Check if we should use emotional actions instead of regular actions
                EmotionalAIBrain emotionalBrain = GetComponent<EmotionalAIBrain>();
                bool useEmotionalAction = false;
                EmotionalAction selectedEmotionalAction = null;
        
                if (emotionalBrain != null && ShouldUseEmotionalActions())
                {
                    Debug.Log($"{name}: Using EmotionalAIBrain for decision making");
                    emotionalBrain.DecideBestEmotionalAction(this);
                    
                    if (emotionalBrain.bestEmotionalAction != null)
                    {
                        selectedEmotionalAction = emotionalBrain.bestEmotionalAction;
                        useEmotionalAction = true;
                        Debug.Log($"{name}: Selected emotional action: {selectedEmotionalAction.Name} (score: {selectedEmotionalAction.score})");
                    }
                }
                
                // If no emotional action was selected, use regular AI decision making
                if (!useEmotionalAction)
                {
                    Debug.Log($"{name}: Using regular AIBrain for decision making");
                    aiBrain.DecideBestAction();
                    
                    if (aiBrain.bestAction == null)
                    {
                        Debug.LogError($"{name}: No best action found!");
                        return;
                    }
                    
                    Debug.Log($"{name}: Best action: {aiBrain.bestAction.Name} (score: {aiBrain.bestAction.score})");
                }
                
                // Set destination based on action type
                Transform targetDestination = null;
                string actionName = "";
                
                if (useEmotionalAction)
                {
                    selectedEmotionalAction.SetRequiredDestination(this);
                    targetDestination = selectedEmotionalAction.RequiredDestination;
                    actionName = selectedEmotionalAction.Name;
                }
                else
                {
                    aiBrain.bestAction.SetRequiredDestination(this);
                    targetDestination = aiBrain.bestAction.RequiredDestination;
                    actionName = aiBrain.bestAction.Name;
                }
                
                if (targetDestination == null)
                {
                    Debug.LogError($"{name}: No destination set for action {actionName}!");
                    return;
                }
                
                if (agent != null)
                {
                    Vector3 destination = targetDestination.position;
                    agent.SetDestination(destination);
                    agent.isStopped = false;
                    
                    //Debug.Log($"{name}: NavMeshAgent destination set to: {destination}");
                    
                    float distance = Vector3.Distance(destination, transform.position);
                    //Debug.Log($"{name}: Distance to target: {distance:F2}");
                    
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
                // Check if we're executing an emotional action
                EmotionalAIBrain emotionalBrain = GetComponent<EmotionalAIBrain>();
                bool isEmotionalAction = emotionalBrain != null && 
                                        emotionalBrain.bestEmotionalAction != null;
                
                if (isEmotionalAction)
                {
                    // Handle emotional action execution - ONLY execute once
                    if (!isExecutingAction)
                    {
                        Debug.Log($"{name}: Starting emotional action: {emotionalBrain.bestEmotionalAction.Name}");
                        isExecutingAction = true;
                        emotionalBrain.finishedExecutingBestEmotionalAction = false; // Ensure it's false
                        emotionalBrain.bestEmotionalAction.Execute(this);
                    }
                    
                    // Wait for emotional action coroutine to signal completion
                    if (emotionalBrain.finishedExecutingBestEmotionalAction)
                    {
                        if (agent != null)
                        {
                            agent.isStopped = false;
                        }
                        
                        // Reset emotional brain state
                        isExecutingAction = false;
                        emotionalBrain.finishedExecutingBestEmotionalAction = false;
                        emotionalBrain.bestEmotionalAction = null;
                        
                        currentState = State.decide;
                        Debug.Log($"{name}: Finished executing emotional action, returning to DECIDE");
                    }
                }
                else
                {
                    // Handle regular action execution - ONLY execute once
                    if (!isExecutingAction)
                    {
                        Debug.Log($"{name}: Starting regular action: {aiBrain.bestAction.Name}");
                        isExecutingAction = true;
                        aiBrain.finishedExecutingBestAction = false; // Ensure it's false
                        aiBrain.bestAction.Execute(this);
                    }
                    
                    // Wait for regular action coroutine to signal completion
                    if (aiBrain.finishedExecutingBestAction)
                    {
                        if (agent != null)
                        {
                            agent.isStopped = false;
                        }
                        
                        // Reset regular brain state
                        isExecutingAction = false;
                        aiBrain.finishedExecutingBestAction = false;
                        
                        currentState = State.decide;
                        Debug.Log($"{name}: Finished executing regular action, returning to DECIDE");
                    }
                }
            }
        }

        // Helper method to determine when to use emotional actions
        private bool ShouldUseEmotionalActions()
        {
            bool shouldUse = false;
            // if player pressed T next to the NPC, it means the player intends to interact with you
            if (Input.GetKeyDown(KeyCode.T) && IsPlayerNearby())
            {
                Debug.Log($"{name}: Player pressed T, so we should use emotional actions");
                shouldUse = true;
            }         
            // if (emotionalState == null) return false;

            // Use emotional actions when:
            // 1. High arousal (excited/passionate state)
            // 2. High pleasure (happy/positive state)  
            // 3. Player is nearby (social opportunity)

            // bool highArousal = emotionalState.Arousal > 0.0f;
            // bool highPleasure = emotionalState.Pleasure > 0.0f;
            // bool playerNearby = IsPlayerNearby();

            // bool shouldUse = highArousal || highPleasure || playerNearby;

            // if (shouldUse)
            // {
            //     Debug.Log($"{name}: Should use emotional actions - Arousal: {emotionalState.Arousal:F2}, Pleasure: {emotionalState.Pleasure:F2}, Player nearby: {playerNearby}");
            // }

            return shouldUse;
        }


        // Helper method to check if player is nearby
        
        private bool IsPlayerNearby()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return false;
            
            float distance = Vector3.Distance(transform.position, player.transform.position);
            
            // Optional: Debug visualization
            #if UNITY_EDITOR
            Debug.DrawLine(transform.position, player.transform.position, 
                distance <= interactionRange ? Color.green : Color.red);
            #endif
            
            return distance <= interactionRange;
        }

        private void HandleAnimations()
        {
            if (anim == null || agent == null) return;
            Camera cam = Camera.main;
            if (cam == null) return;

            // Determine if NPC is moving
            bool isMoving = (currentState == State.move) && !agent.isStopped && agent.hasPath;

            if (!isMoving && currentState == State.move)
            {
                // Fallback: check velocity magnitude
                isMoving = agent.velocity.magnitude > 0.05f;
            }

            if (isMoving)
            {
                //  Tell animator we're moving
                anim.SetBool("isMoving", true);

                Vector3 velocity = agent.velocity;
                if (velocity.magnitude > 0.01f)
                {
                    lastMove = new Vector2(velocity.x, velocity.z).normalized;
                }

                // Convert world-space to camera-relative direction
                Vector3 worldSpaceMovement = new Vector3(lastMove.x, 0, lastMove.y);
                Vector3 cameraRelativeMovement = cam.transform.InverseTransformDirection(worldSpaceMovement);

                // Update blend tree parameters
                anim.SetFloat("lastMoveX", cameraRelativeMovement.x);
                anim.SetFloat("lastMoveY", cameraRelativeMovement.z);
            }
            else
            {
                //  Idle: stop walking, but keep last facing direction
                anim.SetBool("isMoving", false);

                Vector3 worldSpaceLastMove = new Vector3(lastMove.x, 0, lastMove.y);
                Vector3 cameraRelativeLastMove = cam.transform.InverseTransformDirection(worldSpaceLastMove);

                anim.SetFloat("lastMoveX", cameraRelativeLastMove.x);
                anim.SetFloat("lastMoveY", cameraRelativeLastMove.z);
            }
        }

        // action methods here
        public void DoWork(int duration)
        {
            StartCoroutine(WorkCoroutine(duration));
        }

        private IEnumerator WorkCoroutine(int duration)
        {
            isWorking = true;
            
            yield return new WaitForSeconds(duration);
            
            if (stats != null)
            {
                int oldMoney = stats.money;
                int oldEnergy = stats.energy;
                stats.money += 1;
                stats.energy -= 1;
                Debug.Log($"{name}: Work complete - Money: {oldMoney} → {stats.money}, Energy: {oldEnergy} → {stats.energy}");
            }
            
            isWorking = false;
            aiBrain.finishedExecutingBestAction = true;
        }

        public void DoSleep(int duration)
        {
            Debug.Log($"{name}: DoSleep called - Duration: {duration} seconds");
            StartCoroutine(SleepCoroutine(duration));
        }

        private IEnumerator SleepCoroutine(int duration)
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