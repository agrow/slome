/*using TL.UtilityAI;
using TL.EmotionalAI; 
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

        [Header("Emotional AI Components")]
        private EmotionModel emotionModel;
        private EmotionBrain emotionBrain;

        [Header("Movement Settings")]
        public float speed = 3f;
        
        [Header("Visual Components")]
        public Rigidbody rb;
        public SpriteRenderer sr;
        public Animator anim;
        
        [Header("Navigation")]
        public NavMeshAgent agent;
        [SerializeField, Range(1f, 20f)]
        private float interactionRange = 5f;

        [Header("Action States")]
        public bool isFlirting = false;
        public bool isSocializing = false; 
        public bool isWorking = false;
        public bool isExecutingAction = false;
        public bool isInEmotionalInteraction = false;

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
            decide,
            active,
            waitingForPlayerIntent,
            processingPlayerIntent,
            respondingToPlayer
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
            InitializeNavMeshAgent();
            
            // Get regular AI components
            aiBrain = GetComponent<AIBrain>();
            Inventory = GetComponent<NPCInventory>();
            stats = GetComponent<Stats>();
            
            // Initialize EmotionalAI components
            InitializeEmotionalAI();
            
            // Find Context in scene
            InitializeContext();
            
            // Get emotional state
            if (stats != null)
            {
                emotionalState = stats.GetEmotionalState();
            }
            
            // Validate critical components
            ValidateCriticalComponents();
            
            Debug.Log($"{name}: Initial state: {currentState}");
            Debug.Log($"=== {name} NPC Initialization Complete ===\n");
        }

        private void InitializeNavMeshAgent()
        {
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = speed;
                agent.stoppingDistance = 1f;
                agent.angularSpeed = 360f;
                agent.acceleration = 8f;
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
                agent.avoidancePriority = 99;
                
                if (!agent.isOnNavMesh)
                {
                    UnityEngine.AI.NavMeshHit hit;
                    if (UnityEngine.AI.NavMesh.SamplePosition(transform.position, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
                    {
                        transform.position = hit.position;
                    }
                }
                else
                {
                    Debug.Log($"{name}: NavMeshAgent is properly on NavMesh!");
                }

                _mover = new NavMeshMoverWrapper(agent);
            }
            else
            {
                Debug.LogError($"{name}: CRITICAL - NavMeshAgent component missing!");
            }
        }

        private void InitializeEmotionalAI()
        {
            emotionModel = GetComponent<EmotionModel>();
            emotionBrain = GetComponent<EmotionBrain>();
            
            if (emotionModel == null)
            {
                Debug.LogWarning($"{name}: EmotionModel component not found! Adding one...");
                emotionModel = gameObject.AddComponent<EmotionModel>();
            }
            
            if (emotionBrain == null)
            {
                Debug.LogWarning($"{name}: EmotionBrain component not found! Adding one...");
                emotionBrain = gameObject.AddComponent<EmotionBrain>();
            }
            
            Debug.Log($"{name}: EmotionalAI components initialized");
        }

        private void InitializeContext()
        {
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
        }

        private void ValidateCriticalComponents()
        {
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
        }

        void Update()
        {
            FSMTick();
            
            // Handle player emotional interaction input
            if (Input.GetKeyDown(KeyCode.T) && IsPlayerNearby() && !isInEmotionalInteraction)
            {
                StartEmotionalInteraction();
            }
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

            // Handle emotional interaction states (new EmotionalAI system)
            if (HandleEmotionalStates()) return;

            // Handle regular autonomous behavior states (using regular AIBrain)
            HandleAutonomousStates();
        }

        private bool HandleEmotionalStates()
        {
            if (currentState == State.waitingForPlayerIntent)
            {
                if (!isExecutingAction)
                {
                    isExecutingAction = true;
                    StartCoroutine(AutoSelectPlayerActionForDemo());
                }
                return true;
            }
            
            if (currentState == State.processingPlayerIntent)
            {
                // EmotionalAI pipeline is running, wait for completion
                return true;
            }
            
            if (currentState == State.respondingToPlayer)
            {
                // EmotionBrain is executing response, wait for completion
                return true;
            }

            if (currentState == State.active)
            {
                // Emotional AI is active
                if (!isInEmotionalInteraction)
                {
                    currentState = State.decide;
                }
                return true;
            }

            return false; // Not in emotional state
        }

        private void HandleAutonomousStates()
        {
            if (currentState == State.decide)
            {
                Debug.Log($"\n--- {name}: DECIDE STATE ---");
                Debug.Log($"{name}: Stats - Energy: {stats?.energy ?? -1}, Hunger: {stats?.hunger ?? -1}, Money: {stats?.money ?? -1}");
                
                // Use regular AIBrain for autonomous behavior
                aiBrain.DecideBestAction();
                
                if (aiBrain.bestAction == null)
                {
                    Debug.LogError($"{name}: No best action found!");
                    return;
                }
                
                Debug.Log($"{name}: Best action: {aiBrain.bestAction.Name} (score: {aiBrain.bestAction.score})");
                
                // Set destination for regular action
                Transform targetDestination = aiBrain.bestAction.RequiredDestination;
                
                if (targetDestination == null)
                {
                    Debug.LogError($"{name}: No destination set for action {aiBrain.bestAction.Name}!");
                    return;
                }
                
                MoveToDestination(targetDestination.position);
            }
            else if (currentState == State.move)
            {
                HandleMovementState();
            }
            else if (currentState == State.execute)
            {
                HandleExecutionState();
            }
        }

        private void MoveToDestination(Vector3 destination)
        {
            if (agent != null)
            {
                agent.SetDestination(destination);
                agent.isStopped = false;
                
                float distance = Vector3.Distance(destination, transform.position);
                
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

        private void HandleMovementState()
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

        private void HandleExecutionState()
        {
            // Handle regular action execution - ONLY execute once
            if (!isExecutingAction)
            {
                Debug.Log($"{name}: Starting regular action: {aiBrain.bestAction.Name}");
                isExecutingAction = true;
                aiBrain.finishedExecutingBestAction = false;
                aiBrain.bestAction.Execute(this);
            }
            
            // Wait for regular action coroutine to signal completion
            if (aiBrain.finishedExecutingBestAction)
            {
                if (agent != null)
                {
                    agent.isStopped = false;
                }
                
                // Reset state
                isExecutingAction = false;
                aiBrain.finishedExecutingBestAction = false;
                
                currentState = State.decide;
                Debug.Log($"{name}: Finished executing regular action, returning to DECIDE");
            }
        }

        // ========== EMOTIONAL INTERACTION METHODS (NEW EMOTIONALAI SYSTEM) ==========

        private void StartEmotionalInteraction()
        {
            Debug.Log($"{name}: Starting emotional interaction with player");
            
            isInEmotionalInteraction = true;
            currentState = State.waitingForPlayerIntent;
            
            // Stop current movement
            if (agent != null)
            {
                agent.isStopped = true;
            }
        }

        private IEnumerator AutoSelectPlayerActionForDemo()
        {
            yield return new WaitForSeconds(1f);
            
            // Demo: randomly select a player action
            PlayerAction[] demoActions = { 
                PlayerAction.Flirt, 
                PlayerAction.ComplimentLooks, 
                PlayerAction.Hug,
                PlayerAction.TeasePlayful
            };
            
            PlayerAction selectedAction = demoActions[Random.Range(0, demoActions.Length)];
            
            Debug.Log($"{name}: Demo auto-selected player action: {selectedAction}");
            ProcessPlayerAction(selectedAction);
        }

        public void ProcessPlayerAction(PlayerAction playerAction)
        {
            if (emotionModel == null || emotionBrain == null)
            {
                Debug.LogError($"{name}: Cannot process player action - EmotionalAI components missing!");
                EndEmotionalInteraction();
                return;
            }

            Debug.Log($"{name}: Processing player action: {playerAction}");
            currentState = State.processingPlayerIntent;
            
            StartCoroutine(ProcessPlayerActionCoroutine(playerAction));
        }

        private IEnumerator ProcessPlayerActionCoroutine(PlayerAction playerAction)
        {
            // STEP 1-5: Run the EmotionalAI pipeline using EmotionModel
            emotionModel.ApplyPlayerAction(playerAction, 1.0f);
            
            Debug.Log($"{name}: EmotionalAI Pipeline Complete:");
            Debug.Log($"  Intent: {emotionModel.lastIntent}");
            Debug.Log($"  PAD: P={emotionModel.pad.P:F2}, A={emotionModel.pad.A:F2}, D={emotionModel.pad.D:F2}");
            Debug.Log($"  Emotion: {emotionModel.lastEmotion}");
            Debug.Log($"  Delta: {emotionModel.lastDeltaApplied}");
            
            yield return new WaitForSeconds(0.5f); // Brief processing delay
            
            // STEP 6-9: Run the EmotionBrain decision and execution
            currentState = State.respondingToPlayer;
            
            // Use the NEW EmotionBrain (not the old EmotionalAIBrain)
            emotionBrain.DecideBestEmotionalAction();
            
            if (emotionBrain.bestAction != null)
            {
                Debug.Log($"{name}: Executing emotional response: {emotionBrain.bestAction.Name}");
                
                // Execute using the new EmotionalAI system
                emotionBrain.ExecuteBest();
            }
            else
            {
                Debug.Log($"{name}: No emotional action selected, using neutral response");
                TriggerNeutralResponse();
            }
            
            yield return new WaitForSeconds(2f); // Response duration
            
            EndEmotionalInteraction();
        }

        private void TriggerNeutralResponse()
        {
            if (anim != null)
            {
                anim.SetTrigger("neutralResponse");
            }
            Debug.Log($"{name}: Giving neutral response to player");
        }

        private void EndEmotionalInteraction()
        {
            Debug.Log($"{name}: Ending emotional interaction");
            
            isInEmotionalInteraction = false;
            isExecutingAction = false;
            currentState = State.decide;
            
            // Resume normal movement
            if (agent != null)
            {
                agent.isStopped = false;
            }
        }

        // ========== UTILITY METHODS ==========

        private bool IsPlayerNearby()
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
            */