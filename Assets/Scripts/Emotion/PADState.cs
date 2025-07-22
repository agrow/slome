using UnityEngine;

public class EmotionalState
{
    // PAD values: range [-1, 1]
    private float pleasure;
    private float arousal;
    private float dominance;

    // Triangle of Love values: range [0, 1]
    private float intimacy;
    private float passion;
    private float commitment;

    // Reference to the personality data
    private readonly PersonalityTypeDefinition personality;

    // Constructor
    public EmotionalState(PersonalityTypeDefinition personality)
    {
        this.personality = personality;

        // Initialize PAD using baselines (map [0,1] to [-1,1])
        pleasure = NormalizeBaseline(personality.pleasureBaseline);
        arousal = NormalizeBaseline(personality.arousalBaseline);
        dominance = NormalizeBaseline(personality.dominanceBaseline);

        intimacy = 0f;
        passion = 0f;
        commitment = 0f;
    }

    // Normalizes 0–1 to -1 to 1
    private float NormalizeBaseline(float baseline)
    {
        return (baseline * 2f) - 1f;
    }

    // Read-only accessors
    public float Pleasure => pleasure;
    public float Arousal => arousal;
    public float Dominance => dominance;

    public float Intimacy
    {
        get => intimacy;
        set => intimacy = Mathf.Clamp01(value);
    }

    public float Passion
    {
        get => passion;
        set => passion = Mathf.Clamp01(value);
    }

    public float Commitment
    {
        get => commitment;
        set => commitment = Mathf.Clamp01(value);
    }

    // Update PAD based on Triangle of Love logic
    public void ApplyLoveTriangle()
    {
        if (intimacy > 0.6f)
        {
            pleasure += (intimacy - 0.6f) * 0.5f;
            arousal  += (intimacy - 0.6f) * 0.2f;
        }

        if (passion > 0.5f)
        {
            arousal  += (passion - 0.5f) * 0.6f;
            pleasure += (passion - 0.5f) * 0.3f;
        }

        if (commitment > 0.7f)
        {
            dominance += (commitment - 0.7f) * 0.4f;
            pleasure  += (commitment - 0.7f) * 0.2f;
        }

        ClampPAD();
    }

    private void ClampPAD()
    {
        pleasure = Mathf.Clamp(pleasure, -1f, 1f);
        arousal = Mathf.Clamp(arousal, -1f, 1f);
        dominance = Mathf.Clamp(dominance, -1f, 1f);
    }

    // Debug method
    public override string ToString()
    {
        return $"PAD: ({pleasure:F2}, {arousal:F2}, {dominance:F2}) | ToL: ({intimacy:F2}, {passion:F2}, {commitment:F2})";
    }
}
