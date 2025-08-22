using System;

namespace TL.EmotionalAI
{
    public static class PlayerActionBus
    {
        public static event Action<PlayerAction,float> OnPlayerAction; // action, intensity01
        public static void Raise(PlayerAction act, float intensity01 = 0.5f) => OnPlayerAction?.Invoke(act, intensity01);
    }
}
