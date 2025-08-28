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



    /*


    */ 

    // Brainstorm how to use these for considerations? 
    public string primaryLoveLanguage; // e.g., Words of Affirmation, Acts of Service, Receiving Gifts, Quality Time, Physical Touch, a consideration 
    /* Example distributions of considerations: for Love Language 
        Physical Touch: 1.0 
        Words of Affirmation: 8.0 
        Quality Time: 3.0
        Acts of Service: 6.0
        Receiving Gifts: 2.0 

        Touch: if i am feeling low pleasure, and someone touches me, i get a boost to pleasure. 
            - Physical touch? 1.0 
            - Intent Nudge (Hug, Hold Hands, Kiss, Cuddle): Light Nudge + 
            - Consideration of PAD
            - Consideration of PAD Delta
            - Consideration of Relationship Status 
    */ 
    public string attachmentStyle;

    [Range(0, 1)] public float pleasureBaseline;
    [Range(0, 1)] public float arousalBaseline;
    [Range(0, 1)] public float dominanceBaseline;
}
