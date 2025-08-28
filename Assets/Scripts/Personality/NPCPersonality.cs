// Purpose: Minimal component that holds an NPC's PersonalityProfile so other systems can read it.
using UnityEngine;

namespace TL.Personality
{
    [DisallowMultipleComponent]
    public sealed class NPCPersonality : MonoBehaviour
    {
        [Header("NPC Personality Profile")]
        public PersonalityProfile Profile;
    }
}
