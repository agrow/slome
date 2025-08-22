using UnityEngine;

namespace TL.EmotionalAI
{
    // 
    [CreateAssetMenu(menuName="EmotionalAI/Considerations/Intent Match (Nudge)")]
    public class IntentMatchConsideration : EmotionalConsideration
    {
        public Intent desired = Intent.Desire;
        [Range(0f,1f)] public float onMatch = 1f;
        [Range(0f,1f)] public float onMismatch = 0.2f;

        public override float ScoreConsideration(EmotionModel emo)
        {
            float x = (emo.lastIntent == desired) ? onMatch : onMismatch;
            score = responseCurve.Evaluate(x);
            return score;
        }
    }
}
