
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
        }

        // Update is called once per frame
        void Update()
        {
            // if (bestAction is null)
            // {
            //     DecideBestAction(npc.actionsAvailable);
            // }
        }
        // Purpose Statement: Picks out the best action, use a better data structure with faster lookup time majority queue?
        // Would there be an overhead with maintaining a majority queue vs. just score and iterating?
        // Doesn't seem efficent... for a real-time simulation... 
        // maybe use processes

        public void DecideBestAction()
        {
            finishedExecutingBestAction = false; // reset variable 
           
            float score = 0f;
            int nextBestActionIndex = 0;
            for (int i = 0; i < actionsAvailable.Length; i++)
            {
                if (ScoreAction(actionsAvailable[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = actionsAvailable[i].score;
                }
            }

            bestAction = actionsAvailable[nextBestActionIndex];
            bestAction.SetRequiredDestination(npc); //identifieid required destination 

            finishedDeciding = true;
            billBoard.UpdateBestActionText(bestAction.Name);
        }
        // Purpose Statement: Loop through all the considerations of the action
        // Score all the considerations
        // Average the consideration scores ==> overall action score 
        public float ScoreAction(Action action)
        {
            float score = 1f;
            for (int i = 0; i < action.considerations.Length; i++)
            {
                float considerationScore = action.considerations[i].ScoreConsideration(npc);
                score *= considerationScore;

                if (score == 0) 
                {
                    action.score = 0;
                    return action.score; //no point in computing further
                }
            }
            // Average Scheme of overall Score: by Dave Mark pioneer of Utility AI : Behavioral Mathmetatics of Game AI
            // Alot of considerations means super small! Averaging Scheme to rescale to resonable number between 0.0 - 1.0. 
            float originalScore = score;
            float modFactor = 1 - (1 / action.considerations.Length);
            float makeupValue = (1 - originalScore) * modFactor;
            action.score = originalScore + (makeupValue * originalScore);

            return action.score;

        }


    }
}
