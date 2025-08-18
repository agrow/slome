using TL.UtilityAI;
using TL.Core;
using UnityEngine;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Socialize", menuName = "EmotionalUtilityAI/EmotionalActions/Socialize")]
    public class Socialize : EmotionalAction
    {
        [Header("Socialize Settings")]
        public bool canSocializeWithPlayer = true;
        public float playerSocializeRange = 8f;
        public float npcSocializeRange = 10f;
        
        [Header("Duration Settings")]
        public float baseDuration = 10f;
        public float minDuration = 5f;
        public float maxDuration = 20f;
        
        [Header("Social Preferences")]
        public bool preferGroupSocialization = false;

        public override void Execute(NPCController npc)
        {
            Debug.Log($"{npc.name}: Executing Socialize action (score: {score:F2})");
            
            // Considerations have already determined this is a good action to take
            // Now just find the best available target and execute
            
            Transform bestTarget = DetermineBestSocialTarget(npc);
            
            if (bestTarget != null)
            {
                if (bestTarget.CompareTag("Player"))
                {
                    ExecutePlayerSocialize(npc);
                }
                else
                {
                    NPCController targetNPC = bestTarget.GetComponent<NPCController>();
                    if (targetNPC != null)
                    {
                        ExecuteNPCSocialize(npc, targetNPC);
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{npc.name}: Socialize action selected but no valid targets found!");
                FinishAction(npc);
            }
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            Transform target = DetermineBestSocialTarget(npc);
            
            if (target != null)
            {
                RequiredDestination = target;
                Debug.Log($"{npc.name}: Setting socialize destination to {target.name}");
            }
            else
            {
                RequiredDestination = npc.transform; // Stay in place if no target
                Debug.LogWarning($"{npc.name}: No social target found, staying in place");
            }
        }

        private Transform DetermineBestSocialTarget(NPCController npc)
        {
            Transform playerTarget = null;
            NPCController npcTarget = null;
            float playerScore = 0f;
            float npcScore = 0f;

            // Evaluate player as target
            if (canSocializeWithPlayer)
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
            return (distance <= playerSocializeRange) ? playerTransform : null;
        }

        private float EvaluatePlayerAsTarget(NPCController npc)
        {
            if (npc.emotionalState == null) return 0.5f;

            // High pleasure + moderate dominance = social desire
            float socialDesire = (npc.emotionalState.Pleasure * 0.6f) + (npc.emotionalState.Dominance * 0.4f);
            
            // Apply personality modifiers
            MBTIPersonalityController personality = npc.GetComponent<MBTIPersonalityController>();
            if (personality != null && personality.PrefersPlayerInteraction)
            {
                socialDesire *= 1.5f; // 50% bonus for player-preferring personalities
            }

            Debug.Log($"{npc.name}: Social desire towards player: {socialDesire:F2}");
            return Mathf.Clamp01(socialDesire);
        }

        private NPCController FindBestNPCTarget(NPCController npc)
        {
            NPCController[] allNPCs = Object.FindObjectsByType<NPCController>(FindObjectsSortMode.None);
            NPCController bestTarget = null;
            float bestScore = 0f;

            foreach (NPCController otherNPC in allNPCs)
            {
                if (otherNPC == npc) continue; // Don't socialize with yourself!

                float distance = Vector3.Distance(npc.transform.position, otherNPC.transform.position);
                if (distance <= npcSocializeRange)
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
            // Calculate social compatibility
            float compatibility = CalculateSocialCompatibility(npc, target);
            
            // Calculate distance score
            float distance = Vector3.Distance(npc.transform.position, target.transform.position);
            float distanceScore = Mathf.Clamp01(1f - (distance / npcSocializeRange));
            
            // Calculate target's sociability
            float targetSociability = 0.5f; // Default
            if (target.emotionalState != null)
            {
                targetSociability = (target.emotionalState.Pleasure + target.emotionalState.Dominance) / 2f;
            }
            
            // Combine scores: compatibility (50%) + proximity (25%) + target sociability (25%)
            float finalScore = (compatibility * 0.5f) + (distanceScore * 0.25f) + (targetSociability * 0.25f);
            
            return finalScore;
        }

        private void ExecutePlayerSocialize(NPCController npc)
        {
            Debug.Log($"{npc.name}: Socializing with Player");
            float duration = CalculateSocializeDuration(npc, null); // null = player
            npc.DoSocializeWithPlayer(duration);
        }

        private void ExecuteNPCSocialize(NPCController npc, NPCController target)
        {
            Debug.Log($"{npc.name}: Socializing with NPC {target.name}");
            float duration = CalculateSocializeDuration(npc, target);
            npc.DoSocialize(target, duration);
        }

        private float CalculateSocializeDuration(NPCController npc, NPCController target)
        {
            float duration = baseDuration;
            
            // Adjust based on NPC's emotional state
            if (npc.emotionalState != null)
            {
                // High dominance = more confident, longer socializing
                duration += npc.emotionalState.Dominance * 5f; // 0-5 seconds
                
                // High pleasure = enjoys socializing more
                duration += npc.emotionalState.Pleasure * 3f; // 0-3 seconds
                
                // Calm NPCs (low arousal) have longer, deeper conversations
                duration += (1f - npc.emotionalState.Arousal) * 2f; // 0-2 seconds
            }
            
            // If socializing with another NPC, factor in their personality and compatibility
            if (target != null)
            {
                // Mutual extroversion bonus
                if (target.emotionalState != null)
                {
                    float mutualExtroversion = target.emotionalState.Dominance * 3f;
                    duration += mutualExtroversion;
                }
                
                // Compatibility affects duration significantly
                float compatibility = CalculateSocialCompatibility(npc, target);
                duration += compatibility * 4f; // 0-4 seconds bonus
                
                Debug.Log($"{npc.name} socializing with {target.name}: compatibility {compatibility:F2}, duration {duration:F1}s");
            }
            
            // Apply personality-based duration modifiers
            MBTIPersonalityController personality = npc.GetComponent<MBTIPersonalityController>();
            if (personality != null)
            {
                // Extraverted types socialize longer
                if (personality.EnergyTrait == PersonalityProfile.Energy.Extraverted)
                {
                    duration *= 1.3f;
                }
                
                // Feeling types enjoy deeper social connections
                if (personality.NatureTrait == PersonalityProfile.Nature.Feeling)
                {
                    duration *= 1.2f;
                }
                
                // Prospecting types are more spontaneous and varied in conversation
                if (personality.TacticsTrait == PersonalityProfile.Tactics.Prospecting)
                {
                    duration *= 1.1f;
                }
            }
            
            return Mathf.Clamp(duration, minDuration, maxDuration);
        }
        
        private float CalculateSocialCompatibility(NPCController npc1, NPCController npc2)
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
            
            // For socializing, we want similar pleasure and arousal levels
            // But complementary dominance can work well (leader/follower dynamic)
            float pleasureSync = 1f - Mathf.Abs(npc1.emotionalState.Pleasure - npc2.emotionalState.Pleasure);
            float arousalSync = 1f - Mathf.Abs(npc1.emotionalState.Arousal - npc2.emotionalState.Arousal);
            
            // Dominance can be either similar or complementary
            float dominanceDiff = Mathf.Abs(npc1.emotionalState.Dominance - npc2.emotionalState.Dominance);
            float dominanceCompat = Mathf.Max(
                1f - dominanceDiff,  // Similar dominance levels
                dominanceDiff        // Complementary (one leader, one follower)
            );
            
            float compatibility = (pleasureSync * 0.4f) + (arousalSync * 0.3f) + (dominanceCompat * 0.3f);
            
            Debug.Log($"{npc1.name} <-> {npc2.name} social compatibility: {compatibility:F2}");
            return compatibility;
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