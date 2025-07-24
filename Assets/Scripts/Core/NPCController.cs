using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;

namespace TL.Core
{   
    //Purpose Statement: 
    public class NPCController : MonoBehaviour
    {
        public MoveController mover { get; set; }
        public AIBrain aiBrain { get; set; }
        public Action[] actionsAvailable; //populate in inspector, what actions can NPC perform

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
        }

        // Update is called once per frame
        void Update()
        {
            if (aiBrain.finishedDeciding)
            {
                aiBrain.finishedDeciding = false;
                aiBrain.bestAction.Execute(this);
            }
        }


        public void OnFinishedAction()
        {
            aiBrain.DecideBestAction(actionsAvailable);
        }

        #region Coroutine
        // Public Methods to Access the Coroutine Here: 
        public void DoWork(int time)
        {
            StartCoroutine(WorkCoroutine(time));
        }

        public void DoSleep(int time)
        {
            StartCoroutine(SleepCoroutine(time));
        }

        // Coroutines Implemented Here:
        IEnumerator WorkCoroutine(int time)
        {
            int counter = time;
            while(counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
            }
            
            Debug.Log("I just harvested 1 resource!");
            // Logic to update things involved with work

            // Decide our new best action after you finished this one...
            OnFinishedAction();

        }

        IEnumerator SleepCoroutine(int time)
        {
            int counter = time;
            while(counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
            }
            
            Debug.Log("I just slept and gained 1 energy!");
            // Logic where updating energy these chnages, for later and implement that logic
            // Decide our new best action after you finished this one...
            OnFinishedAction();

        }

        #endregion
    }
}