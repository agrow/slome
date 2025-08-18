using TL.UtilityAI;
using TL.Core;
using UnityEngine;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Flirt", menuName = "EmotionalUtilityAI/EmotionalActions/Flirt")]
    public class Flirt : EmotionalAction
    {
        [Header("Flirt Settings")]
        public bool canFlirtWithPlayer = true;
        public float playerFlirtRange = 5f;
        public float npcFlirtRange = 8f;
        
        [Header("Duration Settings")]
        public float baseDuration = 5f;
        public float minDuration = 3f;
        public float maxDuration = 12f;

        public override void Execute(NPCController npc)
        {
            Debug.Log($"{npc.name}: Executing Flirt action (score: {score:F2})");
            
            // Considerations have already determined this is a good action to take
            // Now just find the best available target and execute
            
            Transform bestTarget = DetermineBestFlirtTarget(npc);
            
            if (bestTarget != null)
            {
                if (bestTarget.CompareTag("Player"))
                {
                    ExecutePlayerFlirt(npc);
                }
                else
                {
                    NPCController targetNPC = bestTarget.GetComponent<NPCController>();
                    if (targetNPC != null)
                    {
                        ExecuteNPCFlirt(npc, targetNPC);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{npc.name}: Flirt action selected but no valid targets found!");
                FinishAction(npc);
            }
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            Transform target = DetermineBestFlirtTarget(npc);
            
            if (target != null)
            {
                RequiredDestination = target;
                Debug.Log($"{npc.name}: Setting flirt destination to {target.name}");
            }
            else
            {
                RequiredDestination = npc.transform; // Stay in place if no target
                Debug.LogWarning($"{npc.name}: No flirt target found, staying in place");
            }
        }

        private Transform DetermineBestFlirtTarget(NPCController npc)
        {
            Transform playerTarget = null;
            NPCController npcTarget = null;
            float playerScore = 0f;
            float npcScore = 0f;

            // Evaluate player as target
            if (canFlirtWithPlayer)
            {
                playerTarget = GetPlayerIfInRange(npc);
                if (playerTarget != null)
                {
                    playerScore = EvaluatePlayerAsTarget(npc);
                }
            }

            // Evaluate NPCs as targets
            npcTarget = FindBestNPCTarget(npc);
            if (npcTarget != null)
            {
                npcScore = EvaluateNPCAsTarget(npc, npcTarget);
            }

            // Return the best target
            if (playerScore > npcScore && playerTarget != null)
            {
                return playerTarget;
            }
            else if (npcTarget != null)
            {
                return npcTarget.transform;
            }

            return null;
        }

        private Transform GetPlayerIfInRange(NPCController npc)
        {
            EmotionalAIBrain brain = npc.GetComponent<EmotionalAIBrain>();
            if (brain == null) return null;

            Transform playerTransform = brain.GetPlayerTransform();
            if (playerTransform == null) return null;

            float distance = Vector3.Distance(npc.transform.position, playerTransform.position);
            return (distance <= playerFlirtRange) ? playerTransform : null;
        }

        private float EvaluatePlayerAsTarget(NPCController npc)
        {
            if (npc.emotionalState == null) return 0.5f;

            // High dominance + pleasure = attracted to player
            float attraction = (npc.emotionalState.Dominance + npc.emotionalState.Pleasure) / 2f;
            
            // Apply personality modifiers
            MBTIPersonalityController personality = npc.GetComponent<MBTIPersonalityController>();
            if (personality != null && personality.PrefersPlayerInteraction)
            {
                attraction *= 1.3f; // 30% bonus for player-preferring personalities
            }

            return Mathf.Clamp01(attraction);
        }

        private NPCController FindBestNPCTarget(NPCController npc)
        {
            NPCController[] allNPCs = Object.FindObjectsByType<NPCController>(FindObjectsSortMode.None);
            NPCController bestTarget = null;
            float bestScore = 0f;

            foreach (NPCController otherNPC in allNPCs)
            {
                if (otherNPC == npc) continue; // Don't flirt with yourself!

                float distance = Vector3.Distance(npc.transform.position, otherNPC.transform.position);
                if (distance <= npcFlirtRange)
                {
                    float targetScore = EvaluateNPCAsTarget(npc, otherNPC);
                    if (targetScore > bestScore)
                    {
                        bestScore = targetScore;
                        bestTarget = otherNPC;
                    }
                }
            }

            return bestTarget;
        }

        private float EvaluateNPCAsTarget(NPCController npc, NPCController target)
        {
            // Calculate compatibility
            float compatibility = CalculatePersonalityCompatibility(npc, target);
            
            // Calculate distance score
            float distance = Vector3.Distance(npc.transform.position, target.transform.position);
            float distanceScore = Mathf.Clamp01(1f - (distance / npcFlirtRange));
            
            // Combine scores (70% compatibility, 30% proximity)
            float finalScore = (compatibility * 0.7f) + (distanceScore * 0.3f);
            
            return finalScore;
        }

        private void ExecutePlayerFlirt(NPCController npc)
        {
            Debug.Log($"{npc.name}: Flirting with Player");
            float duration = CalculateFlirtDuration(npc, null); // null = player
            npc.DoFlirtWithPlayer(duration);
        }

        private void ExecuteNPCFlirt(NPCController npc, NPCController target)
        {
            Debug.Log($"{npc.name}: Flirting with NPC {target.name}");
            float duration = CalculateFlirtDuration(npc, target);
            npc.DoFlirt(target, duration);
        }

        private float CalculateFlirtDuration(NPCController npc, NPCController target)
        {
            float duration = baseDuration;
            
            // Adjust based on NPC's emotional state
            if (npc.emotionalState != null)
            {
                // High dominance = more confident, longer flirting
                duration += npc.emotionalState.Dominance * 2f;
                
                // High arousal = more passionate interaction
                duration += npc.emotionalState.Arousal * 1.5f;
            }
            
            // If flirting with another NPC, factor in compatibility
            if (target != null)
            {
                float compatibility = CalculatePersonalityCompatibility(npc, target);
                duration += compatibility * 3f; // Better compatibility = longer interaction
                
                Debug.Log($"{npc.name} flirting with {target.name}: compatibility {compatibility:F2}, duration {duration:F1}s");
            }
            
            // Apply personality-based duration modifiers
            MBTIPersonalityController personality = npc.GetComponent<MBTIPersonalityController>();
            if (personality != null)
            {
                // Extraverted types flirt longer
                if (personality.EnergyTrait == PersonalityProfile.Energy.Extraverted)
                {
                    duration *= 1.2f;
                }
                
                // Feeling types are more emotionally expressive
                if (personality.NatureTrait == PersonalityProfile.Nature.Feeling)
                {
                    duration *= 1.1f;
                }
            }
            
            return Mathf.Clamp(duration, minDuration, maxDuration);
        }
        
        private float CalculatePersonalityCompatibility(NPCController npc1, NPCController npc2)
        {
            // Try MBTI compatibility first
            MBTIPersonalityController personality1 = npc1.GetComponent<MBTIPersonalityController>();
            MBTIPersonalityController personality2 = npc2.GetComponent<MBTIPersonalityController>();
            
            if (personality1 != null && personality2 != null)
            {
                return personality1.GetCompatibilityWith(personality2);
            }
            
            // Fallback to PAD compatibility
            if (npc1.emotionalState == null || npc2.emotionalState == null) return 0.5f;
            
            float pleasureDiff = 1f - Mathf.Abs(npc1.emotionalState.Pleasure - npc2.emotionalState.Pleasure);
            float arousalDiff = 1f - Mathf.Abs(npc1.emotionalState.Arousal - npc2.emotionalState.Arousal);
            float dominanceDiff = 1f - Mathf.Abs(npc1.emotionalState.Dominance - npc2.emotionalState.Dominance);
            
            return (pleasureDiff + arousalDiff + dominanceDiff) / 3f;
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