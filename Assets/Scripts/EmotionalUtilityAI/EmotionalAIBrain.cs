using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI
{
    public class EmotionalAIBrain : MonoBehaviour
    {
        [Header("Emotional Actions")]
        public EmotionalAction[] emotionalActions;
        public EmotionalAction bestEmotionalAction { get; set; }
        public bool finishedExecutingBestEmotionalAction { get; set; }

        [Header("Debug Settings")]
        public bool showDebugLogs = true;

        private void Start()
        {
            finishedExecutingBestEmotionalAction = true;
            
            if (showDebugLogs)
            {
                Debug.Log($"{name}: EmotionalAIBrain initialized with {emotionalActions.Length} emotional actions");
            }
        }

        public void DecideBestEmotionalAction(NPCController npc)
        {
            if (emotionalActions == null || emotionalActions.Length == 0)
            {
                Debug.LogError($"{npc.name}: No emotional actions assigned!");
                bestEmotionalAction = null;
                return;
            }

            float bestScore = 0f;
            int bestActionIndex = -1;

            if (showDebugLogs)
            {
                Debug.Log($"\n--- {npc.name}: Deciding Best Emotional Action ---");
            }

            // Score all emotional actions
            for (int i = 0; i < emotionalActions.Length; i++)
            {
                if (emotionalActions[i] == null)
                {
                    Debug.LogWarning($"{npc.name}: Emotional action {i} is null!");
                    continue;
                }

                float actionScore = ScoreEmotionalAction(npc, emotionalActions[i]);

                if (showDebugLogs)
                {
                    Debug.Log($"{npc.name}: Action '{emotionalActions[i].Name}' scored: {actionScore:F3}");
                }

                if (actionScore > bestScore)
                {
                    bestScore = actionScore;
                    bestActionIndex = i;
                }
            }

            // Set the best action
            if (bestActionIndex >= 0 && bestScore > 0f)
            {
                bestEmotionalAction = emotionalActions[bestActionIndex];
                
                if (showDebugLogs)
                {
                    Debug.Log($"{npc.name}: Best emotional action: {bestEmotionalAction.Name} (score: {bestScore:F3})");
                }
            }
            else
            {
                bestEmotionalAction = null;
                
                if (showDebugLogs)
                {
                    Debug.Log($"{npc.name}: No emotional action scored above 0");
                }
            }
        }

        public float ScoreEmotionalAction(NPCController npc, EmotionalAction emotionalAction)
        {
            // Start with base score of 1.0 (same as regular utility AI)
            float score = 1f;
            
            // Check if considerations exist
            if (emotionalAction.considerations == null || emotionalAction.considerations.Length == 0)
            {
                if (showDebugLogs)
                {
                    Debug.LogWarning($"{emotionalAction.Name}: No considerations assigned! Action will score 0.");
                }
                emotionalAction.score = 0;
                return 0;
            }
            
            // Score all considerations (SAME LOGIC AS REGULAR AI)
            for (int i = 0; i < emotionalAction.considerations.Length; i++)
            {
                if (emotionalAction.considerations[i] == null)
                {
                    Debug.LogWarning($"{emotionalAction.Name}: Consideration {i} is null!");
                    continue;
                }
                
                float considerationScore = emotionalAction.considerations[i].ScoreConsideration(npc);
                score *= considerationScore; // Multiply all consideration scores together
                
                if (showDebugLogs)
                {
                    Debug.Log($"  {emotionalAction.Name}: Consideration '{emotionalAction.considerations[i].name}' scored: {considerationScore:F3}");
                }
                
                if (score == 0)
                {
                    if (showDebugLogs)
                    {
                        Debug.Log($"  {emotionalAction.Name}: Action zeroed out by consideration {i}");
                    }
                    emotionalAction.score = 0;
                    return emotionalAction.score;
                }
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"  {emotionalAction.Name}: Base score after considerations: {score:F3}");
            }
            
            // Apply the same averaging scheme as regular utility AI
            if (emotionalAction.considerations.Length > 0)
            {
                float originalScore = score;
                float modFactor = 1 - (1f / emotionalAction.considerations.Length);
                float makeupValue = (1 - originalScore) * modFactor;
                score = originalScore + (makeupValue * originalScore);
                
                if (showDebugLogs)
                {
                    Debug.Log($"  {emotionalAction.Name}: Score after averaging: {score:F3}");
                }
            }
            
            // Store and return the final score
            emotionalAction.score = score;
            return score;
        }

        // Helper method for considerations that need to find the player
        public Transform GetPlayerTransform()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                return player.transform;
            }
            
            // Fallback: find any object with "Player" in the name
            GameObject fallbackPlayer = GameObject.Find("Player");
            if (fallbackPlayer != null)
            {
                return fallbackPlayer.transform;
            }
            
            return null;
        }

        // Helper method for considerations that need player distance
        public float GetDistanceToPlayer(NPCController npc)
        {
            Transform player = GetPlayerTransform();
            if (player != null)
            {
                return Vector3.Distance(npc.transform.position, player.transform.position);
            }
            return float.MaxValue;
        }
    }
}