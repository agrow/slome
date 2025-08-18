using UnityEngine;

public class PersonalityProfile
{
    //okay to use full names for enums here
    public enum Energy { Introverted, Extraverted }
    public enum Mind { Observant, Intuitive }
    public enum Nature { Thinking, Feeling }
    public enum Tactics { Judging, Prospecting }
    public enum Identity { Assertive, Turbulent }

    private Energy energy;
    private Mind mind;
    private Nature nature;
    private Tactics tactics;
    private Identity identity;

    // for accessing later in case we need (ro)
    public Energy EnergyTrait => energy;
    public Mind MindTrait => mind;
    public Nature NatureTrait => nature;
    public Tactics TacticsTrait => tactics;
    public Identity IdentityTrait => identity;

    public string MBTIType => GenerateMBTIType();

    //basic constructor for now 
    public PersonalityProfile(Energy energy, Mind mind, Nature nature, Tactics tactics, Identity identity)
    {
        this.energy = energy;
        this.mind = mind;
        this.nature = nature;
        this.tactics = tactics;
        this.identity = identity;
    }

    // private method to generate the MBTI type string// for logging for now?
    private string GenerateMBTIType()
    {
        string mbti = "";
        mbti += (energy == Energy.Extraverted) ? "E" : "I";
        mbti += (mind == Mind.Intuitive) ? "N" : "S";
        mbti += (nature == Nature.Thinking) ? "T" : "F";
        mbti += (tactics == Tactics.Judging) ? "J" : "P";
        mbti += "-" + ((identity == Identity.Assertive) ? "A" : "T");
        return mbti;
    }

    // NEW: Convert MBTI traits to PAD values
    public Vector3 GetPADValues()
    {
        float pleasure = CalculateBasePleasure();
        float arousal = CalculateBaseArousal();
        float dominance = CalculateBaseDominance();

        return new Vector3(pleasure, arousal, dominance);
    }

    private float CalculateBasePleasure()
    {
        float pleasure = 0.5f; // Base neutral

        // Extraverted types tend to have higher baseline pleasure (social energy)
        if (energy == Energy.Extraverted) pleasure += 0.2f;

        // Feeling types tend to experience more positive emotions
        if (nature == Nature.Feeling) pleasure += 0.15f;

        // Assertive identity types have higher baseline satisfaction
        if (identity == Identity.Assertive) pleasure += 0.1f;

        // Prospecting types tend to be more optimistic/adaptable
        if (tactics == Tactics.Prospecting) pleasure += 0.05f;

        return Mathf.Clamp01(pleasure);
    }

    private float CalculateBaseArousal()
    {
        float arousal = 0.5f; // Base neutral

        // Extraverted types have higher energy/activation
        if (energy == Energy.Extraverted) arousal += 0.25f;

        // Intuitive types are more mentally active
        if (mind == Mind.Intuitive) arousal += 0.1f;

        // Prospecting types are more spontaneous/energetic
        if (tactics == Tactics.Prospecting) arousal += 0.15f;

        // Turbulent types have higher anxiety/activation
        if (identity == Identity.Turbulent) arousal += 0.1f;

        return Mathf.Clamp01(arousal);
    }

    private float CalculateBaseDominance()
    {
        float dominance = 0.5f; // Base neutral

        // Extraverted types are more assertive in social situations
        if (energy == Energy.Extraverted) dominance += 0.2f;

        // Thinking types tend to be more decisive/controlling
        if (nature == Nature.Thinking) dominance += 0.15f;

        // Judging types prefer structure and control
        if (tactics == Tactics.Judging) dominance += 0.1f;

        // Assertive identity types are more confident/dominant
        if (identity == Identity.Assertive) dominance += 0.15f;

        return Mathf.Clamp01(dominance);
    }

    // NEW: Get social behavior multipliers based on MBTI
    public float GetSocialActionMultiplier()
    {
        float multiplier = 1f;

        // Extraverted types are much more socially inclined
        if (energy == Energy.Extraverted) multiplier += 0.8f;
        else multiplier -= 0.4f; // Introverts are less social

        // Feeling types are more relationship-focused
        if (nature == Nature.Feeling) multiplier += 0.3f;

        // Assertive types are more confident in social situations
        if (identity == Identity.Assertive) multiplier += 0.2f;

        return Mathf.Max(0.1f, multiplier);
    }

    public float GetRomanticActionMultiplier()
    {
        float multiplier = 1f;

        // Feeling types are more emotionally expressive
        if (nature == Nature.Feeling) multiplier += 0.5f;

        // Extraverted types are more likely to express romantic interest
        if (energy == Energy.Extraverted) multiplier += 0.3f;

        // Prospecting types are more spontaneous in romance
        if (tactics == Tactics.Prospecting) multiplier += 0.2f;

        // Intuitive types are more imaginative/romantic
        if (mind == Mind.Intuitive) multiplier += 0.2f;

        return Mathf.Max(0.1f, multiplier);
    }

    public float GetAssertiveActionMultiplier()
    {
        float multiplier = 1f;

        // Thinking types are more logical/direct
        if (nature == Nature.Thinking) multiplier += 0.4f;

        // Judging types are more decisive
        if (tactics == Tactics.Judging) multiplier += 0.3f;

        // Assertive identity types are more confident
        if (identity == Identity.Assertive) multiplier += 0.4f;

        // Extraverted types are more likely to be assertive
        if (energy == Energy.Extraverted) multiplier += 0.2f;

        return Mathf.Max(0.1f, multiplier);
    }

    // NEW: Calculate compatibility with another personality
    public float GetCompatibilityWith(PersonalityProfile other)
    {
        if (other == null) return 0.5f;

        float compatibility = 0f;
        int factors = 0;

        // Energy compatibility (opposites can attract, but similar is stable)
        float energyCompat = (energy == other.energy) ? 0.8f : 0.6f;
        compatibility += energyCompat;
        factors++;

        // Mind compatibility (similar thinking styles work well)
        float mindCompat = (mind == other.mind) ? 0.9f : 0.4f;
        compatibility += mindCompat;
        factors++;

        // Nature compatibility (balanced T/F relationships work well)
        float natureCompat = (nature == other.nature) ? 0.7f : 0.8f; // Slight preference for complementary
        compatibility += natureCompat;
        factors++;

        // Tactics compatibility (opposites can balance each other)
        float tacticsCompat = (tactics == other.tactics) ? 0.6f : 0.8f;
        compatibility += tacticsCompat;
        factors++;

        // Identity compatibility (similar confidence levels work well)
        float identityCompat = (identity == other.identity) ? 0.8f : 0.5f;
        compatibility += identityCompat;
        factors++;

        return compatibility / factors;
    }

    // NEW: Personality preferences
    public bool PrefersPlayerInteraction()
    {
        // Extraverted and Feeling types prefer interacting with player
        return energy == Energy.Extraverted || nature == Nature.Feeling;
    }

    public bool AvoidsConflict()
    {
        // Feeling types and Turbulent identity avoid conflict
        return nature == Nature.Feeling || identity == Identity.Turbulent;
    }

    public bool SeeksThrills()
    {
        // Extraverted Prospecting types seek thrills
        return energy == Energy.Extraverted && tactics == Tactics.Prospecting;
    }
}