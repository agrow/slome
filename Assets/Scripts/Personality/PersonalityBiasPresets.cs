// Purpose: Per-axis pos/neg bias entries + evaluator that multiplies them into the score.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TL.Personality
{
    /// <summary>
    /// Attach 1–2 of these to an action.
    /// posMultiplier applies for E/N/F/J/A (the "positive" side).
    /// negMultiplier applies for I/S/T/P/Turbulent (the "negative" side).
    /// 1.0f = neutral. Keep values small (e.g., 0.95..1.12).
    /// </summary>
    [Serializable]
    public class PersonalityBiasEntry
    {
        public Axis axis;
        public float posMultiplier = 1.0f; // E/N/F/J/A
        public float negMultiplier = 1.0f; // I/S/T/P/Turbulent
    }

    /// <summary>
    /// Multiplies all relevant axis biases. Clamped so PAD still dominates.
    /// </summary>
    public static class PersonalityEval
    {
        public static float Mul(PersonalityTypeDefinition p, List<PersonalityBiasEntry> entries,
                                float min = 0.80f, float max = 1.30f)
        {
            if (entries == null || entries.Count == 0) return 1f; // no effect
            if (p == null) return 1f;

            float m = 1f;
            for (int i = 0; i < entries.Count; i++)
            {
                var e = entries[i];
                switch (e.axis)
                {
                    case Axis.Energy:   m *= IsPositive(p.energy)    ? e.posMultiplier : e.negMultiplier; break;
                    case Axis.Mind:     m *= IsPositive(p.mind)      ? e.posMultiplier : e.negMultiplier; break;
                    case Axis.Nature:   m *= IsPositive(p.nature)    ? e.posMultiplier : e.negMultiplier; break;
                    case Axis.Tactics:  m *= IsPositive(p.tactics)   ? e.posMultiplier : e.negMultiplier; break;
                    case Axis.Identity: m *= IsPositive(p.identity) ? e.posMultiplier : e.negMultiplier; break;
                }
            }
            return Mathf.Clamp(m, min, max);
        }

        private static bool IsPositive(float value)
        {
            return value >= 0.5f;
        }
    }
}
