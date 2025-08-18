using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI
{
    public class EmotionalAIBrain : MonoBehaviour
    {
        public EmotionalAction[] emotionalActions;
        public EmotionalAction bestEmotionalAction { get; set; }
        public bool finishedExecutingBestEmotionalAction { get; set; }

        [Header("Player Tracking")]
        public bool canInteractWithPlayer = true;
        public float playerInteractionRange = 10f;
        private Transform playerTransform;
        private MBTIPersonalityController playerPersonality; // NEW: Track player personality

        [Header("Personality Integration")]
        private MBTIPersonalityController npcPersonality; // NEW: Track NPC's personality

        private void Start()
        {
            finishedExecutingBestEmotionalAction = true;

            // Get NPC's personality controller
            npcPersonality = GetComponent<MBTIPersonalityController>();
            if (npcPersonality != null)
            {
                Debug.Log($"{name}: Found personality controller with type {npcPersonality.MBTIType}");
            }

            // Find player and their personality
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerPersonality = player.GetComponent<MBTIPersonalityController>();

                Debug.Log($"EmotionalAIBrain found player: {player.name}");
                if (playerPersonality != null)
                {
                    Debug.Log($"Player personality type: {playerPersonality.MBTIType}");
                }
            }
            else
            {
                Debug.LogWarning("EmotionalAIBrain: No player found with 'Player' tag!");
            }
        }

        public void DecideBestEmotionalAction(NPCController npc)
        {
            float score = 0f;
            int nextBestActionIndex = 0;
            for (int i = 0; i < emotionalActions.Length; i++)
            {
                if (ScoreEmotionalAction(npc, emotionalActions[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = emotionalActions[i].score;
                }
            }

            bestEmotionalAction = emotionalActions[nextBestActionIndex];
            Debug.Log($"{npc.name}: Best emotional action: {bestEmotionalAction.Name} (score: {bestEmotionalAction.score})");
        }

        public float ScoreEmotionalAction(NPCController npc, EmotionalAction emotionalAction)
        {
            float score = 1f;
            for (int i = 0; i < emotionalAction.considerations.Length; i++)
            {
                float considerationScore = emotionalAction.considerations[i].ScoreConsideration(npc);
                score *= considerationScore;

                if (score == 0)
                {
                    emotionalAction.score = 0;
                    return emotionalAction.score; // no point computing further
                }
            }

            // Apply emotional multipliers based on PAD state
            if (npc.emotionalState != null)
            {
                score = ApplyEmotionalModifiers(score, npc.emotionalState, emotionalAction);
            }

            // NEW: Apply personality-based modifiers
            if (npcPersonality != null)
            {
                score = ApplyPersonalityModifiers(score, emotionalAction);
            }

            // Apply player proximity bonus for social actions
            if (canInteractWithPlayer && playerTransform != null)
            {
                score = ApplyPlayerProximityModifiers(score, npc, emotionalAction);
            }

            emotionalAction.score = score;
            return emotionalAction.score;
        }

        // NEW: Apply personality-based action modifiers
        private float ApplyPersonalityModifiers(float baseScore, EmotionalAction action)
        {
            if (npcPersonality?.PersonalityProfile == null) return baseScore;

            float personalityMultiplier = npcPersonality.GetActionMultiplier(action.actionType);

            // Apply MBTI-based preferences
            if (action.actionType == EmotionalActionType.Social && !npcPersonality.PrefersPlayerInteraction)
            {
                personalityMultiplier *= 0.7f; // Reduce social actions if doesn't prefer player interaction
            }

            if (action.actionType == EmotionalActionType.Assertive && npcPersonality.AvoidsConflict)
            {
                personalityMultiplier *= 0.3f; // Significantly reduce assertive actions for conflict-avoidant types
            }

            if (npcPersonality.showPersonalityDebug)
            {
                Debug.Log($"{npcPersonality.name} ({npcPersonality.MBTIType}): {action.actionType} personality multiplier = {personalityMultiplier:F2}");
            }

            return baseScore * personalityMultiplier;
        }

        private float ApplyEmotionalModifiers(float baseScore, EmotionalState state, EmotionalAction action)
        {
            float modifier = 1f;

            // FIXED: Assuming PAD values are in 0-1 range (not -100 to 100)
            // High pleasure increases social action likelihood
            if (action.actionType == EmotionalActionType.Social)
            {
                modifier += state.Pleasure * 0.5f; // 0-0.5 bonus
            }

            // High arousal increases romantic action likelihood
            if (action.actionType == EmotionalActionType.Romantic)
            {
                modifier += state.Arousal * 0.8f; // 0-0.8 bonus
            }

            // High dominance increases assertive actions
            if (action.actionType == EmotionalActionType.Assertive)
            {
                modifier += state.Dominance * 0.6f; // 0-0.6 bonus
            }

            return baseScore * Mathf.Max(0.1f, modifier); // Ensure positive score
        }

        private float ApplyPlayerProximityModifiers(float baseScore, NPCController npc, EmotionalAction action)
        {
            float distanceToPlayer = Vector3.Distance(npc.transform.position, playerTransform.position);

            // If player is within interaction range, boost social/romantic actions
            if (distanceToPlayer <= playerInteractionRange)
            {
                // NEW: Factor in personality compatibility with player
                float compatibilityBonus = 1f;
                if (npcPersonality != null && playerPersonality != null)
                {
                    float compatibility = npcPersonality.GetCompatibilityWith(playerPersonality);
                    compatibilityBonus = 0.5f + (compatibility * 0.5f); // Range: 0.5 to 1.0

                    if (npcPersonality.showPersonalityDebug)
                    {
                        Debug.Log($"{npc.name}: Personality compatibility with player: {compatibility:F2} (bonus: {compatibilityBonus:F2})");
                    }
                }

                if (action.actionType == EmotionalActionType.Social ||
                    action.actionType == EmotionalActionType.Romantic)
                {
                    float proximityBonus = (playerInteractionRange - distanceToPlayer) / playerInteractionRange;
                    return baseScore * compatibilityBonus * (1f + proximityBonus); // Return modified score
                }
            }

            // Return the original score if no modifications apply
            return baseScore;
        }



// instead of direct access
        public Transform GetPlayerTransform()
        {
            if (playerTransform == null)
            {
                // Try to find the player
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
                else
                {
                    // Fallback: find any object with "Player" in the name
                    GameObject fallbackPlayer = GameObject.Find("Player");
                    if (fallbackPlayer != null)
                    {
                        playerTransform = fallbackPlayer.transform;
                    }
                }
            }

            return playerTransform;
        }
    }
}