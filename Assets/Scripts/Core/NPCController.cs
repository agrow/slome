using TL.UtilityAI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; // Add this for NavMeshAgent

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
        public float speed = 3f; // NPC movement speed (used for NavMeshAgent)
        
        [Header("Visual Components")]
        public Rigidbody rb;
        public SpriteRenderer sr;
        public Animator anim;
        
        [Header("Navigation")]
        public NavMeshAgent agent; // NavMeshAgent for pathfinding

        // BACKWARD COMPATIBILITY: Create a wrapper for existing action scripts
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
            idle,           // NPC is doing nothing
            move,           // Moving to required destination
            execute,        // Executing the chosen action
            decide          // Deciding what to do next
        }
        
        // Keep animation variables for consistency
        private Vector2 lastMove; // For animation state (same as player)

        public State currentState { get; set; }

        void Start()
        {
            Debug.Log($"=== {name} NPC Starting Initialization ===");
            
            // Set initial state FIRST, before any logging that uses it
            currentState = State.decide;
            
            // Initialize visual components using GetComponent (these are serialized fields)
            if (rb == null) rb = GetComponent<Rigidbody>();
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (anim == null) anim = GetComponent<Animator>();
            
            // Initialize NavMeshAgent
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.speed = speed;
                agent.stoppingDistance = 3f; // Stop 3 units from target
                agent.angularSpeed = 360f; // Rotation speed
                agent.acceleration = 8f; // How quickly it accelerates
                Debug.Log($"{name}: NavMeshAgent initialized - Speed: {agent.speed}, Stopping Distance: {agent.stoppingDistance}");
                
                // NAVMESH DIAGNOSTICS
                Debug.Log($"=== {name} NAVMESH DIAGNOSTICS ===");
                Debug.Log($"Agent is on NavMesh: {agent.isOnNavMesh}");
                Debug.Log($"Agent enabled: {agent.enabled}");
                Debug.Log($"GameObject active: {gameObject.activeInHierarchy}");
                Debug.Log($"Current position: {transform.position}");
    


                // Initialize the mover wrapper for backward compatibility
                _mover = new NavMeshMoverWrapper(agent);
                Debug.Log($"{name}: NavMeshMover wrapper created for action compatibility");
            }
            else
            {
                Debug.LogError($"{name}: CRITICAL - NavMeshAgent component is missing! Add NavMeshAgent component to this GameObject.");
            }
            
            // FORCE component assignment for properties using GetComponent
            aiBrain = GetComponent<AIBrain>();
            Inventory = GetComponent<NPCInventory>();
            stats = GetComponent<Stats>();
            
            // Find Context in scene if not assigned
            if (context == null)
            {
                Context sceneContext = FindAnyObjectByType<Context>();
                if (sceneContext != null)
                {
                    context = sceneContext;
                    Debug.Log($"{name}: Found Context - Home assigned: {context.home != null}");
                }
                else
                {
                    Debug.LogWarning($"{name}: No Context component found in scene! Create GameObject with Context script.");
                }
            }
            
            // Get emotional state from stats if available
            if (stats != null)
            {
                emotionalState = stats.GetEmotionalState();
            }
            
            // Debug component initialization
            Debug.Log($"{name}: AIBrain found: {aiBrain != null}");
            Debug.Log($"{name}: Inventory found: {Inventory != null}");
            Debug.Log($"{name}: Stats found: {stats != null}");
            Debug.Log($"{name}: EmotionalState found: {emotionalState != null}");
            Debug.Log($"{name}: Rigidbody found: {rb != null}");
            Debug.Log($"{name}: SpriteRenderer found: {sr != null}");
            Debug.Log($"{name}: Animator found: {anim != null}");
            Debug.Log($"{name}: NavMeshAgent found: {agent != null}");
            Debug.Log($"{name}: NavMeshMover wrapper: {mover != null}");
            
            // Check if critical components are missing after GetComponent assignment
            if (aiBrain == null)
            {
                Debug.LogError($"{name}: CRITICAL - AIBrain component is missing! Add AIBrain component to this GameObject.");
                Debug.LogError($"{name}: GameObject needs: AIBrain, Stats, NavMeshAgent, and optionally NPCInventory");
                return;
            }
            
            if (stats == null)
            {
                Debug.LogError($"{name}: CRITICAL - Stats component is missing! Add Stats component to this GameObject.");
                Debug.LogError($"{name}: GameObject needs: AIBrain, Stats, NavMeshAgent, and optionally NPCInventory");
                return;
            }
            
            if (agent == null)
            {
                Debug.LogError($"{name}: CRITICAL - NavMeshAgent component is missing! Add NavMeshAgent component to this GameObject.");
                Debug.LogError($"{name}: GameObject needs: AIBrain, Stats, NavMeshAgent, and optionally NPCInventory");
                return;
            }
            
            // Warn about optional components
            if (Inventory == null)
            {
                Debug.LogWarning($"{name}: NPCInventory component not found - this is optional but may be needed for some actions");
            }
            
            // Now log the state (after it's been set)
            Debug.Log($"{name}: Initial state set to: {currentState}");
            Debug.Log($"=== {name} NPC Initialization Complete ===\n");
        }

        void Update()
        {
            FSMTick();
        }

        // Handle animations based on NavMeshAgent movement
        void LateUpdate()
        {
            HandleAnimations();
        }

        public void FSMTick()
        {
            // Double-check that GetComponent worked
            if (aiBrain == null)
            {
                Debug.LogError($"{name}: Cannot tick FSM - AIBrain is null! Make sure AIBrain component is attached to this GameObject.");
                return;
            }

            if (currentState == State.decide)
            {
                Debug.Log($"\n--- {name}: DECIDE STATE ---");
                
                // Safe access to stats using null-conditional operator
                Debug.Log($"{name}: Current stats - Energy: {stats?.energy ?? -1}, Hunger: {stats?.hunger ?? -1}, Money: {stats?.money ?? -1}");
                
                aiBrain.DecideBestAction();
                
                if (aiBrain.bestAction == null)
                {
                    Debug.LogError($"{name}: PROBLEM - No best action found! Check if actions are assigned to AIBrain in Inspector.");
                    Debug.LogError($"{name}: AIBrain needs Action assets in 'Actions Available' array");
                    return;
                }
                
                Debug.Log($"{name}: Best action chosen: {aiBrain.bestAction.Name}");
                Debug.Log($"{name}: Action score: {aiBrain.bestAction.score}");
                
                aiBrain.bestAction.SetRequiredDestination(this);
                
                if (aiBrain.bestAction.RequiredDestination == null)
                {
                    Debug.LogError($"{name}: PROBLEM - No destination set for action {aiBrain.bestAction.Name}!");
                    Debug.LogError($"{name}: Make sure the action's SetRequiredDestination() method sets a valid destination.");
                    Debug.LogError($"{name}: Create destination GameObjects in scene and reference them in actions");
                    return;
                }
                
                // Use NavMeshAgent to navigate to destination
                if (agent != null)
                {
                    Vector3 destination = aiBrain.bestAction.RequiredDestination.position;
                    agent.SetDestination(destination);
                    agent.isStopped = false; // Ensure agent is not stopped
                    
                    Debug.Log($"{name}: NavMeshAgent destination set to: {destination}");
                    
                    float distance = Vector3.Distance(destination, transform.position);
                    Debug.Log($"{name}: Distance to target: {distance:F2}");
                    
                    if (distance < agent.stoppingDistance + 0.5f)
                    {
                        currentState = State.execute;
                        Debug.Log($"{name}: Close enough to destination, switching to EXECUTE state");
                    }
                    else
                    {
                        currentState = State.move;
                        Debug.Log($"{name}: Too far from destination, switching to MOVE state");
                    }
                }
                else
                {
                    Debug.LogError($"{name}: NavMeshAgent is null! Cannot navigate to destination.");
                    return;
                }
            }
            else if (currentState == State.move)
            {
                if (agent != null)
                {
                    float remainingDistance = agent.remainingDistance;
                    bool hasPath = agent.hasPath;
                    bool isMoving = agent.velocity.magnitude > 0.1f;
                    
                    Debug.Log($"{name}: MOVE STATE - Distance remaining: {remainingDistance:F2} | Has path: {hasPath} | Moving: {isMoving}");
                    Debug.Log($"{name}: Current position: {transform.position} | Agent velocity: {agent.velocity}");
                    Debug.Log($"{name}: Path status: {agent.pathStatus} | Is stopped: {agent.isStopped}");
                    
                    // Check if we've reached the destination
                    if (!agent.pathPending && remainingDistance < agent.stoppingDistance + 0.5f)
                    {
                        currentState = State.execute;
                        agent.isStopped = true; // Stop the agent at destination
                        Debug.Log($"{name}: Reached destination! Switching to EXECUTE state");
                    }
                    else if (!hasPath || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                    {
                        Debug.LogWarning($"{name}: Invalid path or no path found! Path status: {agent.pathStatus}");
                        // Try to recalculate path or switch to execute if we're close enough
                        float directDistance = Vector3.Distance(transform.position, aiBrain.bestAction.RequiredDestination.position);
                        if (directDistance < agent.stoppingDistance + 1f)
                        {
                            currentState = State.execute;
                            agent.isStopped = true;
                            Debug.Log($"{name}: Close enough despite path issues, switching to EXECUTE state");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"{name}: NavMeshAgent is null during MOVE state!");
                }
            }
            else if (currentState == State.execute)
            {
                Debug.Log($"{name}: EXECUTE STATE - Action: {aiBrain.bestAction?.Name} | Finished: {aiBrain.finishedExecutingBestAction}");
                
                if (aiBrain.finishedExecutingBestAction == false)
                {
                    Debug.Log($"{name}: Executing action: {aiBrain.bestAction.Name}");
                    aiBrain.bestAction.Execute(this);
                }
                else if (aiBrain.finishedExecutingBestAction == true)
                {
                    // Re-enable NavMeshAgent for next action
                    if (agent != null)
                    {
                        agent.isStopped = false;
                    }
                    
                    currentState = State.decide;
                    Debug.Log($"{name}: Action completed! Switching back to DECIDE state\n");
                }
            }
        }

        // Handle animations based on NavMeshAgent velocity
        private void HandleAnimations()
        {
            if (anim == null || agent == null) return;

            // Get movement from NavMeshAgent velocity
            Vector3 velocity = agent.velocity;
            Vector2 moveInput = new Vector2(velocity.x, velocity.z);
            
            // Same animation logic as before, but using agent velocity
            bool wasMoving = anim.GetBool("isMoving");
            bool isMoving = moveInput.magnitude > 0.1f; // Small threshold to avoid jitter
            
            if (isMoving != wasMoving)
            {
                Debug.Log($"{name}: Animation state changed - Moving: {isMoving}");
            }
            
            if (isMoving)
            {
                // If we are MOVING, save it as our last movement direction
                lastMove = moveInput.normalized;
                anim.SetBool("isMoving", true);
            }
            else
            {
                // If we are NOT MOVING, play the idle animation facing the last direction we were moving
                anim.SetBool("isMoving", false);
            }

            // Set the same animation parameters as player
            anim.SetFloat("lastMoveX", lastMove.x);
            anim.SetFloat("lastMoveY", lastMove.y);
        }

        // Action methods (these use the properties assigned via GetComponent)
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
                Debug.Log($"{name}: Sleep animation started");
            }
            
            yield return new WaitForSeconds(duration);
            
            // Access stats through the property (which now points to real component)
            if (stats != null)
            {
                int oldEnergy = stats.energy;
                stats.energy += 30;
                Debug.Log($"{name}: Finished sleeping - Energy: {oldEnergy} → {stats.energy}");
            }
            
            if (anim != null)
            {
                anim.SetBool("isSleeping", false);
                Debug.Log($"{name}: Sleep animation ended");
            }
            
            aiBrain.finishedExecutingBestAction = true;
            Debug.Log($"{name}: Sleep action marked as completed");
        }

        public void DoWork(int duration)
        {
            Debug.Log($"{name}: DoWork called - Duration: {duration} seconds");
            StartCoroutine(WorkCoroutine(duration));
        }

        private System.Collections.IEnumerator WorkCoroutine(int duration)
        {
            Debug.Log($"{name}: Starting to work for {duration} seconds");
            
            yield return new WaitForSeconds(duration);
            
            // Access stats through the property (which now points to real component)
            if (stats != null)
            {
                int oldMoney = stats.money;
                int oldEnergy = stats.energy;
                stats.money += 50;
                stats.energy -= 20;
                
                Debug.Log($"{name}: Finished working - Money: {oldMoney} → {stats.money}, Energy: {oldEnergy} → {stats.energy}");
            }
            
            aiBrain.finishedExecutingBestAction = true;
            Debug.Log($"{name}: Work action marked as completed");
        }
    }

    // BACKWARD COMPATIBILITY WRAPPER: Allows existing action scripts to work unchanged
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
            get 
            { 
                return null; // Not needed for getter in your actions
            }
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