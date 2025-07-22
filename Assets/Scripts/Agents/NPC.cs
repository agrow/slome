using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    [Header("NPC Meta-Data")]
    public string npcName;

    [Header("Personality Setup")]
    public PersonalityProfile profile;  // MBTI trait class (e.g., E, N, F, P, A)
    public TextMeshProUGUI output; // Optional: Assign in Inspector to show info

    [HideInInspector] public string mbtiType;  // auto-generated like "ENFP-A"
    public PersonalityTypeDefinition definition;  // full trait data

    void Awake()
    {
        // TEMP: Create a personality profile manually if none set in Inspector
        if (profile == null)
        {
            profile = new PersonalityProfile(
                PersonalityProfile.Energy.Extraverted,
                PersonalityProfile.Mind.Intuitive,
                PersonalityProfile.Nature.Feeling,
                PersonalityProfile.Tactics.Prospecting,
                PersonalityProfile.Identity.Assertive
            );
        }
    }

    void Start()
    {
        if (profile == null)
        {
            Debug.LogWarning($"{npcName} has no personality profile.");
            return;
        }

        // Step 1: Generate MBTI string from profile
        mbtiType = profile.MBTIType;
        //Debug.Log($"{npcName}'s MBTI type: {mbtiType}");

        // Step 2: Use manually assigned definition
        if (definition == null)
        {
            Debug.LogError($"No PersonalityTypeDefinition assigned to NPC {npcName}.");
            return;
        }

        // Step 3: Compose output summary
        string summary = $"{npcName} is a {definition.typeName}.\n" +
                        $"Love Language: {definition.primaryLoveLanguage}\n" +
                        $"Attachment Style: {definition.attachmentStyle}\n" +
                        $"Emotional Baseline (PAD):\n" +
                        $"Pleasure: {definition.pleasureBaseline}, " +
                        $"Arousal: {definition.arousalBaseline}, " +
                        $"Dominance: {definition.dominanceBaseline}";

        Debug.Log(summary);
        if (output != null)
            output.text = summary;
    }


    // Simple command handler for test input 
    public string ReceiveCommand(string command, string[] args)
    {
        switch (command)
        {
            case "/talk":
                return $"{npcName} says: 'Nice weather today, huh?'";
            case "/flirt":
                return $"{npcName} blushes slightly.";
            default:
                return $"{npcName} doesn't understand.";
        }
    }
}
