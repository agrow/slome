using UnityEngine;

namespace TL.EmotionalAI
{
    [CreateAssetMenu(menuName="EmotionalAI/Actions/Protective Gesture")]
    public class Act_ProtectiveGesture : EmotionalAction
    {
        public override void Execute(EmotionModel emo, Animator animator=null) { base.Execute(emo, animator); }
    }
}
