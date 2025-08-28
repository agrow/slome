using UnityEngine;
using TL.Personality; // <-- add this

namespace TL.EmotionalAI
{
    public class EmotionBrain : MonoBehaviour
    {
        [SerializeField] private EmotionModel emotion;
        [SerializeField] private Animator animator;
        [SerializeField] private EmotionalAction[] actions;

        // Where is the personality stored? EITHER:
        [SerializeField] private NPCPersonality npcPersonality; 
        // OR if you keep it elsewhere, expose a PersonalityProfile property you can read.

        public bool finishedDeciding { get; private set; }
        public bool finishedExecutingBestAction { get; set; }
        public EmotionalAction bestAction { get; private set; }

        public void DecideBestEmotionalAction()
        {
            finishedExecutingBestAction = false;
            if (actions == null || actions.Length == 0) { bestAction = null; finishedDeciding = true; return; }

            // Grab the profile once
            var profile = (npcPersonality != null) ? npcPersonality.Profile : default;

            float best = float.NegativeInfinity; int idx = 0;
            for (int i = 0; i < actions.Length; i++)
            {
                // 1) Your existing per-action score (PAD Axis × ΔPAD × IntentMatch → curve)
                float curved = actions[i].ScoreAction(emotion);

                // 2) Personality bias (pos/neg per-axis) for THIS action only
                float bias = PersonalityEval.Mul(profile, actions[i].PersonalityBiases); // 0.80..1.30 typical

                // 3) Final score = curved × personality
                float s = curved * bias;

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
