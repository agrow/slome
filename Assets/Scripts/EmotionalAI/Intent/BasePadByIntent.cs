using UnityEngine;

// Purpose Statement: baseline PAD delta (ΔP, ΔA, ΔD) for each Intent.
namespace TL.EmotionalAI
{
    public static class BasePadByIntent
    {
        public static Vector3 Get(Intent i) => i switch
        {
            Intent.Affection    => new(+0.20f, +0.05f, -0.05f),
            Intent.Desire       => new(+0.18f, +0.18f, +0.06f),
            Intent.Bonding      => new(+0.15f, +0.10f,  0.00f),
            Intent.Trust        => new(+0.10f, -0.05f, -0.10f),
            Intent.Respect      => new(+0.15f,  0.00f, +0.15f),
            Intent.Playfulness  => new(+0.15f, +0.15f,  0.00f),
            Intent.Security     => new(+0.10f, -0.05f, +0.15f),
            Intent.Conflict     => new(-0.20f, +0.15f, +0.15f),
            Intent.Manipulation => new(-0.15f, +0.05f, -0.10f),
            _ => Vector3.zero
        };
    }
}
