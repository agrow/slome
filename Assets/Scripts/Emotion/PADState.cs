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

    // Personality: e.g., MBTI type (used later for modulation)
    public string mbtiType;

    // we could structure base line emotions for personality like this?
    public EmotionalState(string mbtiType = "INFP")
    {
        this.mbtiType = mbtiType;
        pleasure = 0f;
        arousal = 0f;
        dominance = 0f;
        intimacy = 0f;
        passion = 0f;
        commitment = 0f;
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

    // Update PAD based on Triangle of Love values
    public void ApplyLoveTriangle()
    {
        // Intimacy → Pleasure and Arousal (pleasure is more sensitive here?)
        if (intimacy > 0.6f)
        {
            pleasure += (intimacy - 0.6f) * 0.5f;
            arousal  += (intimacy - 0.6f) * 0.2f;
        }

        // Passion → Arousal and Pleasure (arousal is more sensitive here?)
        if (passion > 0.5f)
        {
            arousal  += (passion - 0.5f) * 0.6f; 
            pleasure += (passion - 0.5f) * 0.3f;
        }

        // Commitment → Dominance and Pleasure (dominance is more sensitive here?) 
        // not sure if i am using dominance correctly here... not sure what else to use though
        if (commitment > 0.7f)
        {
            dominance += (commitment - 0.7f) * 0.4f;
            pleasure += (commitment - 0.7f) * 0.2f;
        }

        ClampPAD();
    }

    // Clamp PAD values to [-1, 1]
    private void ClampPAD()
    {
        pleasure = Mathf.Clamp(pleasure, -1f, 1f);
        arousal = Mathf.Clamp(arousal, -1f, 1f);
        dominance = Mathf.Clamp(dominance, -1f, 1f);
    }

    // Optional: Apply personality effects (stubbed for now)
    public void ApplyPersonalityInfluence()
    {
        // Future logic: Use MBTI types to bias PAD changes
        // e.g., INFP might be more sensitive to intimacy (pleasure boost)
    }

    // Debug method
    public override string ToString()
    {
        return $"PAD: ({pleasure:F2}, {arousal:F2}, {dominance:F2}) | ToL: ({intimacy:F2}, {passion:F2}, {commitment:F2})";
    }
}
