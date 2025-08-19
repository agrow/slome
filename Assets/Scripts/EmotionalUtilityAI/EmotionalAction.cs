using TL.Core;
using UnityEngine;

namespace TL.UtilityAI
{
    public abstract class EmotionalAction : ScriptableObject
    {
        public string Name;
        
        private float _score;
        public float score 
        {
            get { return _score; }
            set { this._score = Mathf.Clamp01(value); }
        }
        // considerations array 
        public Consideration[] considerations; 
        
        public Transform RequiredDestination { get; protected set; }

        // Abstract methods for execution
        public abstract void Execute(NPCController npc);
        public abstract void SetRequiredDestination(NPCController npc);
        
        public virtual void Awake()
        {
            score = 0;
        }
    }
}