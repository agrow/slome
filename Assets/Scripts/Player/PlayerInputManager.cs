using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TL.Core;
using TMPro;
using TL.EmotionalAI;

public class PlayerInputManager : MonoBehaviour
{
    private InputAction interactAction;   
    public GameObject playerUICanvas; 
    public TextMeshProUGUI output;
    public TMP_InputField playerInput;
    private PlayerAction parsedAction;
    private Dictionary<string, PlayerAction> commandMap;

    public bool EmotionalTriggered { get; set;}

    
    void Start()
    {
        EmotionalTriggered = true; // Initialize to true to allow first interaction
                                   // Initialize command map for text commands
                                   // need to add new commands here as they are added to PlayerAction for versatility
                                   // add intensity levels to player actions, so that every intent has a base pad, multiplied by the degree at which 
                                   // the action is considered
                                   // ...existing code...
        Debug.Log($"[DEBUG] playerUICanvas assigned: {playerUICanvas != null}");
        commandMap = new Dictionary<string, PlayerAction>()
        {
            // Affection
            { "compliment_looks", PlayerAction.ComplimentLooks },
            { "hug",              PlayerAction.Hug },
            { "hold_hands",       PlayerAction.HoldHands },
            { "comfort",          PlayerAction.Comfort },
            { "encourage",        PlayerAction.Encourage },
            { "gift_small",       PlayerAction.GiftSmall },
        
            // Desire
            { "kiss_quick",       PlayerAction.KissQuick },
            { "kiss_deep",        PlayerAction.KissDeep },
            { "flirt",            PlayerAction.Flirt },
            { "seduce",           PlayerAction.Seduce },
            { "long_for",         PlayerAction.LongFor },
        
            // Bonding
            { "invite_activity",  PlayerAction.InviteActivity },
            { "share_story",      PlayerAction.ShareStory },
            { "reminisce",        PlayerAction.Reminisce },
            { "celebrate",        PlayerAction.Celebrate },
            { "support",          PlayerAction.Support },
        
            // Trust
            { "apology",          PlayerAction.Apology },
            { "confide",          PlayerAction.Confide },
            { "forgive",          PlayerAction.Forgive },
            { "ask_help",         PlayerAction.AskHelp },
            { "promise",          PlayerAction.Promise },
        
            // Respect
            { "compliment_skill", PlayerAction.ComplimentSkill },
            { "acknowledge",      PlayerAction.Acknowledge },
            { "admire",           PlayerAction.Admire },
            { "defend",           PlayerAction.Defend },
            { "praise",           PlayerAction.Praise },
        
            // Playfulness
            { "tease_playful",    PlayerAction.TeasePlayful },
            { "joke",             PlayerAction.Joke },
            { "challenge",        PlayerAction.Challenge },
            { "surprise",         PlayerAction.Surprise },
            { "trick",            PlayerAction.Trick },
        
            // Security
            { "keep_promise",     PlayerAction.KeepPromise },
            { "reassure",         PlayerAction.Reassure },
            { "protect",          PlayerAction.Protect },
            { "shelter",          PlayerAction.Shelter },
            { "steady",           PlayerAction.Steady },
        
            // Conflict
            { "tease_harsh",      PlayerAction.TeaseHarsh },
            { "confront",         PlayerAction.Confront },
            { "criticize",       PlayerAction.Criticize },
            { "withdraw",        PlayerAction.Withdraw },
            { "demand",          PlayerAction.Demand },

            //Manipulation
            { "guilt_trip",      PlayerAction.GiftLarge },
            { "bribe",           PlayerAction.GuiltTrip },
            { "threaten",        PlayerAction.Flatter },
            { "lie",             PlayerAction.Pressure },
            { "blackmail",       PlayerAction.Withhold }
        };

        
        // Setup Input Action for T key
        interactAction = new InputAction("interact", InputActionType.Button);
        interactAction.AddBinding("<Keyboard>/t");


        // Subscribe to the performed event
        interactAction.performed += OnInteractPressed;

        // Enable the action
        interactAction.Enable();
        Debug.Log("[GLOBAL_INPUT] PlayerInputManager initialized");
    }

    //called whenever t is pressed 
    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (!playerUICanvas.activeSelf)
        {
            Debug.Log("[GLOBAL_INPUT] T key pressed - showing UI");
            playerUICanvas.SetActive(true);
            playerInput.text = "";
            playerInput.Select();
            playerInput.ActivateInputField(); // Focus input for typing
        }
    }

    // Called by the Done button
    public void ParseInput()
    {
        EmotionalTriggered = true; // Initialize to true to allow first interaction
        playerUICanvas.SetActive(false); // Hide UI at start
        string inputText = playerInput.text.ToLower().Trim();
        // Validate and parse input
        if (!commandMap.TryGetValue(inputText, out parsedAction))
        {
            output.text = $"Unknown command: {inputText}";
            Debug.LogWarning($"[GLOBAL_INPUT] Invalid action input: {inputText}");
            return;
        }
        
        playerUICanvas.SetActive(false);
        // Find all NPCs and trigger interaction
        NPCController[] allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        bool anyNPCTriggered = false;
        foreach (NPCController npc in allNPCs)
        {
            if (npc.IsPlayerNearby())
            {
                Debug.Log($"[GLOBAL_INPUT] Triggering interaction with {npc.name}");
                // Entry Point for NPC interaction
                npc.TriggerEmotionalInteraction(parsedAction);
                anyNPCTriggered = true;
                break;
            }
        }
        if (!anyNPCTriggered)
        {
            output.text = "No NPCs nearby for interaction.";
            Debug.Log("[GLOBAL_INPUT] No NPCs nearby for interaction");
        }
    }

    void OnDestroy()
    {
        // Clean up input action
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPressed;
            interactAction.Disable();
            interactAction.Dispose();
        }
    }
}