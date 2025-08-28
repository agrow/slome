using UnityEngine;

namespace TL.EmotionalAI
{
    // Purpose: baseline triangle delta (ΔIntimacy, ΔPassion, ΔCommitment) for each Intent.
    public static class BaseRelationshipDeltaByIntent
    {
        public static Vector3 Get(Intent i) => i switch
        {
            Intent.Affection    => new(+0.10f, +0.02f, +0.05f),
            Intent.Desire       => new(+0.05f, +0.12f, +0.03f),
            Intent.Bonding      => new(+0.08f, +0.04f, +0.08f),
            Intent.Trust        => new(+0.12f, +0.01f, +0.12f),
            Intent.Respect      => new(+0.03f, +0.01f, +0.07f),
            Intent.Playfulness  => new(+0.04f, +0.07f, +0.02f),
            Intent.Security     => new(+0.02f, +0.00f, +0.12f),
            Intent.Conflict     => new(-0.10f, +0.05f, -0.08f),
            Intent.Manipulation => new(-0.08f, +0.03f, -0.10f),
            _ => Vector3.zero
        };
    }
}