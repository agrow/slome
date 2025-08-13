// Assets/Scripts/Personality/PersonalityTypeDefinition.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PersonalityTypeDefinition", menuName = "NPC/Personality Type Definition")]
public class PersonalityTypeDefinition : ScriptableObject
{
    public string typeName;  
    [TextArea] public string description;

    [Range(0, 1)] public float sensitivity;
    [Range(0, 1)] public float expressiveness;
    [Range(0, 1)] public float emotionalResilience;

    public bool prefersDeepConversations;
    [Range(0, 1)] public float groupEnergy;

    [Range(0, 1)] public float emotionalDepth;
    [Range(0, 1)] public float autonomy;
    [Range(0, 1)] public float structure;

    public string stressResponse;
    public string primaryLoveLanguage;
    public string attachmentStyle;

    [Range(0, 1)] public float pleasureBaseline;
    [Range(0, 1)] public float arousalBaseline;
    [Range(0, 1)] public float dominanceBaseline;
}
