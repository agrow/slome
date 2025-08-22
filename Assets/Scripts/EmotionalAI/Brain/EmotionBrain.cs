using UnityEngine;

namespace TL.EmotionalAI
{
    public class EmotionBrain : MonoBehaviour
    {
        [SerializeField] private EmotionModel emotion;
        [SerializeField] private Animator animator;
        [SerializeField] private EmotionalAction[] actions; // assign in Inspector

        public bool finishedDeciding { get; private set; }
        public bool finishedExecutingBestAction { get; set; }
        public EmotionalAction bestAction { get; private set; }

        public void DecideBestEmotionalAction()
        {
            finishedExecutingBestAction = false;
            if (actions == null || actions.Length == 0) { bestAction = null; finishedDeciding = true; return; }

            float best = float.NegativeInfinity; int idx = 0;
            for (int i=0;i<actions.Length;i++)
            {
                float s = actions[i].ScoreAction(emotion);
                if (s > best) { best = s; idx = i; }
            }
            bestAction = actions[idx];
            finishedDeciding = true;
        }

        public void ExecuteBest()
        {
            if (bestAction == null) return;
            bestAction.Execute(emotion, animator);
            finishedExecutingBestAction = true;
        }
    }
}
