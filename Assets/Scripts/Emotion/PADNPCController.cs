using UnityEngine;
using UnityEngine.InputSystem;

public class PADNPCController : MonoBehaviour
{
    private EmotionalState emotionalState;
    public PersonalityTypeDefinition personality;

    public float moveSpeed = 3f;

    private Vector2 movementInput;

    // Input Action Asset (Optional: can also wire via PlayerInput)
    [SerializeField] private InputAction movementAction;

    private void OnEnable()
    {
        movementAction.Enable();
    }

    private void OnDisable()
    {
        movementAction.Disable();
    }

    private void Awake()
    {
        // If you didn't assign in the Inspector, you can create manually
        if (movementAction == null || !movementAction.enabled)
        {
            movementAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
            movementAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            movementAction.Enable();
        }
    }

    private void Start()
    {
        emotionalState = new EmotionalState
        {
            Pleasure = personality.pleasureBaseline,
            Arousal = personality.arousalBaseline,
            Dominance = personality.dominanceBaseline
        };

        Debug.Log($"Initialized PAD — Pleasure: {emotionalState.Pleasure}, Arousal: {emotionalState.Arousal}, Dominance: {emotionalState.Dominance}");
    }

    private void Update()
    {
        movementInput = movementAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(movementInput.x, 0f, movementInput.y);
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }
}




/*
using UnityEngine;

public class PADNPCController : MonoBehaviour
{
    [Header("NPC Personality")]
    public PersonalityTypeDefinition personalityData;

    [Header("Movement Settings")]
    public float moveSpeed = 3f;

    private EmotionalState emotionalState;

    private void Start()
    {
        // Initialize emotional state using personality data if available
        if (personalityData != null)
        {
            emotionalState = new EmotionalState(personalityData);
            Debug.Log($"Initialized EmotionalState from {personalityData.Type}: " +
                      $"Pleasure: {emotionalState.Pleasure}, " +
                      $"Arousal: {emotionalState.Arousal}, " +
                      $"Dominance: {emotionalState.Dominance}");
        }
        else
        {
            emotionalState = new EmotionalState();
            Debug.LogWarning("No PersonalityTypeDefinition assigned. Using default PAD values.");
        }
    }

    private void Update()
    {
        // Basic WASD movement with arousal influencing speed
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Example: Use Arousal to modify move speed (0.5x to 1.5x range)
        float speedModifier = Mathf.Lerp(0.5f, 1.5f, emotionalState.Arousal);
        Vector3 movement = new Vector3(moveX, 0f, moveZ) * moveSpeed * speedModifier * Time.deltaTime;

        transform.Translate(movement);
    }

    public EmotionalState GetEmotionalState()
    {
        return emotionalState;
    }
}

// with the PersonalityTypeDefinition class for later use


*/