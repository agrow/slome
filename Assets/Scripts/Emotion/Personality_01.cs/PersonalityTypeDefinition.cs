// Assets/Scripts/Personality/PersonalityTypeDefinition.cs
using UnityEngine;

[CreateAssetMenu(fileName = "PersonalityTypeDefinition", menuName = "NPC/Personality Type Definition")]
public class PersonalityTypeDefinition : ScriptableObject
{
    public string typeName;  
    [TextArea] public string description;

    [Range(0, 1)] public float energy;

    [Range(0, 1)] public float mind;

    [Range(0, 1)] public float nature;

    [Range(0, 1)] public float tactics;

    [Range(0, 1)] public float identity;

    // we should prob add more dimensions to the personality model

    [Range(0, 1)] public float sensitivity; 


    public string primaryLoveLanguage; // only focusing on this for now 

    //Love Language: Words of Affirmation, Acts of Service, Receiving Gifts, Quality Time, Physical Touch
    // these are going to be used in the considerations ! as binary numbers that will be added to the considerations score 
    public int WordsOfAffirmation;
    public int ActsOfService;
    public int ReceivingGifts;
    public int QualityTime;
    public int PhysicalTouch;



    [Range(0, 1)] public float pleasureBaseline;
    [Range(0, 1)] public float arousalBaseline;
    [Range(0, 1)] public float dominanceBaseline;
}
