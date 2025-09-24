using TL.EmotionalAI;

namespace TL.EmotionalAI
{
    /// <summary>
    /// Defines intensity multipliers for player actions.
    /// Higher values = stronger emotional impact.
    /// Conflict/Manipulation actions have higher intensity for negative impact.
    /// </summary>
    public static class PlayerActionIntensity
    {
        public static float Get(PlayerAction action)
        {
            return action switch
            {
                // AFFECTION - Gentle positive actions
                PlayerAction.ComplimentLooks => 1.2f,
                PlayerAction.Hug => 1.5f,
                PlayerAction.HoldHands => 1.0f,
                PlayerAction.Comfort => 1.3f,
                PlayerAction.Encourage => 1.2f,
                PlayerAction.GiftSmall => 0.9f,

                // DESIRE - Strong positive
                PlayerAction.KissQuick => 1.8f,
                PlayerAction.KissDeep => 2.2f,
                PlayerAction.Flirt => 1.4f,
                PlayerAction.Seduce => 2.0f,
                PlayerAction.LongFor => 1.2f,

                // BONDING - Moderate positive
                PlayerAction.InviteActivity => 1.5f,
                PlayerAction.ShareStory => 1.2f,
                PlayerAction.Reminisce => 1.0f,
                PlayerAction.Celebrate => 1.6f,
                PlayerAction.Support => 1.3f,

                // TRUST - Moderate positive
                PlayerAction.Apology => 1.5f,
                PlayerAction.Confide => 1.3f,
                PlayerAction.Forgive => 1.6f,
                PlayerAction.AskHelp => 1.0f,
                PlayerAction.Promise => 1.2f,

                // RESPECT - Gentle positive
                PlayerAction.ComplimentSkill => 1.2f,
                PlayerAction.Acknowledge => 1.0f,
                PlayerAction.Admire => 1.3f,
                PlayerAction.Defend => 1.8f,
                PlayerAction.Praise => 1.2f,

                // PLAYFULNESS - Light to moderate
                PlayerAction.TeasePlayful => 0.9f,
                PlayerAction.Joke => 0.8f,
                PlayerAction.Challenge => 1.2f,
                PlayerAction.Surprise => 1.3f,
                PlayerAction.Trick => 1.0f,

                // SECURITY - Moderate positive
                PlayerAction.KeepPromise => 1.5f,
                PlayerAction.Reassure => 1.3f,
                PlayerAction.Protect => 1.8f,
                PlayerAction.Shelter => 1.2f,
                PlayerAction.Steady => 1.0f,

                // CONFLICT - VERY HIGH INTENSITY NEGATIVE
                PlayerAction.TeaseHarsh => 2.5f,
                PlayerAction.Confront => 3.0f,
                PlayerAction.Criticize => 2.8f,
                PlayerAction.Withdraw => 2.2f,
                PlayerAction.Demand => 2.5f,

                // MANIPULATION - EXTREMELY HIGH INTENSITY NEGATIVE
                PlayerAction.GiftLarge => 3.2f,      // Overwhelming/manipulative
                PlayerAction.GuiltTrip => 3.5f,     // Very damaging
                PlayerAction.Flatter => 2.0f,       // Moderate manipulation
                PlayerAction.Pressure => 3.0f,      // High pressure
                PlayerAction.Withhold => 2.8f,      // Emotional withholding

                _ => 1.0f // Default intensity
            };
        }
    }
}