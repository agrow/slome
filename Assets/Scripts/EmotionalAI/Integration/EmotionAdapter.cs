using UnityEngine;

namespace TL.EmotionalAI
{
    public class EmotionAdapter : MonoBehaviour
    {
        [SerializeField] private EmotionModel emotion;
        [SerializeField] private EmotionBrain brain;
        [SerializeField] private bool autoAct = true;

        // Call this from your gameplay/UI when the player chooses something.
        public void OnPlayerAction(PlayerAction action, float intensity01 = 0.7f)
        {
            // Action => ______ => Change PAD
            emotion.ApplyPlayerAction(action, intensity01);
            
            // UtilityAI Based off of new State
            brain.DecideBestEmotionalAction();

            // what is autoAct? seems like we are just executing the action. 
            if (autoAct) brain.ExecuteBest();
        }
    }
}
