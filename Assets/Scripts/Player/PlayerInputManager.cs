using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TL.Core;
using TMPro;
using TL.EmotionalAI;

public class PlayerInputManager : MonoBehaviour
{
    public GameObject playerUICanvas; 
    private InputAction interactAction;
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
        commandMap = new Dictionary<string, PlayerAction>()
        {
            { "compliment_looks", PlayerAction.ComplimentLooks },
            { "compliment_skill", PlayerAction.ComplimentSkill },
            { "flirt", PlayerAction.Flirt },
            { "hold_hands", PlayerAction.HoldHands },
            { "hug", PlayerAction.Hug },
            { "kiss_quick", PlayerAction.KissQuick },
            { "kiss_deep", PlayerAction.KissDeep },
            { "gift_small", PlayerAction.GiftSmall },
            { "gift_large", PlayerAction.GiftLarge },
            { "apology", PlayerAction.Apology },
            { "tease_playful", PlayerAction.TeasePlayful },
            { "tease_harsh", PlayerAction.TeaseHarsh },
            { "keep_promise", PlayerAction.KeepPromise },
            { "invite_activity", PlayerAction.InviteActivity }
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

    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        Debug.Log("[GLOBAL_INPUT] T key pressed - showing UI");
        playerUICanvas.SetActive(true);
        playerInput.text = "";
    }

    // Called by the Done button
    public void ParseInput()
    {
        EmotionalTriggered = true; // Initialize to true to allow first interaction
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