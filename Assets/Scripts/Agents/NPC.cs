using UnityEngine;
using TMPro;


public class NPC : MonoBehaviour
{
    //NPC Meta-Data:
    public string npcName;
    private string Personality;
    private string AttachmentStyle;
    private Vector3 PADVector;
    private Vector3 RelationshipTriangle;
    private string LoveLanguage;

    public TextMeshProUGUI output; //output of player ui


    // public void Interact()
    // {
    //     Debug.Log($"You interacted with {npcName}!");
    //     // TODO: Trigger dialogue, animation, etc.
    // }


    //Purpose Statement: Recieve command from player, source out commands and how 
    // they will interact with the system. 
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
