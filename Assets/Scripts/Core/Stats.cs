using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UI;

namespace TL.Core
{
    public class Stats : MonoBehaviour
    {
        private int _energy;
        public int energy
        {
            get { return _energy; }
            set
            {
                _energy = Mathf.Clamp(value, 0, 100);
                
                // Energy affects PAD: Low energy = low arousal, low pleasure
                if (emotionalState != null)
                {
                    float energyRatio = _energy / 100f; // 0-1 ratio
                    emotionalState.AdjustPAD(
                        (energyRatio - 0.5f) * 0.1f,  // Pleasure: high energy = slight pleasure boost
                        (energyRatio - 0.3f) * 0.2f,  // Arousal: energy directly affects arousal
                        0f                             // Dominance: energy doesn't affect dominance much
                    );
                }
                
                OnStatValueChanged?.Invoke();
            }
        }

        private int _hunger;
        public int hunger
        {
            get { return _hunger; }
            set
            {
                _hunger = Mathf.Clamp(value, 0, 100);
                
                // Hunger affects PAD: High hunger = low pleasure, high arousal (stress)
                if (emotionalState != null)
                {
                    float hungerRatio = _hunger / 100f; // 0-1 ratio
                    emotionalState.AdjustPAD(
                        -hungerRatio * 0.15f,          // Pleasure: hunger reduces pleasure
                        hungerRatio * 0.1f,            // Arousal: hunger increases stress/arousal
                        -hungerRatio * 0.05f           // Dominance: hunger slightly reduces confidence
                    );
                }
                
                OnStatValueChanged?.Invoke();
            }
        }

        private int _money;
        public int money
        {
            get { return _money; }
            set
            {
                _money = value;
                
                // Money affects PAD: More money = more pleasure and dominance
                if (emotionalState != null)
                {
                    float moneyRatio = Mathf.Clamp01(_money / 1000f); // Assuming comfortable money is 1000
                    emotionalState.AdjustPAD(
                        (moneyRatio - 0.5f) * 0.08f,   // Pleasure: financial security brings pleasure
                        0f,                            // Arousal: money doesn't directly affect arousal
                        (moneyRatio - 0.3f) * 0.12f    // Dominance: wealth increases confidence/dominance
                    );
                }
                
                OnStatValueChanged?.Invoke();
            }
        }

        private float _intimacy;
        public float intimacy
        {
            get { return _intimacy; }
            set
            {
                _intimacy = Mathf.Clamp01(value); // Keep in 0-1 range
                
                // Intimacy affects PAD and updates love triangle
                if (emotionalState != null)
                {
                    emotionalState.Intimacy = _intimacy; // Update love triangle directly
                    emotionalState.AdjustPAD(
                        _intimacy * 0.1f,              // Pleasure: intimacy brings joy
                        _intimacy * 0.05f,             // Arousal: intimacy creates excitement
                        _intimacy * 0.03f              // Dominance: close relationships boost confidence
                    );
                }
                
                OnStatValueChanged?.Invoke();
            }
        }

        private float _belonging;
        public float belonging
        {
            get { return _belonging; }
            set
            {
                _belonging = Mathf.Clamp01(value); // Keep in 0-1 range
                
                // Belonging affects PAD: Social connection = pleasure and dominance
                if (emotionalState != null)
                {
                    emotionalState.AdjustPAD(
                        _belonging * 0.08f,            // Pleasure: belonging brings contentment
                        0f,                            // Arousal: belonging is calming, not arousing
                        _belonging * 0.06f             // Dominance: social acceptance boosts confidence
                    );
                }
                
                OnStatValueChanged?.Invoke();
            }
        }

        private float _romantic;
        public float romantic
        {
            get { return _romantic; }
            set
            {
                _romantic = Mathf.Clamp01(value); // Keep in 0-1 range
                
                // Romantic feelings affect PAD: Romance = pleasure and arousal
                if (emotionalState != null)
                {
                    emotionalState.Passion = _romantic; // Update love triangle directly
                    emotionalState.AdjustPAD(
                        _romantic * 0.12f,             // Pleasure: romance brings happiness
                        _romantic * 0.15f,             // Arousal: romance is exciting
                        _romantic * 0.02f              // Dominance: slight confidence boost
                    );
                }
                
                OnStatValueChanged?.Invoke();
            }
        }

        // Add reference to emotional state
        [SerializeField] private EmotionalState emotionalState;

        // How much decay per frame? ....
        [SerializeField] private float timeToDecreaseHunger = 5f;
        [SerializeField] private float timeToDecreaseEnergy = 5f;
        private float timeLeftEnergy;
        private float timeLeftHunger;

        [SerializeField] private Billboard billboard;

        public delegate void StatValueChangedHandler();
        public event StatValueChangedHandler OnStatValueChanged;

        // Start is called before the first frame update
        void Start()
        {
            // Initialize emotional state if not assigned
            if (emotionalState == null)
            {
                emotionalState = new EmotionalState();
            }

            // Test case: NPC will likely eat
            hunger = 90;
            energy = 50;
            money = 500;
        }

        // Update Stats
        private void OnEnable()
        {
            OnStatValueChanged += UpdateDisplayText;
        }

        private void OnDisable()
        {
            OnStatValueChanged -= UpdateDisplayText;
        }

        //Updates Considerations per frame. 
        private void Update()
        {
            UpdateEnergy();
            UpdateHunger();
        }

        public void UpdateHunger()
        {
            if (timeLeftHunger > 0)
            {
                timeLeftHunger -= Time.deltaTime;
                return;
            }

            timeLeftHunger = timeToDecreaseHunger;
            hunger += 1;
        }

        // loops per time interval, then decreases consideration, then next frame does it again after time. 
        public void UpdateEnergy()
        {
            if (timeLeftEnergy > 0)
            {
                timeLeftEnergy -= Time.deltaTime;
                return;
            }

            timeLeftEnergy = timeToDecreaseEnergy;
            energy -= 1;
        }

        //  Update
        void UpdateDisplayText()
        {
            billboard.UpdateStatsText(energy, hunger, money);
        }

        // Method to get current emotional state (for considerations to use)
        public EmotionalState GetEmotionalState()
        {
            return emotionalState;
        }
    }
}