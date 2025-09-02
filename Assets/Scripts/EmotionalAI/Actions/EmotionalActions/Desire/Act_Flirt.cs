using UnityEngine;

namespace TL.EmotionalAI
{
    [CreateAssetMenu(menuName="EmotionalAI/Actions/Flirt Playfully")]
    public class Act_Flirt : EmotionalAction
    {
        public override void Execute(EmotionModel emo, Animator animator=null) { base.Execute(emo, animator); }
    }
}
