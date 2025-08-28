// Purpose: 16P axes + PersonalityProfile for NPCs.
using System;
using UnityEngine;

namespace TL.Personality
{
    public enum Axis { Energy, Mind, Nature, Tactics, Identity }

    [Serializable]
    public struct PersonalityProfile
    {
        public bool Extraverted; // E vs I
        public bool Intuitive;   // N vs S
        public bool Feeling;     // F vs T
        public bool Judging;     // J vs P
        public bool Assertive;   // A vs T (Turbulent)
    }
}
