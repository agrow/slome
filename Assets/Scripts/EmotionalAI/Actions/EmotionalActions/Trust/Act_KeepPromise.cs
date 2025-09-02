using UnityEngine;

namespace TL.EmotionalAI
{
    [CreateAssetMenu(menuName="EmotionalAI/Actions/Keep Promise (follow through)")]
    public class Act_KeepPromise : EmotionalAction
    {
        public override void Execute(EmotionModel emo, Animator animator=null) { base.Execute(emo, animator); }
    }
}
