using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TL.EmotionalAI; // Intent, IntentMapper
using TL.Core;        // PlayerAction

/// <summary>
/// Populates the Intent dropdown and the Action scroll list.
/// Uses your existing IntentMapper to determine which actions belong to each intent.
/// Clicking an action button only SELECTS it (doesn't execute).
/// Execution happens when the Confirm button calls PlayerInputManager.ConfirmSelectedAction().
/// </summary>
public class IntentActionSelectUI : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public NewPlayerInputManager inputManager;   // Your PlayerInputManager in the scene
    public TMP_Dropdown intentDropdown;       // Panel/IntentDropdown
    public Transform actionListContent;       // ActionList/Viewport/Content
    public Button actionButtonPrefab;         // Button (TextMeshPro) prefab

    // Optional: highlight UI for the last selected action button
    private Button _lastSelectedButton;

    void Awake()
    {
        if (!inputManager || !intentDropdown || !actionListContent || !actionButtonPrefab)
        {
            Debug.LogError("[IntentActionSelectUI] Missing references. " +
                           "Assign InputManager, IntentDropdown, ActionList/Content, ActionButtonPrefab.");
            enabled = false; 
            return;
        }

        // Fill dropdown with Intent enum names
        var names = Enum.GetNames(typeof(Intent)).ToList();
        intentDropdown.ClearOptions();
        intentDropdown.AddOptions(names);

        intentDropdown.onValueChanged.AddListener(OnIntentChanged);

        // Initial populate
        OnIntentChanged(intentDropdown.value);
    }

    void OnDestroy()
    {
        intentDropdown.onValueChanged.RemoveListener(OnIntentChanged);
    }

    // Called when the player chooses a new Intent
    private void OnIntentChanged(int idx)
    {
        var selectedIntent = (Intent)idx;

        // Clear old action buttons
        for (int i = actionListContent.childCount - 1; i >= 0; i--)
            Destroy(actionListContent.GetChild(i).gameObject);

        _lastSelectedButton = null;

        // Build the list of actions that map to this intent via your existing mapper
        var actions = Enum.GetValues(typeof(PlayerAction))
            .Cast<PlayerAction>()
            .Where(a =>
            {
                try { return IntentMapper.Map(a) == selectedIntent; }
                catch { return false; } // skip if mapper doesn't handle an action
            })
            .OrderBy(a => a.ToString()) // simple sort; you can use a "Pretty" if you like
            .ToList();

        // Spawn a button for each action
        foreach (var a in actions)
        {
            var btn = Instantiate(actionButtonPrefab, actionListContent);

            // Set label (supports TMP or legacy Text)
            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp) tmp.text = a.ToString();

            // When clicked: SELECT the action (does NOT execute)
            btn.onClick.AddListener(() =>
            {
                inputManager.SetSelectedAction(a);
                Highlight(btn);
            });
        }
    }

    // Optional: visual selection state
    private void Highlight(Button b)
    {
        if (_lastSelectedButton && _lastSelectedButton != b)
        {
            var old = _lastSelectedButton.colors;
            old.normalColor = Color.white;
            _lastSelectedButton.colors = old;
        }

        _lastSelectedButton = b;

        var c = b.colors;
        c.normalColor = new Color(0.85f, 0.95f, 1f); // subtle selected tint
        b.colors = c;
    }
}
