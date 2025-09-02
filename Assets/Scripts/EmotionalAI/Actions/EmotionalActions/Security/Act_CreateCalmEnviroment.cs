using UnityEngine;

namespace TL.EmotionalAI
{
    [CreateAssetMenu(menuName="EmotionalAI/Actions/Create Calm Enviroment")]
    public class Act_CreateCalmEnviorment : EmotionalAction
    {
        public override void Execute(EmotionModel emo, Animator animator=null) { base.Execute(emo, animator); }
    }
}
