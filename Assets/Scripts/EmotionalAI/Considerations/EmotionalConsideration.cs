using UnityEngine;

namespace TL.EmotionalAI
{
    // Purpose Statement: base class (ScriptableObject) with an AnimationCurve mapping raw input → [0..1].
    public abstract class EmotionalConsideration : ScriptableObject
    {
        public string Name;
        [SerializeField] protected AnimationCurve responseCurve = AnimationCurve.Linear(0,0,1,1);
        // Right now the its just a linear graph is it customizable? 
        private float _score;
        public float score { get => _score; protected set => _score = Mathf.Clamp01(value); }

        public abstract float ScoreConsideration(EmotionModel emo);
    }
}
