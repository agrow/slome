using System.Collections;
using TL.UtilityAI;
using TL.Core;
using UnityEngine;
using TL.EmotionalAI;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Flirt", menuName = "UtilityAI/Actions/Flirt")]
    public class Flirt : Action // Inherit from UtilityAI.Action, not EmotionalAction
    {
        [Header("Flirt Settings")]
        public float flirtDuration = 4f;
        public float playerFlirtRange = 5f; // range to find player
        
        [Header("PAD Adjustments (Inspector Configurable)")]
        public float pleasureChange = 0.1f;
        public float arousalChange = 0.05f;
        public float dominanceChange = 0.02f;

        // IAction interface implementation (inherited from Action base class)
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
            
            // Trigger flirt animation if available
            if (npc.anim != null)
            {
                npc.anim.SetTrigger("flirt"); // Assumes you have a "flirt" animation trigger
            }
            
            Debug.Log($"{npc.name}: Starting flirt animation and behavior");
            
            // Wait for flirt duration
            yield return new WaitForSeconds(flirtDuration);
            
            // Apply PAD changes to both emotional systems (if available)
            ApplyPADChanges(npc);
            
            // Reset animation state
            if (npc.anim != null)
            {
                npc.anim.ResetTrigger("flirt");
            }
            
            // Reset state
            npc.isFlirting = false;
            
            // Signal completion to AIBrain (follows NPCController FSM logic)
            FinishAction(npc);
            
            Debug.Log($"{npc.name}: Finished flirting with Player");
        }
        
        private void ApplyPADChanges(NPCController npc)
        {
            // Apply to old emotional state system (if available)
            if (npc.emotionalState != null)
            {
                npc.emotionalState.AdjustPAD(
                   pleasureChange,
                   arousalChange,
                   dominanceChange
                );
                
                Debug.Log($"{npc.name}: Applied PAD changes to EmotionalState - P: {pleasureChange:+0.00;-0.00}, A: {arousalChange:+0.00;-0.00}, D: {dominanceChange:+0.00;-0.00}");
            }
            
            // Apply to new EmotionModel system (if available)
            if (npc.emotionModel != null)
            {
                // Store old values for comparison
                float oldP = npc.emotionModel.pad.P;
                float oldA = npc.emotionModel.pad.A;
                float oldD = npc.emotionModel.pad.D;
                EmotionOctant oldEmotion = npc.emotionModel.lastEmotion;
                
                // Apply changes directly to PAD values
                npc.emotionModel.pad.P = Mathf.Clamp01(npc.emotionModel.pad.P + pleasureChange);
                npc.emotionModel.pad.A = Mathf.Clamp01(npc.emotionModel.pad.A + arousalChange);
                npc.emotionModel.pad.D = Mathf.Clamp01(npc.emotionModel.pad.D + dominanceChange);
                
                // Manually reclassify emotion using the static method
                npc.emotionModel.lastEmotion = EmotionClassifier.From(npc.emotionModel.pad);
                
                Debug.Log($"{npc.name}: Applied PAD changes to EmotionModel:");
                Debug.Log($"  Old PAD: P={oldP:F2}, A={oldA:F2}, D={oldD:F2} | Emotion: {oldEmotion}");
                Debug.Log($"  New PAD: P={npc.emotionModel.pad.P:F2}, A={npc.emotionModel.pad.A:F2}, D={npc.emotionModel.pad.D:F2} | Emotion: {npc.emotionModel.lastEmotion}");
            }
        }

        private void FinishAction(NPCController npc)
        {
            // Signal completion to AIBrain (this follows the NPCController FSM logic)
            if (npc.aiBrain != null)
            {
                npc.aiBrain.finishedExecutingBestAction = true;
            }
            
            Debug.Log($"{npc.name}: Flirt action signaled completion to AIBrain");
        }
    }
}