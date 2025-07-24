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

    private float lastIntimacy;
    private float lastPassion;
    private float lastCommitment;

    private readonly PersonalityTypeDefinition personality;

    // Constructor
    public EmotionalState(PersonalityTypeDefinition personality)
    {
        this.personality = personality;

        // Initialize PAD using baselines (map [0,1] to [-1,1])
        pleasure = NormalizeBaseline(personality.pleasureBaseline);
        arousal = NormalizeBaseline(personality.arousalBaseline);
        dominance = NormalizeBaseline(personality.dominanceBaseline);

        intimacy = passion = commitment = 0f;
        lastIntimacy = lastPassion = lastCommitment = 0f;
    }

    private float NormalizeBaseline(float baseline)
    {
        return (baseline * 2f) - 1f;
    }

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

    public void UpdateLoveValues(float intimacyDelta, float passionDelta, float commitmentDelta)
    {
        lastIntimacy = intimacy;
        lastPassion = passion;
        lastCommitment = commitment;

        Intimacy += intimacyDelta;
        Passion += passionDelta;
        Commitment += commitmentDelta;

        ApplyLoveTriangle();
        LogStatusChanges();
    }

    // Adjust PAD based on current ToL values
    public void ApplyLoveTriangle()
    {
        // Base effects
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

    // Notification log for value drops
    private void LogStatusChanges()
    {
        if (intimacy < lastIntimacy)
        {
            Debug.Log($"[ToL] Intimacy dropped to {intimacy:F2}");
        }
        if (passion < lastPassion)
        {
            Debug.Log($"[ToL] Passion dropped to {passion:F2}");
        }
        if (commitment < lastCommitment)
        {
            Debug.Log($"[ToL] Commitment dropped to {commitment:F2}");
        }
    }

    public void DisplayLoveTriangle()
    {
        Debug.Log($"Love Triangle → Intimacy: {intimacy:F2}, Passion: {passion:F2}, Commitment: {commitment:F2}");
    }

    public override string ToString()
    {
        return $"PAD: ({pleasure:F2}, {arousal:F2}, {dominance:F2}) | ToL: ({intimacy:F2}, {passion:F2}, {commitment:F2})";
    }
}
