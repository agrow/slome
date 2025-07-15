using UnityEngine;

public class PADState
{
    private float _pleasure;
    private float _arousal;
    private float _dominance;
//ro
    public float Pleasure => _pleasure;
    public float Arousal => _arousal;

    public float Dominance => _dominance;

    
    public PADState(float pleasure = 0f, float arousal = 0f, float dominance = 0f)
    {
        _pleasure = pleasure;
        _arousal = arousal;
        _dominance = dominance;

    }

    // basic arithmetic for now
    public void ApplyDelta(float dPleasure, float dArousal, float dDominance)
    {
        _pleasure += dPleasure;
        _arousal += dArousal;
        _dominance += dDominance;

    }

    // option to reset if needed
    public void Reset()
    {
        _pleasure = 0f;
        _arousal = 0f;
        _dominance = 0f;

    }
}
