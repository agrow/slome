namespace TL.EmotionalAI
{
    // Purpose Statement: 1:1 table from PlayerAction → Intent.
    public static class IntentMapper
    {
        public static Intent Map(PlayerAction a) => a switch
        {
            // Affection
            PlayerAction.ComplimentLooks => Intent.Affection,
            PlayerAction.Hug => Intent.Affection,
            PlayerAction.HoldHands => Intent.Affection,
            PlayerAction.Comfort => Intent.Affection,
            PlayerAction.Encourage => Intent.Affection,
            PlayerAction.GiftSmall => Intent.Affection,

            // Desire
            PlayerAction.KissQuick => Intent.Desire,
            PlayerAction.KissDeep => Intent.Desire,
            PlayerAction.Flirt => Intent.Desire,
            PlayerAction.Seduce => Intent.Desire,
            PlayerAction.LongFor => Intent.Desire,

            // Bonding
            PlayerAction.InviteActivity => Intent.Bonding,
            PlayerAction.ShareStory => Intent.Bonding,
            PlayerAction.Reminisce => Intent.Bonding,
            PlayerAction.Celebrate => Intent.Bonding,
            PlayerAction.Support => Intent.Bonding,

            // Trust
            PlayerAction.Apology => Intent.Trust,
            PlayerAction.Confide => Intent.Trust,
            PlayerAction.Forgive => Intent.Trust,
            PlayerAction.AskHelp => Intent.Trust,
            PlayerAction.Promise => Intent.Trust,

            // Respect
            PlayerAction.ComplimentSkill => Intent.Respect,
            PlayerAction.Acknowledge => Intent.Respect,
            PlayerAction.Admire => Intent.Respect,
            PlayerAction.Defend => Intent.Respect,
            PlayerAction.Praise => Intent.Respect,

            // Playfulness
            PlayerAction.TeasePlayful => Intent.Playfulness,
            PlayerAction.Joke => Intent.Playfulness,
            PlayerAction.Challenge => Intent.Playfulness,
            PlayerAction.Surprise => Intent.Playfulness,
            PlayerAction.Trick => Intent.Playfulness,

            // Security
            PlayerAction.KeepPromise => Intent.Security,
            PlayerAction.Reassure => Intent.Security,
            PlayerAction.Protect => Intent.Security,
            PlayerAction.Shelter => Intent.Security,
            PlayerAction.Steady => Intent.Security,

            // Conflict
            PlayerAction.TeaseHarsh => Intent.Conflict,
            PlayerAction.Confront => Intent.Conflict,
            PlayerAction.Criticize => Intent.Conflict,
            PlayerAction.Withdraw => Intent.Conflict,
            PlayerAction.Demand => Intent.Conflict,

            // Manipulation
            PlayerAction.GiftLarge => Intent.Manipulation,
            PlayerAction.GuiltTrip => Intent.Manipulation,
            PlayerAction.Flatter => Intent.Manipulation,
            PlayerAction.Pressure => Intent.Manipulation,
            PlayerAction.Withhold => Intent.Manipulation,

            _ => Intent.Bonding
        };
    }
}
