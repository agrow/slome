using UnityEngine;

namespace TL.EmotionalAI
{
    // Purpose Statement: reads current PAD axis (High/Low via invert).
    // What does this code really do? Seems like it takes in the emotionalconsideration class which has a response curve attached to it
    // Emotional Consideration Object that inherits the Response Curve
    // Need a clear picture how to make these considerations
    [CreateAssetMenu(menuName="EmotionalAI/Considerations/PAD Axis")]
    public class PADAxisConsideration : EmotionalConsideration
    {
        public enum Axis { Pleasure, Arousal, Dominance }
        public Axis axis = Axis.Pleasure;
        public bool invert = false; // Low = invert

        public override float ScoreConsideration(EmotionModel emo)
        {
            float x = axis switch { Axis.Pleasure => emo.pad.P, Axis.Arousal => emo.pad.A, _ => emo.pad.D };
            if (invert) x = 1f - x;
            score = responseCurve.Evaluate(Mathf.Clamp01(x));
            return score;
        }
    }
}
