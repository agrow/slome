using UnityEngine;

/// <summary>
/// Represents the emotional state of a character using the PAD model and love components.
/// </summary>
public class EmotionalState: MonoBehaviour
{
    // PAD Model values (0 to 1)
    public float Pleasure { get; set; }
    public float Arousal { get; set; }
    public float Dominance { get; set; }

    // Love components (0 to 1)
    public float Intimacy { get; set; }
    public float Passion { get; set; }
    public float Commitment { get; set; }

    // Previous love values for delta calculations
    public float LastIntimacy { get; set; }
    public float LastPassion { get; set; }
    public float LastCommitment { get; set; }

    // Optional: reference to personality data (can be null)
    private PersonalityTypeDefinition personality;

    /// <summary>
    /// Default constructor for testing without personality input.
    
    public EmotionalState()
    {
        Pleasure = 0.5f;
        Arousal = 0.5f;
        Dominance = 0.5f;

        Intimacy = Passion = Commitment = 0f;
        LastIntimacy = LastPassion = LastCommitment = 0f;
    }

    /// <summary>
    /// Constructor using personality definition.
    /// </summary>
    public EmotionalState(PersonalityTypeDefinition personality)
    {
        this.personality = personality;

        Pleasure = personality.pleasureBaseline;
        Arousal = personality.arousalBaseline;
        Dominance = personality.dominanceBaseline;

        Intimacy = Passion = Commitment = 0f;
        LastIntimacy = LastPassion = LastCommitment = 0f;
    }

    public void AdjustPAD(float pleasureDelta, float arousalDelta, float dominanceDelta)
    {
        Pleasure = Mathf.Clamp01(Pleasure + pleasureDelta);
        Arousal = Mathf.Clamp01(Arousal + arousalDelta);
        Dominance = Mathf.Clamp01(Dominance + dominanceDelta);
    }

    public void SetLoveComponents(float intimacy, float passion, float commitment)
    {
        LastIntimacy = Intimacy;
        LastPassion = Passion;
        LastCommitment = Commitment;

        Intimacy = Mathf.Clamp01(intimacy);
        Passion = Mathf.Clamp01(passion);
        Commitment = Mathf.Clamp01(commitment);
    }
    
    public void ApplyLoveTriangle(float intimacy, float passion, float commitment)
{
    Intimacy = intimacy;
    Passion = passion;
    Commitment = commitment;
}


}
