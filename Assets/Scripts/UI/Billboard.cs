using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TL.Core;

namespace TL.UI
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private TextMeshProUGUI bestActionText;
        [SerializeField] private TextMeshProUGUI inventoryText;
        private Transform mainCameraTransform;

        // Start is called before the first frame update
        void Start()
        {
            // Safety check for Camera.main
            if (Camera.main != null)
            {
                mainCameraTransform = Camera.main.transform;
            }
            else
            {
                // Fallback: find any camera in scene
                Camera fallbackCamera = FindAnyObjectByType<Camera>();
                if (fallbackCamera != null)
                {
                    mainCameraTransform = fallbackCamera.transform;
                    Debug.LogWarning("Billboard: Camera.main not found, using fallback camera");
                }
                else
                {
                    Debug.LogError("Billboard: No camera found in scene!");
                }
            }
            
            // Initialize text components with default values
            if (statsText != null) statsText.text = "Stats Loading...";
            if (bestActionText != null) bestActionText.text = "Deciding...";
            if (inventoryText != null) inventoryText.text = "Inventory Loading...";
        }

        void LateUpdate()
        {
            // Safety check before rotating
            if (mainCameraTransform != null)
            {
                transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, 
                                mainCameraTransform.rotation * Vector3.up);
            }
        }

        // Updated method to include emotional stats
        public void UpdateStatsText(int energy, int hunger, int money, EmotionalState emotionalState = null)
        {
            // Safety check
            if (statsText == null) 
            {
                Debug.LogWarning("Billboard: statsText is null! Make sure TextMeshPro component is assigned.");
                return;
            }
            
            string basicStats = $"Energy: {energy}\nHunger: {hunger}\nMoney: {money}";
            
            if (emotionalState != null)
            {
                string emotionalStats = $"\n--- Emotional State ---" +
                                      $"\nPleasure: {emotionalState.Pleasure:F2}" +
                                      $"\nArousal: {emotionalState.Arousal:F2}" +
                                      $"\nDominance: {emotionalState.Dominance:F2}" +
                                      $"\n--- Relationships ---" +
                                      $"\nIntimacy: {emotionalState.Intimacy:F2}" +
                                      $"\nPassion: {emotionalState.Passion:F2}" +  
                                      $"\nCommitment: {emotionalState.Commitment:F2}";
                
                statsText.text = basicStats + emotionalStats;
            }
            else
            {
                statsText.text = basicStats;
            }
        }

        public void UpdateBestActionText(string bestAction)
        {
            // Safety check
            if (bestActionText == null) 
            {
                Debug.LogWarning("Billboard: bestActionText is null! Make sure TextMeshPro component is assigned.");
                return;
            }
            
            bestActionText.text = $"Action: {bestAction}";
        }

        public void UpdateInventoryText(int wood, int stone, int food)
        {
            // Safety check
            if (inventoryText == null) 
            {
                Debug.LogWarning("Billboard: inventoryText is null! Make sure TextMeshPro component is assigned.");
                return;
            }
            
            inventoryText.text = $"Wood: {wood}\nStone: {stone}\nFood: {food}";
        }
        
        // Bonus: Method to update everything at once
        public void UpdateAllStats(Stats stats)
        {
            if (stats == null) return;
            
            EmotionalState emotional = stats.GetEmotionalState();
            UpdateStatsText(stats.energy, stats.hunger, stats.money, emotional);
            
            // Update inventory if available
            if (stats.GetComponent<NPCInventory>() != null)
            {
                var inventory = stats.GetComponent<NPCInventory>();
                // UpdateInventoryText(inventory.wood, inventory.stone, inventory.food);
            }
        }
    }
}