using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using TL.Core;
using TL.EmotionalAI;

/// <summary>
/// Player UI controller: 
/// - Press T to open the panel
/// - Holds the current SelectedAction (set by UI buttons)
/// - ConfirmSelectedAction() runs validation, finds nearby NPC, and triggers the interaction
/// 
/// NOTE: All text-input & string->enum mapping removed.
/// </summary>
public class NewPlayerInputManager : MonoBehaviour
{
    // === Input for opening the UI with T ===
    private InputAction interactAction;

    // === UI References ===
    [Header("UI")]
    public GameObject playerUICanvas;           // The panel you show/hide
    public TextMeshProUGUI output;              // Status messages (optional)
    public TextMeshProUGUI selectedActionLabel; // Optional: shows "Selected: X"
    
    [Header("NPC Dialogue UI")]
    public GameObject npcDialogueCanvas;        // Separate canvas for NPC dialogue
    public TextMeshProUGUI npcStatusDisplay;    // Shows NPC emotional state info

    // === State ===
    // The currently selected action (chosen from the scroll list). Not executed until Confirm.
    public PlayerAction? SelectedAction { get; private set; }

    // Gate to prevent spamming, if you use it elsewhere.
    public bool EmotionalTriggered { get; set; }

    void Start()
    {
        EmotionalTriggered = false;

        Debug.Log($"[DEBUG] playerUICanvas assigned: {playerUICanvas != null}");

        // --- Bind T key to open the UI panel ---
        interactAction = new InputAction("interact", InputActionType.Button);
        interactAction.AddBinding("<Keyboard>/t"); 
        interactAction.performed += OnInteractPressed; // Subscribe to the performed event
        interactAction.Enable();

        // (Optional) Initialize label
        if (selectedActionLabel) selectedActionLabel.text = "Selected: (none)";
        Debug.Log("[GLOBAL_INPUT] PlayerInputManager initialized");
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

    // === Open the panel when T is pressed ===
    private void OnInteractPressed(InputAction.CallbackContext context)
    {
        if (!playerUICanvas.activeSelf)
        {
            Debug.Log("[GLOBAL_INPUT] T key pressed - showing UI");
            playerUICanvas.SetActive(true);
            // We no longer focus a TMP_InputField, since we removed text-entry.
        }
    }

    // === Called by action buttons (select only; does NOT execute) ===
    public void SetSelectedAction(PlayerAction action)
    {
        SelectedAction = action;

        if (selectedActionLabel)
            selectedActionLabel.text = $"Selected: {Pretty(action)}";

        Debug.Log($"[SELECTION] Player selected action: {action}");
    }

    // === Called by a separate Confirm button to run the action ===
    public void ConfirmSelectedAction()
    {
        if (!SelectedAction.HasValue)
        {
            if (output) output.text = "Pick an action first.";
            Debug.LogWarning("[CONFIRM] No action selected.");
            return;
        }

        var action = SelectedAction.Value;

        // Example: If you need pre-checks, add them here (cooldowns, resources, etc.)
        // if (!CanPerform(action)) { output.text = "Can't perform right now"; return; }

        // Find all NPCs and trigger interaction with the first nearby
        NPCController[] allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        Debug.Log($"[GLOBAL_INPUT] Found {allNPCs.Length} NPCs in scene");

        bool anyNPCTriggered = false;
        foreach (NPCController npc in allNPCs)
        {
            Debug.Log($"[GLOBAL_INPUT] Checking NPC: {npc.name}, IsPlayerNearby: {npc.IsPlayerNearby()}");
            if (npc.IsPlayerNearby())
            {
                Debug.Log($"[GLOBAL_INPUT] Triggering interaction with {npc.name}");
                npc.TriggerEmotionalInteraction(action);
                
                // Update UI with NPC status after interaction
                UpdateNPCStatusDisplay(npc, action);
                
                anyNPCTriggered = true;
                break;
            }
        }

        if (!anyNPCTriggered)
        {
            if (output) output.text = "No NPCs nearby for interaction.";
            Debug.Log("[GLOBAL_INPUT] No NPCs nearby for interaction");
            return;
        }

        // Set the emotional trigger flag for NPCs to detect
        EmotionalTriggered = true;
        
        // Success → clear & close (optional)
        ClearSelectedAction();
        if (playerUICanvas) playerUICanvas.SetActive(false);
    }

    // === Clear the selection (use after confirm or when closing panel) ===
    public void ClearSelectedAction()
    {
        SelectedAction = null;
        if (selectedActionLabel)
            selectedActionLabel.text = "Selected: (none)";
    }

    // === Update NPC status display ===
    private void UpdateNPCStatusDisplay(NPCController npc, PlayerAction playerAction)
    {
        if (npcStatusDisplay == null) return;
        
        // Activate the NPC dialogue canvas
        if (npcDialogueCanvas != null)
        {
            npcDialogueCanvas.SetActive(true);
        }
        
        var emotionModel = npc.emotionModel;
        if (emotionModel == null)
        {
            npcStatusDisplay.text = "No emotion data available";
            return;
        }
        
        // Get current NPC action (if available)
        string npcAction = "None";
        if (npc.emotionBrain?.bestAction != null)
        {
            npcAction = npc.emotionBrain.bestAction.Name;
        }
        
        // Format the display
        string statusText = $"<b>{npc.name} Status:</b>\n" +
                           $"Player Action: {Pretty(playerAction)}\n" +
                           $"NPC Response: {npcAction}\n\n" +
                           $"<b>Emotional State:</b>\n" +
                           $"Emotion: {emotionModel.lastEmotion}\n" +
                           $"P: {emotionModel.pad.P:F2} | A: {emotionModel.pad.A:F2} | D: {emotionModel.pad.D:F2}\n\n" +
                           $"<b>Relationship:</b>\n" +
                           $"Type: {emotionModel.GetCurrentRelationshipType()}\n" +
                           $"I: {emotionModel.tri.I:F2} | Pa: {emotionModel.tri.Pa:F2} | C: {emotionModel.tri.C:F2}";
        
        npcStatusDisplay.text = statusText;
    }
    
    // === Pretty-print for UI labels ===
    private static string Pretty(PlayerAction a)
    {
        // "KissQuick" -> "Kiss Quick", "hold_hands" -> "Hold Hands"
        var s = a.ToString().Replace("_", " ");
        s = Regex.Replace(s, "([a-z])([A-Z])", "$1 $2");
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
    }
}
