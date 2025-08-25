/*

using TL.UtilityAI;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;


namespace TL.Core
{
    /// <summary>
    /// New NPCController implementation that clearly separates handling of EmotionalAIBrain and AIBrain.
    /// Uses IAction interface for unified action handling.
    /// </summary>
    public class NPCController : MonoBehaviour
    {
        // AI brains
        public AIBrain aiBrain { get; private set; }
        public EmotionalAIBrain emotionalBrain { get; private set; }

        // Other components
        public NPCInventory Inventory { get; private set; }
        public Stats stats { get; private set; }
        public Context context { get; private set; }
        public EmotionalState emotionalState { get; private set; }
        public NavMeshAgent agent { get; private set; }
        public Animator anim { get; private set; }

        // State machine
        public enum State { idle, move, execute, decide, active }
        public State currentState { get; private set; }

        private IAction currentAction;

        void Start()
        {
            // Initialize components
            aiBrain = GetComponent<AIBrain>();
            emotionalBrain = GetComponent<EmotionalAIBrain>();
            Inventory = GetComponent<NPCInventory>();
            stats = GetComponent<Stats>();
            context = FindAnyObjectByType<Context>();
            emotionalState = GetComponent<EmotionalState>();
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            currentState = State.decide;
        }

        void Update()
        {
            FSMTick();
        }

        /// <summary>
        /// Main state machine tick. Delegates to the correct brain based on ShouldUseEmotionalActions().
        /// </summary>
        public void FSMTick()
        {
            bool useEmotional = ShouldUseEmotionalActions() && emotionalBrain != null;

            // DECIDE
            if (currentState == State.decide)
            {
                // Decide best action using the appropriate brain
                if (useEmotional)
                {
                    currentAction = emotionalBrain.DecideBestAction();
                }
                else if (aiBrain != null)
                {
                    currentAction = aiBrain.DecideBestAction();
                }

                if (currentAction == null)
                    return;

                // Set required destination for the action
                currentAction.SetRequiredDestination(this);
                Transform targetDestination = currentAction.RequiredDestination;

                if (targetDestination == null || agent == null)
                    return;

                agent.SetDestination(targetDestination.position);
                agent.isStopped = false;

                float distance = Vector3.Distance(targetDestination.position, transform.position);

                // Transition to move or execute based on distance
                if (distance < agent.stoppingDistance + 0.5f)
                    // if we don't need to move, go straight to execute
                    currentState = State.execute;
                else
                    // if we need to move, go to move state
                    currentState = State.move;
            }
            // MOVE
            else if (currentState == State.move)
            {
                if (agent != null && !agent.pathPending && agent.remainingDistance < agent.stoppingDistance + 0.5f)
                {
                    currentState = State.execute;
                    agent.isStopped = true;
                }
            }
            // EXECUTE
            else if (currentState == State.execute)
            {
                if (useEmotional && emotionalBrain != null)
                {
                    emotionalBrain.ExecuteAction(currentAction, this);
                }
                else if (aiBrain != null)
                {
                    aiBrain.ExecuteAction(currentAction, this);
                }
                // After execution, transition to next state as needed
                // ...
            }
        }

        /// <summary>
        /// Determines if emotional actions should be used (stub for demo).
        /// </summary>
        private bool ShouldUseEmotionalActions()
        {
            // Example: Use emotional actions if player is nearby and a key is pressed
            // Replace with your own logic
            return false;
        }
    }

    /// <summary>
    /// Example IAction interface for unified action handling.
    /// </summary>
    public interface IAction
    {
        string Name { get; }
        Transform RequiredDestination { get; }
        void SetRequiredDestination(NPCController npc);
        void Execute(NPCController npc);
    }

    /// <summary>
    /// Example stub classes for brains. Implement these in your project.
    /// </summary>
    public class AIBrain : MonoBehaviour
    {
        public IAction DecideBestAction() { return null; }
        public void ExecuteAction(IAction action, NPCController npc) { }
    }

    public class EmotionalAIBrain : MonoBehaviour
    {
        public IAction DecideBestAction() { return null; }
        public void ExecuteAction(IAction action, NPCController npc) { }
    }
}
*/