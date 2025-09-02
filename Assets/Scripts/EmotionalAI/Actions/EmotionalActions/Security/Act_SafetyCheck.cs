using UnityEngine;

namespace TL.EmotionalAI
{
    [CreateAssetMenu(menuName="EmotionalAI/Actions/Safety Check (are you ok?)")]
    public class Act_SafetyCheck : EmotionalAction
    {
        public override void Execute(EmotionModel emo, Animator animator=null) { base.Execute(emo, animator); }
    }
}
