using TL.Core;
using UnityEngine;

namespace TL.UtilityAI
{
    public enum EmotionalActionType
    {
        Social,
        Romantic,
        Assertive,
        Nurturing
    }

    public abstract class EmotionalAction : ScriptableObject
    {
        public string Name;
        public EmotionalActionType actionType;
        
        private float _score;
        public float score 
        {
            get { return _score; }
            set { this._score = Mathf.Clamp01(value); }
        }

        public Consideration[] considerations;
        public Transform RequiredDestination { get; protected set; }

        // Emotional actions might have different execution patterns
        public abstract void Execute(NPCController npc);
        public abstract void SetRequiredDestination(NPCController npc);
        
        // New: Emotional intensity factor
        [Range(0.1f, 2.0f)]
        public float emotionalIntensity = 1.0f;
        
        public virtual void Awake()
        {
            score = 0;
        }
    }
}