using UnityEngine;

namespace TL.EmotionalAI
{
    /* Purpose Statement:
        scales Δ by Triangle facets 
        (Intimacy→P, Passion→A, Commitment→D) with gains α.
    */
    public static class RelationshipAmplifier
    {
        public static Vector3 Apply(Vector3 d, Triangle t, float aP=0.8f, float aA=0.6f, float aD=0.6f)
        {
            d.x *= (1f + aP * Mathf.Clamp01(t.I));   // Intimacy amplifies Pleasure
            d.y *= (1f + aA * Mathf.Clamp01(t.Pa));  // Passion  amplifies Arousal
            d.z *= (1f + aD * Mathf.Clamp01(t.C));   // Commitment amplifies Dominance

            // per-action safety clamp
            d = Vector3.Max(new(-0.35f,-0.35f,-0.35f), Vector3.Min(new(0.35f,0.35f,0.35f), d));
            return d;
        }
    }
}
