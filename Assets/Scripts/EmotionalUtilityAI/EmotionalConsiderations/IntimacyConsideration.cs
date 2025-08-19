using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "IntimacyConsideration", menuName = "UtilityAI/Considerations/Intimacy Consideration")]
    public class IntimacyConsideration : Consideration
    {
        [Header("Intimacy Settings")]
        [SerializeField] private AnimationCurve responseCurve;
        
        [Header("Player Proximity")]
        public float playerIntimacyRange = 5f;
        public bool requirePlayerNearby = true;
        
        [Header("Debug")]
        public bool showDebugLogs = true;

        public override float ScoreConsideration(NPCController npc)
        {
            // Check if player is nearby (if required)
            if (requirePlayerNearby)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    if (showDebugLogs)
                        Debug.Log($"IntimacyConsideration: No player found - score = 0");
                    score = 0f;
                    return score;
                }
                
                float distance = Vector3.Distance(npc.transform.position, player.transform.position);
                if (distance > playerIntimacyRange)
                {
                    if (showDebugLogs)
                        Debug.Log($"IntimacyConsideration: Player too far ({distance:F1}m > {playerIntimacyRange}m) - score = 0");
                    score = 0f;
                    return score;
                }
            }
            
            // Get intimacy value (default to 0.5 if null)
            float intimacyValue = 0.5f; // Default fallback
            if (npc.stats != null)
            {
                intimacyValue = npc.stats.intimacy;
            }
            
            // Clamp and evaluate
            float normalizedIntimacy = Mathf.Clamp01(intimacyValue);
            score = responseCurve.Evaluate(normalizedIntimacy);
            
            if (showDebugLogs)
            {
                Debug.Log($"IntimacyConsideration: Intimacy = {intimacyValue:F2}, Normalized = {normalizedIntimacy:F2}, Score = {score:F2}");
            }
            
            return score;
        }
    }
}