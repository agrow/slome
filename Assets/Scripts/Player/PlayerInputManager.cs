using UnityEngine;
using UnityEngine.InputSystem;
using TL.Core;

public class PlayerInputManager : MonoBehaviour
{
    private InputAction interactAction;

    void Start()
    {
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
        Debug.Log("[GLOBAL_INPUT] T key pressed - checking all nearby NPCs");
        
        // Find all NPCs and check which ones are nearby
        NPCController[] allNPCs = FindObjectsByType<NPCController>(FindObjectsSortMode.None);
        bool anyNPCTriggered = false;
        
        foreach (NPCController npc in allNPCs)
        {
            if (npc.IsPlayerNearby())
            {
                Debug.Log($"[GLOBAL_INPUT] Triggering interaction with {npc.name}");
                npc.TriggerEmotionalInteraction();
                Debug.Log("key pressed!");
                anyNPCTriggered = true;
                break; // Only trigger closest/first NPC
            }
        }
        
        if (!anyNPCTriggered)
        {
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