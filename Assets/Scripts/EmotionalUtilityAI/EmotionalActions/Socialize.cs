using System.Collections;
using TL.UtilityAI;
using TL.Core;
using UnityEngine;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Socialize", menuName = "EmotionalUtilityAI/EmotionalActions/Socialize")]
    public class Socialize : EmotionalAction
    {
        [Header("Socialize Duration")]
        public float socializeDuration = 10f;
        
        [Header("PAD Adjustments (Inspector Configurable)")]
        public float pleasureChange = 0.08f;
        public float arousalChange = 0.03f;
        public float dominanceChange = 0.04f;

        public override void Execute(NPCController npc)
        {
            Debug.Log($"{npc.name}: Executing Socialize action (score: {score:F2})");
            
            // Simple execution - considerations already determined this is valid
            npc.StartCoroutine(SocializeCoroutine(npc));
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            // Simple destination - just find player (considerations already validated proximity)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            RequiredDestination = player != null ? player.transform : npc.transform;
        }

        private IEnumerator SocializeCoroutine(NPCController npc)
        {
            // Mark NPC as socializing
            npc.isSocializing = true;
            
            // Wait for socialize duration
            yield return new WaitForSeconds(socializeDuration);
            
            // Apply PAD changes
            ApplyPADChanges(npc);
            
            // Reset state
            npc.isSocializing = false;
            
            // Tell EmotionalAI the action is finished
            FinishAction(npc);
            
            Debug.Log($"{npc.name}: Finished socializing with Player");
        }
        
        private void ApplyPADChanges(NPCController npc)
        {
            if (npc.emotionalState != null)
            {
                npc.emotionalState.AdjustPAD(
                   pleasureChange,
                   arousalChange,
                   dominanceChange
                );
                
                Debug.Log($"{npc.name}: PAD changes applied - Pleasure: {pleasureChange:+0.00;-0.00}, Arousal: {arousalChange:+0.00;-0.00}, Dominance: {dominanceChange:+0.00;-0.00}");
            }
        }

        private void FinishAction(NPCController npc)
        {
            EmotionalAIBrain emotionalBrain = npc.GetComponent<EmotionalAIBrain>();
            if (emotionalBrain != null)
            {
                emotionalBrain.finishedExecutingBestEmotionalAction = true;
            }
        }
    }
}