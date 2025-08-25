using System.Collections;
using TL.UtilityAI;
using TL.Core;
using UnityEngine;
using TL.EmotionalAI;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Socialize", menuName = "UtilityAI/Actions/Socialize")]
    public class Socialize : Action // Inherit from UtilityAI.Action, not EmotionalAction
    {
        [Header("Socialize Settings")]
        public float socializeDuration = 10f;
        public float playerSocializeRange = 6f; // range to find player
        
        [Header("PAD Adjustments (Inspector Configurable)")]
        public float pleasureChange = 0.08f;
        public float arousalChange = 0.03f;
        public float dominanceChange = 0.04f;

        // IAction interface implementation (inherited from Action base class)
        public override void Execute(NPCController npc)
        {
            Debug.Log($"{npc.name}: Executing Socialize action (score: {score:F2})");
            
            Transform player = FindPlayer(npc);
            
            if (player != null)
            {
                Debug.Log($"{npc.name}: Socializing with Player");
                npc.StartCoroutine(SocializeCoroutine(npc));
            }
            else
            {
                Debug.LogWarning($"{npc.name}: No player found for socializing!");
                FinishAction(npc);
            }
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            Transform player = FindPlayer(npc);
            
            if (player != null)
            {
                RequiredDestination = player;
                Debug.Log($"{npc.name}: Setting socialize destination to Player");
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
            
            if (distance <= playerSocializeRange)
            {
                return player.transform;
            }
            
            return null; // Player too far away
        }

        private IEnumerator SocializeCoroutine(NPCController npc)
        {
            // Mark NPC as socializing
            npc.isSocializing = true;
            
            // Trigger socialize animation if available
            if (npc.anim != null)
            {
                npc.anim.SetTrigger("socialize"); // Assumes you have a "socialize" animation trigger
            }
            
            Debug.Log($"{npc.name}: Starting socialize animation and behavior");
            
            // Wait for socialize duration
            yield return new WaitForSeconds(socializeDuration);
            
            // Apply PAD changes to both emotional systems (if available)
            ApplyPADChanges(npc);
            
            // Reset animation state
            if (npc.anim != null)
            {
                npc.anim.ResetTrigger("socialize");
            }
            
            // Reset state
            npc.isSocializing = false;
            
            // Signal completion to AIBrain (follows NPCController FSM logic)
            FinishAction(npc);
            
            Debug.Log($"{npc.name}: Finished socializing with Player");
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
            
            Debug.Log($"{npc.name}: Socialize action signaled completion to AIBrain");
        }
    }
}