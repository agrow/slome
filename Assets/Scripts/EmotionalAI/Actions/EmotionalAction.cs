using UnityEngine;
using System.Collections.Generic;
using TL.Personality;

namespace TL.EmotionalAI
{
    /* Purpose Statement: 
    ScriptableObject action: has considerations[] → multiply (AND-like) → Dave Mark makeup → utilityCurve → score.
    Execute() hook for animation/dialogue, cooldown, weight.
    */
    public abstract class EmotionalAction : ScriptableObject
    {
        public string Name;
        private float _score;
        public float score { get => _score; protected set => _score = Mathf.Clamp01(value); }

        public EmotionalConsideration[] considerations; // 2–3 in v1
        [SerializeField] protected AnimationCurve utilityCurve = AnimationCurve.Linear(0,0,1,1);
        [Range(0f,2f)] public float weight = 1f;
        public string animationTrigger;
        public float cooldown = 0f;
        float cooldownUntil;

        [Header("Personality Bias (optional)")]
        [Tooltip("Pick a preset to auto-fill pos/neg axis multipliers if list is empty.")]
        public BiasPreset BiasPreset = BiasPreset.None;

        [Tooltip("If empty at runtime, we auto-fill from BiasPreset; otherwise we use your custom entries.")]
        public List<PersonalityBiasEntry> PersonalityBiases = new();


        /// <summary>
        /// Call this once before scoring to ensure PersonalityBiases is initialized.
        /// </summary>
        public void EnsurePersonalityBiasesInitialized()
        {
            if ((PersonalityBiases == null || PersonalityBiases.Count == 0) && BiasPreset != BiasPreset.None)
            {
                PersonalityBiases = PersonalityBiasPresets.Get(BiasPreset);
            }
        }

        public virtual void Awake(){ score = 0f; }

        public float ScoreAction(EmotionModel emo)
        {
            if (Time.time < cooldownUntil) { score = 0f; return 0f; }
            if (considerations == null || considerations.Length == 0) { score = 0.5f * weight; return score; }

            // Multiply (AND-like)
            float product = 1f;
            for (int i=0;i<considerations.Length;i++)
            {
                float c = Mathf.Clamp01(considerations[i].ScoreConsideration(emo));
                product *= c;
                if (product <= 0f) { score = 0f; return 0f; }
            }

            // Dave Mark makeup: p + (1-p)*p*(1-1/n)
            int n = considerations.Length;
            float modFactor = 1f - (1f / n);
            float combined = product + (1f - product) * modFactor * product;

            float utility = Mathf.Clamp01(utilityCurve.Evaluate(Mathf.Clamp01(combined)));
            score = Mathf.Clamp01(utility * weight);
            return score;
        }

        public virtual void Execute(EmotionModel emo, Animator animator=null)
        {
            if (animator && !string.IsNullOrEmpty(animationTrigger)) animator.SetTrigger(animationTrigger);
            if (cooldown>0f) cooldownUntil = Time.time + cooldown;
            Debug.Log($"[EmotionalAI] Execute {Name}");
        }
    }
}
