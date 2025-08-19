using System.Collections;
using TL.UtilityAI;
using TL.Core;
using UnityEngine;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Flirt", menuName = "EmotionalUtilityAI/EmotionalActions/Flirt")]
    public class Flirt : EmotionalAction
    {
        [Header("Flirt Settings")]
        public float flirtDuration = 4f;

        public float playerFlirtRange = 5f; // range to find player
        
        [Header("PAD Adjustments (Inspector Configurable)")]
        public float pleasureChange = 0.1f;
        public float arousalChange = 0.05f;
        public float dominanceChange = 0.02f;

        public override void Execute(NPCController npc)
        {
            Debug.Log($"{npc.name}: Executing Flirt action (score: {score:F2})");
            
            Transform player = FindPlayer(npc);
            
            if (player != null)
            {
                Debug.Log($"{npc.name}: Flirting with Player");
                npc.StartCoroutine(FlirtCoroutine(npc));
            }
            else
            {
                Debug.LogWarning($"{npc.name}: No player found for flirting!");
                FinishAction(npc);
            }
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            Transform player = FindPlayer(npc);
            
            if (player != null)
            {
                RequiredDestination = player;
                Debug.Log($"{npc.name}: Setting flirt destination to Player");
            }
            else
            {
                RequiredDestination = npc.transform; // Stay in place if no player
                Debug.LogWarning($"{npc.name}: No player found, staying in place");
            }
        }

        private Transform FindPlayer(NPCController npc)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return null;

            float distance = Vector3.Distance(npc.transform.position, player.transform.position);
            
            if (distance <= playerFlirtRange)
            {
                return player.transform;
            }
            
            return null; // Player too far away
        }

        private IEnumerator FlirtCoroutine(NPCController npc)
        {
            // Mark NPC as flirting
            npc.isFlirting = true;
            
            // Wait for flirt duration
            yield return new WaitForSeconds(flirtDuration);
            
            // Apply PAD changes
            ApplyPADChanges(npc);
            
            // Reset state
            npc.isFlirting = false;
            
            // Tell EmotionalAI the action is finished
            FinishAction(npc);
            
            Debug.Log($"{npc.name}: Finished flirting with Player");
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