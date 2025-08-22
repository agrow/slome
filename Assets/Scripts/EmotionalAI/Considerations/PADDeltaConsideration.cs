using UnityEngine;

namespace TL.EmotionalAI
{
    // Purpose Statement: 
    // reads last Δ axis (Pos/Neg via invert), normalized with 0.5 = “no change”.
    [CreateAssetMenu(menuName="EmotionalAI/Considerations/PAD Delta")]
    public class PADDeltaConsideration : EmotionalConsideration
    {
        public enum Axis { DeltaP, DeltaA, DeltaD }
        public Axis axis = Axis.DeltaA;
        public bool invert = false;       // Neg = mirror
        [SerializeField] float range = 0.35f;

        public override float ScoreConsideration(EmotionModel emo)
        {
            float raw = axis switch { Axis.DeltaP => emo.lastDeltaApplied.x, Axis.DeltaA => emo.lastDeltaApplied.y, _ => emo.lastDeltaApplied.z };
            raw = Mathf.Clamp(raw, -range, +range);
            float z = 0.5f + raw / (2f * range); // 0.5 = no change
            if (invert) z = 1f - z;
            score = responseCurve.Evaluate(z);
            return score;
        }
    }
}
