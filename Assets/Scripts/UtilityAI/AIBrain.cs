using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UI; 

namespace TL.UtilityAI
{
    public class AIBrain : MonoBehaviour //responsible for the calculations
    {
        public bool finishedDeciding {get; set; }
        public bool finishedExecutingBestAction {get; set;}

        public Action bestAction { get; set; } 
        private NPCController npc;
        
        [SerializeField] private Billboard billBoard;
        [SerializeField] private Action[] actionsAvailable; //populate in inspector, what actions can NPC perform

        // Start is called before the first frame update
        void Start()
        {
            npc = GetComponent<NPCController>();
            finishedDeciding = false;
            finishedExecutingBestAction = false;
            
            // Safety checks during initialization
            if (npc == null)
            {
                Debug.LogError($"{name}: NPCController component not found on this GameObject!");
            }
            
            if (actionsAvailable == null || actionsAvailable.Length == 0)
            {
                Debug.LogError($"{name}: No actions assigned to AIBrain! Add Action assets to 'Actions Available' array in Inspector.");
            }
            else
            {
                Debug.Log($"{name}: AIBrain initialized with {actionsAvailable.Length} actions:");
                for (int i = 0; i < actionsAvailable.Length; i++)
                {
                    if (actionsAvailable[i] != null)
                    {
                        Debug.Log($"  - Action {i}: {actionsAvailable[i].Name}");
                        
                        // Check if action has considerations
                        if (actionsAvailable[i].considerations == null || actionsAvailable[i].considerations.Length == 0)
                        {
                            Debug.LogWarning($"    ⚠️ Action '{actionsAvailable[i].Name}' has NO CONSIDERATIONS! This action will always score 0.");
                            Debug.LogWarning($"    💡 Solution: Create Consideration assets and assign them to this action's Considerations array.");
                        }
                        else
                        {
                            Debug.Log($"    ✅ Action '{actionsAvailable[i].Name}' has {actionsAvailable[i].considerations.Length} considerations");
                            
                            // Check individual considerations
                            for (int j = 0; j < actionsAvailable[i].considerations.Length; j++)
                            {
                                if (actionsAvailable[i].considerations[j] == null)
                                {
                                    Debug.LogError($"      ❌ Consideration {j} is NULL! Remove empty slots from considerations array.");
                                }
                                else
                                {
                                    Debug.Log($"      - Consideration {j}: {actionsAvailable[i].considerations[j].GetType().Name}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"  - Action {i}: NULL ACTION! Remove empty slots from Actions Available array.");
                    }
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            // if (bestAction is null)
            // {
            //     DecideBestAction(npc.actionsAvailable);
            // }
        }

        public void DecideBestAction()
        {
            // SAFETY CHECK 1: Verify actions array exists and has content
            if (actionsAvailable == null || actionsAvailable.Length == 0)
            {
                Debug.LogError($"{name}: Cannot decide best action - No actions available! Add Action assets to 'Actions Available' array in Inspector.");
                return;
            }
            
            // SAFETY CHECK 2: Verify NPC reference exists
            if (npc == null)
            {
                Debug.LogError($"{name}: Cannot decide best action - NPC reference is null! Make sure NPCController component exists on this GameObject.");
                return;
            }
            
            // SAFETY CHECK 3: Verify NPC has required components
            if (npc.stats == null)
            {
                Debug.LogError($"{name}: Cannot decide best action - NPC.stats is null! Make sure NPC has Stats component.");
                return;
            }
            
            // DETAILED ACTION SCORING DEBUG
            Debug.Log($"\n=== {name}: SCORING {actionsAvailable.Length} ACTIONS ===");
            Debug.Log($"Current NPC Stats - Energy: {npc.stats.energy}, Hunger: {npc.stats.hunger}, Money: {npc.stats.money}");
            
            finishedExecutingBestAction = false; // reset variable 
            float score = 0f;
            int nextBestActionIndex = 0;
            bool foundValidAction = false;
            
            for (int i = 0; i < actionsAvailable.Length; i++)
            {
                // SAFETY CHECK 4: Verify individual action exists
                if (actionsAvailable[i] == null)
                {
                    Debug.LogWarning($"❌ Action {i}: NULL! Skipping...");
                    continue;
                }
                
                Debug.Log($"\n--- Scoring Action {i}: '{actionsAvailable[i].Name}' ---");
                
                // Check if action has considerations before scoring
                if (actionsAvailable[i].considerations == null || actionsAvailable[i].considerations.Length == 0)
                {
                    Debug.LogWarning($"❌ Action '{actionsAvailable[i].Name}' has NO CONSIDERATIONS! Score = 0");
                    Debug.LogWarning($"💡 Create and assign Consideration assets to this action!");
                    continue;
                }
                
                float actionScore = ScoreAction(actionsAvailable[i]);
                Debug.Log($"📊 Action '{actionsAvailable[i].Name}' FINAL SCORE: {actionScore:F3}");
                
                if (actionScore > score)
                {
                    nextBestActionIndex = i;
                    score = actionScore;
                    foundValidAction = true;
                    Debug.Log($"🏆 NEW BEST ACTION: '{actionsAvailable[i].Name}' with score {score:F3}");
                }
                else
                {
                    Debug.Log($"📉 Action '{actionsAvailable[i].Name}' (score: {actionScore:F3}) not better than current best ({score:F3})");
                }
            }
            
            // SAFETY CHECK 5: Verify we found a valid action
            if (!foundValidAction)
            {
                Debug.LogError($"\n❌ {name}: NO VALID ACTIONS FOUND! All actions scored 0 or were null.");
                Debug.LogError($"🔧 SOLUTIONS:");
                Debug.LogError($"   1. Make sure your action assets have Considerations assigned");
                Debug.LogError($"   2. Check that your considerations are returning scores > 0");
                Debug.LogError($"   3. Verify NPC stats are set up properly (Energy, Hunger, Money)");
                Debug.LogError($"   4. Create simple test actions with basic considerations");
                return;
            }
            
            // SAFETY CHECK 6: Double-check array bounds before accessing
            if (nextBestActionIndex < 0 || nextBestActionIndex >= actionsAvailable.Length)
            {
                Debug.LogError($"{name}: nextBestActionIndex ({nextBestActionIndex}) is out of bounds for actions array (length: {actionsAvailable.Length})");
                return;
            }
            
            // SAFETY CHECK 7: Verify the selected action isn't null
            if (actionsAvailable[nextBestActionIndex] == null)
            {
                Debug.LogError($"{name}: Selected action at index {nextBestActionIndex} is null!");
                return;
            }

            bestAction = actionsAvailable[nextBestActionIndex];
            Debug.Log($"\n🎯 DECISION COMPLETE: Best action is '{bestAction.Name}' with score {score:F3}");
            
            // SAFETY CHECK 8: Verify action has SetRequiredDestination method
            try
            {
                Debug.Log($" Setting required destination for '{bestAction.Name}'...");
                bestAction.SetRequiredDestination(npc); //identified required destination 
                
                if (bestAction.RequiredDestination != null)
                {
                    Debug.Log($" Required destination set to: {bestAction.RequiredDestination.position}");
                }
                else
                {
                    Debug.LogError($" Action '{bestAction.Name}' failed to set RequiredDestination!");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error setting required destination for '{bestAction.Name}': {e.Message}");
                Debug.LogError($" Check the SetRequiredDestination method in your action script!");
                return;
            }

            finishedDeciding = true;
            
            // SAFETY CHECK 9: Verify billboard exists before updating
            if (billBoard != null)
            {
                billBoard.UpdateBestActionText(bestAction.Name);
                Debug.Log($"📺 Billboard updated with action: {bestAction.Name}");
            }
            else
            {
                Debug.LogWarning($" Billboard is null - cannot update action display. Assign Billboard component in Inspector if needed.");
            }
            
            Debug.Log($"=== {name}: DECISION PROCESS COMPLETE ===\n");
        }
        
        public float ScoreAction(Action action)
        {
            // SAFETY CHECK 1: Verify action exists
            if (action == null)
            {
                Debug.LogError($"Cannot score null action!");
                return 0f;
            }
            
            // SAFETY CHECK 2: Verify action has considerations
            if (action.considerations == null || action.considerations.Length == 0)
            {
                Debug.LogWarning($" Action '{action.Name}' has no considerations! Score = 0");
                Debug.LogWarning($"💡 Solution: Create Consideration assets and assign them to this action!");
                action.score = 0f;
                return 0f;
            }
            
            // SAFETY CHECK 3: Verify NPC exists for consideration scoring
            if (npc == null)
            {
                Debug.LogError($" Cannot score action '{action.Name}' - NPC is null!");
                action.score = 0f;
                return 0f;
            }
            
            Debug.Log($"  🔍 Scoring '{action.Name}' with {action.considerations.Length} considerations:");
            
            float score = 1f;
            int validConsiderations = 0;
            
            for (int i = 0; i < action.considerations.Length; i++)
            {
                // SAFETY CHECK 4: Verify individual consideration exists
                if (action.considerations[i] == null)
                {
                    Debug.LogWarning($"    Consideration {i}: NULL! Skipping...");
                    continue;
                }
                
                float considerationScore = 0f;
                try
                {
                    considerationScore = action.considerations[i].ScoreConsideration(npc);
                    validConsiderations++;
                    Debug.Log($"    Consideration {i} ({action.considerations[i].GetType().Name}): {considerationScore:F3}");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"     Error scoring consideration {i} for action '{action.Name}': {e.Message}");
                    considerationScore = 0f;
                }
                
                score *= considerationScore;

                if (score == 0f) 
                {
                    Debug.Log($"     Action '{action.Name}' eliminated (score became 0 after consideration {i})");
                    action.score = 0f;
                    return 0f; //no point in computing further
                }
            }
            
            // SAFETY CHECK 5: Prevent issues with no valid considerations
            if (validConsiderations == 0)
            {
                Debug.LogWarning($"   Action '{action.Name}' has no valid considerations!");
                action.score = 0f;
                return 0f;
            }
            
            // Average Scheme of overall Score: by Dave Mark pioneer of Utility AI : Behavioral Mathematics of Game AI
            // A lot of considerations means super small! Averaging Scheme to rescale to reasonable number between 0.0 - 1.0. 
            float originalScore = score;
            float modFactor = 1f - (1f / validConsiderations);
            float makeupValue = (1f - originalScore) * modFactor;
            action.score = originalScore + (makeupValue * originalScore);
            
            // SAFETY CHECK 6: Clamp final score to valid range
            action.score = Mathf.Clamp01(action.score);
            
            Debug.Log($"   Raw score: {originalScore:F3} → Final score (after averaging): {action.score:F3}");
            return action.score;
        }
    }
}