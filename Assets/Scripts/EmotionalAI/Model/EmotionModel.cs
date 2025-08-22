using UnityEngine;

namespace TL.EmotionalAI
{
    /* Purpose Statement: owns PAD/Triangle state and runs the pipeline:
        ApplyPlayerAction(playerAction, intensity) → intent → base Δ → amp → update PAD → classify.
        Also exposes lastIntent, lastDeltaApplied, lastEmotion for the scoring layer.
    */
    public class EmotionModel : MonoBehaviour
    {
        [Header("PAD (0..1)")] public PAD pad = new PAD{ P=0.5f, A=0.5f, D=0.5f };
        [Header("Love Triangle (0..1)")] public Triangle tri = new Triangle{ I=0.4f, Pa=0.4f, C=0.4f };

        [Header("Diagnostics")]
        public Intent lastIntent;
        public Vector3 lastDeltaApplied;
        public EmotionOctant lastEmotion;

        public void ApplyPlayerAction(PlayerAction act, float intensity01 = 0.7f)
        {
            lastIntent = IntentMapper.Map(act); // Player Action => Intent 
            // maybe print it here to testing

            // optional intensity scaling (feel knob)
            Vector3 d0 = BasePadByIntent.Get(lastIntent) * Mathf.Lerp(0.6f, 1.4f, Mathf.Clamp01(intensity01)); // itensity later on correlated to personality? 
            Vector3 d1 = RelationshipAmplifier.Apply(d0, tri);

            // Adjust PAD
            pad.P = Mathf.Clamp01(pad.P + d1.x);
            pad.A = Mathf.Clamp01(pad.A + d1.y);
            pad.D = Mathf.Clamp01(pad.D + d1.z);

            lastDeltaApplied = d1; //Keep track of last change
            lastEmotion = EmotionClassifier.From(pad); //Keep track of Emotion
        }
    }
}
