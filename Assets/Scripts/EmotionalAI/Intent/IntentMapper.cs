namespace TL.EmotionalAI
{
    // Purpose Statement: 1:1 table from PlayerAction → Intent.
    public static class IntentMapper
    {
        public static Intent Map(PlayerAction a) => a switch
        {
            PlayerAction.ComplimentLooks => Intent.Affection,
            PlayerAction.ComplimentSkill => Intent.Respect,
            PlayerAction.Flirt           => Intent.Affection,
            PlayerAction.HoldHands       => Intent.Affection,
            PlayerAction.Hug             => Intent.Affection,
            PlayerAction.KissQuick       => Intent.Desire,
            PlayerAction.KissDeep        => Intent.Desire,
            PlayerAction.GiftSmall       => Intent.Affection,
            PlayerAction.GiftLarge       => Intent.Manipulation,
            PlayerAction.Apology         => Intent.Trust,
            PlayerAction.TeasePlayful    => Intent.Playfulness,
            PlayerAction.TeaseHarsh      => Intent.Conflict,
            PlayerAction.KeepPromise     => Intent.Security,
            PlayerAction.InviteActivity  => Intent.Bonding,
            _ => Intent.Bonding
        };
    }
}
