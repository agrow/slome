using TL.UtilityAI;
using TL.EmotionalAI;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

namespace TL.Core
{
    /// <summary>
    /// NPCController implementation that uses EmotionBrain (new EmotionalAI system) and AIBrain.
    /// Uses IAction interface for unified action handling.
    /// </summary>
    public class NPCController : MonoBehaviour
    {
        // Action States for AIBrain 
        [HideInInspector]
        public bool isFlirting = false;
        [HideInInspector]   
        public bool isSocializing = false;
        [HideInInspector]
        public bool isWorking = false;
        [HideInInspector]
        public bool isExecutingAction = false;

        [Header("AI Components")]
        public AIBrain aiBrain { get; private set; }
        public EmotionBrain emotionBrain { get; private set; }
        public EmotionModel emotionModel { get; private set; }

        [Header("Other Components")]
        public NPCInventory Inventory { get; private set; }
        public Stats stats { get; private set; }
        public Context context { get; private set; }
        public EmotionalState emotionalState { get; private set; }
        public NavMeshAgent agent { get; private set; }
        public Animator anim { get; private set; }

        [Header("Movement Settings")]
        public float speed = 3f;
        [SerializeField, Range(1f, 20f)]
        private float interactionRange = 5f;

        [Header("Emotional AI Settings")]
        [SerializeField] public float emotionalTimeout = 2f; // Time for one emotional response
        public float lastEmotionalTrigger = -999f; // When T was last pressed
        public bool hasRespondedToCurrentTrigger = false; // Prevent multiple responses per T press

        // State machine
        public enum State { idle, move, execute, decide }
        public State currentState { get; private set; }

        private IAction currentAction;
        private Vector2 lastMove;

        //purpose stmt: initialize NPC components,setup, enter decision making state machine
        void Start()
        {
            Debug.Log($"=== {name} NPC Starting Initialization ===");

            // Initialize components
            InitializeComponents();
            
            // Setup NavMesh
            InitializeNavMesh();
            
            // Validate critical components
            ValidateComponents();
            
            currentState = State.decide;
            
            Debug.Log($"{name}: Initialization complete. Starting in {currentState} state");
        }

        private void InitializeComponents()
        {
            aiBrain = GetComponent<AIBrain>();
            emotionBrain = GetComponent<EmotionBrain>();
            emotionModel = GetComponent<EmotionModel>();
            Inventory = GetComponent<NPCInventory>();
            stats = GetComponent<Stats>();
            context = FindAnyObjectByType<Context>();
            emotionalState = GetComponent<EmotionalState>();
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();

            // Get emotional state from stats if available
            if (stats != null && emotionalState == null)
            {
                emotionalState = stats.GetEmotionalState();
            }
        }

        private void InitializeNavMesh()
        {
            if (agent != null)
            {
                agent.speed = speed;
                agent.stoppingDistance = 1f;
                agent.angularSpeed = 360f;
                agent.acceleration = 8f;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                agent.avoidancePriority = 99;

                // Ensure agent is on NavMesh
                if (!agent.isOnNavMesh)
                {
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(transform.position, out hit, 10f, NavMesh.AllAreas))
                    {
                        transform.position = hit.position;
                        Debug.Log($"{name}: Repositioned to NavMesh");
                    }
                    else
                    {
                        Debug.LogError($"{name}: Could not place on NavMesh!");
                    }
                }
            }
        }

        private void ValidateComponents()
        {
            if (aiBrain == null)
                Debug.LogWarning($"{name}: AIBrain component missing!");
            
            if (emotionBrain == null)
                Debug.LogWarning($"{name}: EmotionBrain component missing!");
            
            if (emotionModel == null)
                Debug.LogWarning($"{name}: EmotionModel component missing!");
            
            if (stats == null)
                Debug.LogError($"{name}: Stats component missing!");
            
            if (agent == null)
                Debug.LogError($"{name}: NavMeshAgent component missing!");
        }

        void Update()
        {
            FSMTick();
        }

        void LateUpdate()
        {
            HandleAnimations();
        }

        // purpose stmt: Main state machine tick. Delegates to the correct brain based on ShouldUseEmotionalActions().
        public void FSMTick()
        {
            bool useEmotional = ShouldUseEmotionalActions();

            switch (currentState)
            {
                case State.decide:
                    HandleDecideState(useEmotional);
                    break;
                    
                case State.move:
                    HandleMoveState();
                    break;
                    
                case State.execute:
                    HandleExecuteState(useEmotional);
                    break;
                    
                case State.idle:
                    HandleIdleState();
                    break;
            }
        }
        //purpose stmt: deciding what best actions to take based on the current brain, 
        private void HandleDecideState(bool useEmotional)
        {
            Debug.Log($"\n--- {name}: DECIDE STATE ---");
            
            // Add debug info for emotional state
            float timeSinceLastTrigger = Time.time - lastEmotionalTrigger;
            Debug.Log($"{name}: Time since T press: {timeSinceLastTrigger:F1}s, Responded: {hasRespondedToCurrentTrigger}");
            
            // Reset execution flag
            isExecutingAction = false;
            currentAction = null;

            // Decide best action using the appropriate brain
            if (useEmotional && emotionBrain != null)
            {
                Debug.Log($"{name}: Using EmotionBrain (responding to T press)");
                emotionBrain.DecideBestEmotionalAction();
                
                if (emotionBrain.bestAction != null)
                {
                    currentAction = new EmotionalActionWrapper(emotionBrain.bestAction);
                }
            }
            else if (aiBrain != null)
            {
                if (hasRespondedToCurrentTrigger)
                {
                    Debug.Log($"{name}: Using AIBrain (already responded to T press - back to utility AI)");
                }
                else if (timeSinceLastTrigger > emotionalTimeout)
                {
                    Debug.Log($"{name}: Using AIBrain (T press timeout - back to utility AI)");
                }
                else
                {
                    Debug.Log($"{name}: Using AIBrain (no emotional trigger)");
                }
                
                aiBrain.DecideBestAction();
                currentAction = aiBrain.bestAction;
            }

            if (currentAction == null)
            {
                Debug.LogWarning($"{name}: No action decided, staying in decide state");
                return;
            }

            Debug.Log($"{name}: Selected action: {currentAction.Name}");

            // Set required destination for the action
            currentAction.SetRequiredDestination(this);
            Transform targetDestination = currentAction.RequiredDestination;

            if (targetDestination == null)
            {
                Debug.LogError($"{name}: No destination set for action {currentAction.Name}");
                return;
            }

            if (agent == null)
            {
                Debug.LogError($"{name}: NavMeshAgent is null!");
                return;
            }

            // Move towards destination
            agent.SetDestination(targetDestination.position);
            agent.isStopped = false;

            float distance = Vector3.Distance(targetDestination.position, transform.position);

            // Transition to move or execute based on distance
            if (distance < agent.stoppingDistance + 0.5f)
            {
                currentState = State.execute;
                Debug.Log($"{name}: Close to target, going to EXECUTE");
            }
            else
            {
                currentState = State.move;
                Debug.Log($"{name}: Moving to target, going to MOVE");
            }
        }

        // purpose stmt: monitors the navmeshagent movement and transitions to execute when reaches destination
        private void HandleMoveState()
        {
            if (agent != null && !agent.pathPending)
            {
                if (agent.remainingDistance < agent.stoppingDistance + 0.5f)
                {
                    currentState = State.execute;
                    agent.isStopped = true;
                    Debug.Log($"{name}: Reached destination, going to EXECUTE");
                }
            }
        }

        //purpose stmt: executes selected action (emotional or utility) and monitors completion flags to return to decide state
        private void HandleExecuteState(bool useEmotional)
        {
            if (currentAction == null)
            {
                Debug.LogWarning($"{name}: No current action to execute");
                currentState = State.decide;
                return;
            }

            // Only execute once
            if (!isExecutingAction)
            {
                Debug.Log($"{name}: Executing action: {currentAction.Name}");
                isExecutingAction = true;
                
                // Reset completion flags
                if (useEmotional && emotionBrain != null)
                {
                    emotionBrain.finishedExecutingBestAction = false;
                }
                else if (aiBrain != null)
                {
                    aiBrain.finishedExecutingBestAction = false;
                }
                
                // Execute the action
                currentAction.Execute(this);
            }
            
            // Check if action is finished
            bool actionFinished = false;
            if (useEmotional && emotionBrain != null)
            {
                actionFinished = emotionBrain.finishedExecutingBestAction;
            }
            else if (aiBrain != null)
            {
                actionFinished = aiBrain.finishedExecutingBestAction;
            }
            
            if (actionFinished)  
            {
                Debug.Log($"{name}: Action {currentAction.Name} completed");
                
                // Reset states
                isExecutingAction = false;
                currentAction = null;
                if (agent != null) agent.isStopped = false;
                
                // Choose next state based on action type
                if (useEmotional)
                {
                    Debug.Log($"{name}: Emotional action finished - going to IDLE");
                    currentState = State.idle; // Go to idle after emotional action
                }
                else
                {
                    Debug.Log($"{name}: Utility action finished - going to DECIDE");
                    currentState = State.decide; // Go directly to decide for utility actions
                }
            }
        }

        private void HandleIdleState()
        {
            Debug.Log($"{name}: In IDLE state - emotional action complete");
    
            // Check if there's a NEW T press while in idle
            if (ShouldUseEmotionalActions())
            {
                Debug.Log($"{name}: New T press detected while in IDLE - responding immediately");
                currentState = State.decide; // Respond to new T press
            }
            else
            {
                Debug.Log($"{name}: No new T press - returning to utility AI");
                currentState = State.decide; // Return to utility AI
            }
        }
        
        /// <summary>
        /// Determines if emotional actions should be used.
        /// ONLY use emotional actions immediately after T press, then return to utility AI.
        /// </summary>
        /*
        T Press → TriggerEmotionalInteraction() → State.decide (emotional=true)
            ->
        EmotionBrain picks action → State.execute → Emotional action executes
            ->
        Action completes → State.idle → HandleIdleState() checks for new T press
            ->
        If new T press: Another emotional action
        If no T press: Return to utility AI
         */
        private bool ShouldUseEmotionalActions()
        {
            // Check if we've had a recent T press
            float timeSinceLastTrigger = Time.time - lastEmotionalTrigger;
            
            // Don't use emotional actions if:
            // 1. No recent T press (older than timeout)
            // 2. Already responded to current T press
            if (timeSinceLastTrigger > emotionalTimeout || hasRespondedToCurrentTrigger)
            {
                return false; // Return to utility AI
            }

            
            
            // Only use emotional actions if:
            // 1. Recent T press (within timeout)
            // 2. Haven't responded yet
            // 3. Player is nearby
            return IsPlayerNearby();
        }

        public void TriggerEmotionalInteraction()
        {
            if (emotionBrain != null && emotionModel != null)
            {
                Debug.Log($"{name}: Player triggered emotional interaction");
                
                // Update timestamps and flags
                lastEmotionalTrigger = Time.time;
                hasRespondedToCurrentTrigger = false; // Reset for new T press
                
                // Apply a player action to update the emotional model
                PlayerAction[] demoActions = { 
                    PlayerAction.Flirt, 
                    PlayerAction.ComplimentLooks,     
                    PlayerAction.Hug, 
                    PlayerAction.TeasePlayful
                };
                
                PlayerAction selectedAction = demoActions[Random.Range(0, demoActions.Length)];
                // now both perosnality and action affect PAD!
                emotionModel.ApplyPlayerAction(selectedAction, 1.0f);
                
                Debug.Log($"{name}: Applied player action: {selectedAction}");
                Debug.Log($"{name}: New PAD: P={emotionModel.pad.P:F2}, A={emotionModel.pad.A:F2}, D={emotionModel.pad.D:F2}");
                Debug.Log($"{name}: Emotion: {emotionModel.lastEmotion}");
                
                // Go to decide state - let FSM handle the response properly
                currentState = State.decide;
                
                // REMOVED: Manual execution - let the state machine handle it
            }
        }

        public bool IsPlayerNearby()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return false;
            
            float distance = Vector3.Distance(transform.position, player.transform.position);
            
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
                isMoving = agent.velocity.magnitude > 0.05f;
            }

            if (isMoving)
            {
                anim.SetBool("isMoving", true);

                Vector3 velocity = agent.velocity;
                if (velocity.magnitude > 0.01f)
                {
                    lastMove = new Vector2(velocity.x, velocity.z).normalized;
                }

                Vector3 worldSpaceMovement = new Vector3(lastMove.x, 0, lastMove.y);
                Vector3 cameraRelativeMovement = cam.transform.InverseTransformDirection(worldSpaceMovement);

                anim.SetFloat("lastMoveX", cameraRelativeMovement.x);
                anim.SetFloat("lastMoveY", cameraRelativeMovement.z);
            }
            else
            {
                anim.SetBool("isMoving", false);

                Vector3 worldSpaceLastMove = new Vector3(lastMove.x, 0, lastMove.y);
                Vector3 cameraRelativeLastMove = cam.transform.InverseTransformDirection(worldSpaceLastMove);

                anim.SetFloat("lastMoveX", cameraRelativeLastMove.x);
                anim.SetFloat("lastMoveY", cameraRelativeLastMove.z);
            }
        }

        // ========== ACTION METHODS (for regular AI actions) ==========

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
            if (aiBrain != null)
                aiBrain.finishedExecutingBestAction = true;
        }

        public void DoSleep(int duration)
        {
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
            
            if (aiBrain != null)
                aiBrain.finishedExecutingBestAction = true;
        }

        // ========== DEBUG METHODS ==========

        public void LogCurrentState()
        {
            Debug.Log($"{name}: State={currentState}, Action={currentAction?.Name ?? "None"}, Executing={isExecutingAction}");
            
            if (emotionalState != null)
            {
                Debug.Log($"{name}: PAD - P={emotionalState.Pleasure:F2}, A={emotionalState.Arousal:F2}, D={emotionalState.Dominance:F2}");
            }
            
            if (emotionModel != null)
            {
                Debug.Log($"{name}: EmotionModel PAD - P={emotionModel.pad.P:F2}, A={emotionModel.pad.A:F2}, D={emotionModel.pad.D:F2}");
                Debug.Log($"{name}: Last Emotion: {emotionModel.lastEmotion}");
            }
        }
    }

    /// <summary>
    /// IAction interface for unified action handling.
    /// Both regular actions and emotional actions should implement this.
    /// </summary>
    public interface IAction 
    {
        string Name { get; }
        Transform RequiredDestination { get; }
        void SetRequiredDestination(NPCController npc);
        void Execute(NPCController npc);
    }

    /// <summary>
    /// Wrapper to make TL.EmotionalAI.EmotionalAction work with IAction interface.
    /// </summary>
    public class EmotionalActionWrapper : IAction
    {
        private EmotionalAction emotionalAction;
        
        public EmotionalActionWrapper(EmotionalAction action)
        {
            emotionalAction = action;
        }

        public string Name => emotionalAction.Name;
        public Transform RequiredDestination { get; private set; }

        public void SetRequiredDestination(NPCController npc)
        {
            // For emotional actions, typically don't need to move - stay near player
            RequiredDestination = npc.transform;
        }

        public void Execute(NPCController npc)
        {
            // Execute the emotional action using EmotionModel and Animator
            if (npc.emotionModel != null)
            {
                emotionalAction.Execute(npc.emotionModel, npc.anim);
                
                // Mark that we've responded to current T press
                npc.hasRespondedToCurrentTrigger = true;
                
                // Signal completion to EmotionBrain
                if (npc.emotionBrain != null)
                {
                    npc.emotionBrain.finishedExecutingBestAction = true;
                }
                
                Debug.Log($"{npc.name}: Emotional action complete - marked as responded");
            }
        }
    }
}