using UnityEngine;

namespace TL.EmotionalAI
{
    public class EmotionalAIKeyInput : MonoBehaviour
    {
        public EmotionAdapter adapter;
        [Range(0f,1f)] public float intensity01 = 0.5f;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) adapter.OnPlayerAction(PlayerAction.ComplimentLooks, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha2)) adapter.OnPlayerAction(PlayerAction.Flirt, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha3)) adapter.OnPlayerAction(PlayerAction.Hug, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha4)) adapter.OnPlayerAction(PlayerAction.KissQuick, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha5)) adapter.OnPlayerAction(PlayerAction.KissDeep, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha6)) adapter.OnPlayerAction(PlayerAction.Apology, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha7)) adapter.OnPlayerAction(PlayerAction.TeasePlayful, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha8)) adapter.OnPlayerAction(PlayerAction.TeaseHarsh, intensity01);
            if (Input.GetKeyDown(KeyCode.Alpha9)) adapter.OnPlayerAction(PlayerAction.GiftLarge, intensity01);
        }
    }
}
