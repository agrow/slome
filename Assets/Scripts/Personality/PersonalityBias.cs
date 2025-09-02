// Purpose: Dropdown presets per action "archetype" so you don't hand-enter numbers.
// Pick a BiasPreset on an action; if its PersonalityBiases list is empty at runtime,
// we'll auto-fill with these values (pos/neg per axis). No editor tooling needed.
//
// posMultiplier applies to E/N/F/J/A; negMultiplier applies to I/S/T/P/Turbulent.
// Keep values small so PAD remains the main driver.

using System.Collections.Generic;

namespace TL.Personality
{
    /// <summary> Choose one preset per action in the Inspector. </summary>
    public enum BiasPreset
    {
        None = 0,

        // Affection
        ComplimentLooks, Hug, HoldHands, Comfort, Encourage,

        // Desire
        KissQuick, KissDeep, Flirt, Seduce, LongFor,

        // Bonding
        InviteActivity, ShareStory, Reminisce, Celebrate, Support,

        // Trust
        Apology, Confide, Forgive, AskHelp, Promise,

        // Respect
        ComplimentSkill, Acknowledge, Admire, Defend, Praise,

        // Playfulness
        TeasePlayful, Joke, Challenge, Surprise, Trick,

        // Security
        KeepPromise, Reassure, Protect, Deflect, Anchor,

        // Conflict / Manipulative
        TeaseHarsh, Confront, Criticize, Withdraw, Demand,
        Manipulation, GiftLarge, GuiltTrip, Flatter, Pressure, Withhold
    }

    public static class PersonalityBiasPresets
    {
        public static PersonalityBiasEntry Bias(Axis a, float pos, float neg)
            => new PersonalityBiasEntry { axis = a, posMultiplier = pos, negMultiplier = neg };

        /// <summary>
        /// Returns the recommended bias list for the given preset.
        /// If a preset doesn't need personality, return an empty list.
        /// </summary>
        public static List<PersonalityBiasEntry> Get(BiasPreset preset)
        {
            switch (preset)
            {
                // ---------- AFFECTION ----------
                case BiasPreset.ComplimentLooks:
                    return new() { Bias(Axis.Energy,1.08f,1.00f), Bias(Axis.Nature,1.08f,0.98f) };
                case BiasPreset.Hug:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Nature,1.10f,0.95f) };
                case BiasPreset.HoldHands:
                    return new() { Bias(Axis.Energy,1.10f,0.95f) };
                case BiasPreset.Comfort:
                    return new() { Bias(Axis.Nature,1.12f,0.98f) };
                case BiasPreset.Encourage:
                    return new() { Bias(Axis.Nature,1.08f,1.00f), Bias(Axis.Energy,1.06f,1.00f) };

                // ---------- DESIRE ----------
                case BiasPreset.KissQuick:
                    return new() { Bias(Axis.Energy,1.10f,0.95f) };
                case BiasPreset.KissDeep:
                    return new() { Bias(Axis.Identity,1.10f,0.95f) };
                case BiasPreset.Flirt:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Identity,1.08f,0.98f) };
                case BiasPreset.Seduce:
                    return new() { Bias(Axis.Identity,1.10f,0.95f), Bias(Axis.Energy,1.06f,1.00f) };
                case BiasPreset.LongFor:
                    return new() { Bias(Axis.Energy,0.98f,1.06f), Bias(Axis.Nature,1.06f,1.00f) };

                // ---------- BONDING ----------
                case BiasPreset.InviteActivity:
                    return new() { Bias(Axis.Mind,0.98f,1.10f),  Bias(Axis.Tactics,1.04f,1.02f) };
                case BiasPreset.ShareStory:
                    return new() { Bias(Axis.Mind,1.10f,1.00f),  Bias(Axis.Nature,1.06f,1.00f) };
                case BiasPreset.Reminisce:
                    return new() { Bias(Axis.Mind,0.98f,1.10f),  Bias(Axis.Nature,1.06f,1.00f) };
                case BiasPreset.Celebrate:
                    return new() { Bias(Axis.Energy,1.10f,0.95f) };
                case BiasPreset.Support:
                    return new() { Bias(Axis.Nature,1.06f,1.00f), Bias(Axis.Tactics,1.06f,1.00f) };

                // ---------- TRUST ----------
                case BiasPreset.Apology:
                    return new() { Bias(Axis.Identity,0.98f,1.10f), Bias(Axis.Nature,1.06f,1.00f) };
                case BiasPreset.Confide:
                    return new() { Bias(Axis.Nature,1.10f,0.98f) };
                case BiasPreset.Forgive:
                    return new() { Bias(Axis.Nature,1.08f,1.00f), Bias(Axis.Identity,1.02f,1.04f) };
                case BiasPreset.AskHelp:
                    return new() { Bias(Axis.Identity,0.98f,1.08f) };
                case BiasPreset.Promise:
                    return new() { Bias(Axis.Tactics,1.10f,0.95f) };

                // ---------- RESPECT ----------
                case BiasPreset.ComplimentSkill:
                    return new() { Bias(Axis.Nature,1.06f,1.00f) };
                case BiasPreset.Acknowledge:
                    return new() { Bias(Axis.Nature,1.08f,1.00f) };
                case BiasPreset.Admire:
                    return new() { Bias(Axis.Nature,1.06f,1.00f) };
                case BiasPreset.Defend:
                    return new() { Bias(Axis.Nature,1.02f,1.08f), Bias(Axis.Identity,1.06f,1.00f) };
                case BiasPreset.Praise:
                    return new() { Bias(Axis.Energy,1.06f,1.00f), Bias(Axis.Nature,1.06f,1.00f) };

                // ---------- PLAYFULNESS ----------
                case BiasPreset.TeasePlayful:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Tactics,0.98f,1.10f) };
                case BiasPreset.Joke:
                    return new() { Bias(Axis.Energy,1.12f,0.95f) };
                case BiasPreset.Challenge:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Tactics,0.98f,1.10f) };
                case BiasPreset.Surprise:
                    return new() { Bias(Axis.Energy,1.10f,0.95f) };
                case BiasPreset.Trick:
                    return new() { Bias(Axis.Tactics,0.98f,1.10f) };

                // ---------- SECURITY ----------
                case BiasPreset.KeepPromise:
                    return new() { Bias(Axis.Tactics,1.10f,0.95f) };
                case BiasPreset.Reassure:
                    return new() { Bias(Axis.Identity,0.95f,1.12f), Bias(Axis.Nature,1.08f,1.00f) };
                case BiasPreset.Protect:
                    return new() { Bias(Axis.Identity,1.10f,0.95f) };
                case BiasPreset.Deflect:
                    return new() { Bias(Axis.Nature,0.98f,1.10f) };
                case BiasPreset.Anchor:
                    return new() { Bias(Axis.Mind,1.06f,1.00f),   Bias(Axis.Nature,1.06f,1.00f) };

                // ---------- CONFLICT / MANIPULATIVE ----------
                case BiasPreset.TeaseHarsh:
                    return new() { Bias(Axis.Nature,0.98f,1.08f), Bias(Axis.Energy,0.98f,1.08f) };
                case BiasPreset.Confront:
                    return new() { Bias(Axis.Identity,1.10f,0.95f), Bias(Axis.Nature,0.98f,1.10f) };
                case BiasPreset.Criticize:
                    return new() { Bias(Axis.Nature,0.98f,1.10f) };
                case BiasPreset.Withdraw:
                    return new() { Bias(Axis.Energy,0.95f,1.10f) };
                case BiasPreset.Demand:
                    return new() { Bias(Axis.Identity,1.10f,0.95f) };
                case BiasPreset.Manipulation:
                    return new() { Bias(Axis.Nature,0.98f,1.10f) };
                case BiasPreset.GiftLarge:
                    return new() { Bias(Axis.Identity,1.06f,1.00f), Bias(Axis.Nature,0.98f,1.08f) };
                case BiasPreset.GuiltTrip:
                    return new() { Bias(Axis.Energy,0.98f,1.08f),   Bias(Axis.Nature,0.98f,1.08f) };
                case BiasPreset.Flatter:
                    return new() { Bias(Axis.Energy,1.06f,1.00f),   Bias(Axis.Identity,1.04f,1.00f) };
                case BiasPreset.Pressure:
                    return new() { Bias(Axis.Identity,1.10f,0.95f) };
                case BiasPreset.Withhold:
                    return new() { Bias(Axis.Energy,0.98f,1.08f),   Bias(Axis.Identity,1.02f,1.06f) };
            }

            return new List<PersonalityBiasEntry>(); // None or fallback
        }
    }
}
