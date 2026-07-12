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
        [SerializeField] private PersonalityTypeDefinition npcPersonality; 
        // OR if you keep it elsewhere, expose a PersonalityProfile property you can read.

        public bool finishedDeciding { get; private set; }
        public bool finishedExecutingBestAction { get; set; }
        public EmotionalAction bestAction { get; private set; }

        public void DecideBestEmotionalAction()
        {
            finishedExecutingBestAction = false;
            if (actions == null || actions.Length == 0) { bestAction = null; finishedDeciding = true; return; }

            if (emotion == null)
            {
                Debug.LogWarning($"{name}: EmotionModel is missing. Cannot score emotional actions.");
                bestAction = null;
                finishedDeciding = true;
                return;
            }

            PersonalityTypeDefinition personality = npcPersonality != null ? npcPersonality : emotion.personalityType;

            if (personality != null)
            {
                Debug.Log($"{name}: Personality source = {personality.typeName} | PAD base = ({personality.pleasureBaseline:F2}, {personality.arousalBaseline:F2}, {personality.dominanceBaseline:F2}) | Traits = E:{personality.energy:F2} M:{personality.mind:F2} N:{personality.nature:F2} T:{personality.tactics:F2} I:{personality.identity:F2}");
            }
            else
            {
                Debug.Log($"{name}: Personality source = none (neutral bias)");
            }

            if (personality == null)
            {
                Debug.LogWarning($"{name}: No PersonalityTypeDefinition assigned. Personality bias will be neutral.");
            }

            float best = float.NegativeInfinity; int idx = 0;
            for (int i = 0; i < actions.Length; i++)
            {
                actions[i].EnsurePersonalityBiasesInitialized();
                Debug.Log($"{name}: Scoring action {actions[i].name} with {actions[i].PersonalityBiases.Count} personality bias entries.");
                // 1) Your existing per-action score (PAD Axis × ΔPAD × IntentMatch → curve)
                float curved = actions[i].ScoreAction(emotion);

                // 2) Personality bias (pos/neg per-axis) for THIS action only
                float bias = PersonalityEval.Mul(personality, actions[i].PersonalityBiases); // 0.80..1.30 typical
                Debug.Log($"{name}: Action {actions[i].name} scored utility={curved:F3} personalityBias={bias:F3} final={curved * bias:F3}");
                // 3) Final score = curved × personality
                float s = curved * bias;

                if (s > best) { best = s; idx = i; }
            }
            bestAction = actions[idx];
            Debug.Log($"{name}: Best emotional action is {bestAction.name} with final score {best:F3}.");
            finishedDeciding = true;
        }

        public void ExecuteBest()
        {
            if (bestAction == null) return;
            bestAction.Execute(emotion, animator);
            finishedExecutingBestAction = true;
            Debug.Log($"{name}: Executed best emotional action {bestAction.name}.");    
        }
    }
}
