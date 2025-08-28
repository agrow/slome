using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TL.Core;
using TL.EmotionalAI;

namespace TL.UI
{
    /// <summary>
    /// Screen UI that displays NPC emotional state and current action in bottom corner
    /// </summary>
    public class EmotionalBillboard : MonoBehaviour
    {
        [Header("UI References")]
        public Canvas billboardCanvas;
        public TextMeshProUGUI emotionText;
        public TextMeshProUGUI padValuesText;
        public TextMeshProUGUI currentActionText;
        public TextMeshProUGUI stateText;
        public Image backgroundPanel;
        
        [Header("Visual Settings")]
        public Color normalColor = Color.white;
        public Color emotionalColor = Color.yellow;
        public float updateInterval = 0.1f; // How often to update UI
        
        [Header("Screen UI Settings")]
        public Vector2 screenPosition = new Vector2(10, 10); // Bottom-left corner offset
        public Vector2 panelSize = new Vector2(250, 150); // Size of the UI panel
        
        // References
        private NPCController npcController;
        private float lastUpdateTime;
        private RectTransform panelRectTransform;
        
        void Start()
        {
            // Get NPC Controller from parent
            npcController = GetComponentInParent<NPCController>();
            if (npcController == null)
            {
                Debug.LogError($"EmotionalBillboard: No NPCController found in parent!");
                gameObject.SetActive(false);
                return;
            }
            
            // Setup canvas for screen space
            if (billboardCanvas != null)
            {
                billboardCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                billboardCanvas.sortingOrder = 100; // Render on top
                
                // Position the canvas in bottom corner
                RectTransform canvasRect = billboardCanvas.GetComponent<RectTransform>();
                if (canvasRect != null)
                {
                    canvasRect.anchorMin = Vector2.zero; // Bottom-left
                    canvasRect.anchorMax = Vector2.zero; // Bottom-left
                    canvasRect.pivot = Vector2.zero; // Bottom-left
                    canvasRect.anchoredPosition = screenPosition;
                    canvasRect.sizeDelta = panelSize;
                }
                
                // Setup background panel
                if (backgroundPanel != null)
                {
                    panelRectTransform = backgroundPanel.GetComponent<RectTransform>();
                    if (panelRectTransform != null)
                    {
                        // Fill the canvas
                        panelRectTransform.anchorMin = Vector2.zero;
                        panelRectTransform.anchorMax = Vector2.one;
                        panelRectTransform.offsetMin = Vector2.zero;
                        panelRectTransform.offsetMax = Vector2.zero;
                    }
                }
            }
            else
            {
                Debug.LogError($"EmotionalBillboard: No billboard canvas assigned for {npcController.name}!");
                gameObject.SetActive(false);
                return;
            }
            
            // Initial update
            UpdateBillboard();
            
            Debug.Log($"EmotionalBillboard initialized for {npcController.name} in bottom corner");
        }
        
        void Update()
        {
            // Update UI data at intervals
            if (Time.time - lastUpdateTime > updateInterval)
            {
                UpdateBillboard();
                lastUpdateTime = Time.time;
            }
        }
        
        private void UpdateBillboard()
        {
            if (npcController == null) return;
            
            // Update NPC name in title
            UpdateNPCInfo();
            
            // Update emotion and PAD values
            UpdateEmotionDisplay();
            
            // Update current action
            UpdateActionDisplay();
            
            // Update state
            UpdateStateDisplay();
            
            // Update visual style based on emotional state
            UpdateVisualStyle();
        }
        
        private void UpdateNPCInfo()
        {
            // Add NPC name to emotion text
            if (emotionText != null && npcController.emotionModel != null)
            {
                string emotion = npcController.emotionModel.lastEmotion.ToString();
                emotionText.text = $"{npcController.name}\nEmotion: {emotion}";
            }
            else if (emotionText != null)
            {
                emotionText.text = $"{npcController.name}\nEmotion: N/A";
            }
        }
        
        private void UpdateEmotionDisplay()
        {
            if (padValuesText != null)
            {
                if (npcController.emotionModel != null)
                {
                    float p = npcController.emotionModel.pad.P;
                    float a = npcController.emotionModel.pad.A;
                    float d = npcController.emotionModel.pad.D;
                    
                    padValuesText.text = $"P: {p:F2}  A: {a:F2}  D: {d:F2}";
                }
                else if (npcController.emotionalState != null)
                {
                    float p = npcController.emotionalState.Pleasure;
                    float a = npcController.emotionalState.Arousal;
                    float d = npcController.emotionalState.Dominance;
                    
                    padValuesText.text = $"P: {p:F2}  A: {a:F2}  D: {d:F2}";
                }
                else
                {
                    padValuesText.text = "PAD: N/A";
                }
            }
        }
        
        private void UpdateActionDisplay()
        {
            if (currentActionText != null)
            {
                string actionInfo = "Action: ";
                
                if (npcController.isExecutingAction && npcController.currentState == NPCController.State.execute)
                {
                    // Try to get current action name
                    if (npcController.emotionBrain != null && npcController.emotionBrain.bestAction != null)
                    {
                        actionInfo += $"[EMOTIONAL] {npcController.emotionBrain.bestAction.Name}";
                        currentActionText.color = emotionalColor;
                    }
                    else if (npcController.aiBrain != null && npcController.aiBrain.bestAction != null)
                    {
                        actionInfo += $"[UTILITY] {npcController.aiBrain.bestAction.Name}";
                        currentActionText.color = normalColor;
                    }
                    else
                    {
                        actionInfo += "Executing...";
                        currentActionText.color = normalColor;
                    }
                }
                else
                {
                    actionInfo += "None";
                    currentActionText.color = normalColor;
                }
                
                currentActionText.text = actionInfo;
            }
        }
        
        private void UpdateStateDisplay()
        {
            if (stateText != null)
            {
                stateText.text = $"State: {npcController.currentState}";
                
                // Color code states
                switch (npcController.currentState)
                {
                    case NPCController.State.decide:
                        stateText.color = Color.blue;
                        break;
                    case NPCController.State.move:
                        stateText.color = Color.green;
                        break;
                    case NPCController.State.execute:
                        stateText.color = Color.red;
                        break;
                    case NPCController.State.idle:
                        stateText.color = Color.yellow;
                        break;
                    default:
                        stateText.color = Color.white;
                        break;
                }
            }
        }
        
        private void UpdateVisualStyle()
        {
            if (backgroundPanel != null)
            {
                // Change background color based on emotional vs utility AI
                bool isEmotional = ShouldUseEmotionalActions();
                backgroundPanel.color = isEmotional ? 
                    new Color(emotionalColor.r, emotionalColor.g, emotionalColor.b, 0.8f) : 
                    new Color(normalColor.r, normalColor.g, normalColor.b, 0.8f);
            }
        }
        
        private bool ShouldUseEmotionalActions()
        {
            float timeSinceLastTrigger = Time.time - npcController.lastEmotionalTrigger;
            return timeSinceLastTrigger <= npcController.emotionalTimeout && 
                   !npcController.hasRespondedToCurrentTrigger;
        }
        
        // Public method to make fields accessible
        public void SetVisible(bool visible)
        {
            if (billboardCanvas != null)
            {
                billboardCanvas.gameObject.SetActive(visible);
            }
        }
    }
}