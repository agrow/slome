// Purpose: Dropdown presets per action "archetype" so you don't hand-enter numbers.
// Pick a BiasPreset on an action; if its PersonalityBiases list is empty at runtime,
// we'll auto-fill with these values (pos/neg per axis). No editor tooling needed.

using System.Collections.Generic;

namespace TL.Personality
{
    /// <summary>
    /// Choose one preset per action in the Inspector.
    /// </summary>
    public enum BiasPreset
    {
        None = 0,

        // Affection
        HugWarm, CuddleLeanIn, HoldHands, GentleTouch, WordsOfComfort,
        // Desire
        FlirtPlayful, KissQuick, KissDeep, ComplimentAppearance, PlayfulTease,
        // Bonding
        SharePersonalStory, AskAboutDayDeep, PlanFutureLight, ReminisceMemory, InsideJoke,
        // Trust
        KeepPromise, ConfideVulnerability, AskConsent, AdmitFault, ShieldFromOverwhelm,
        // Respect
        RespectBoundary, GiveSpace, ApologizeDirect, InviteChoice, CreditContribution,
        // Playfulness
        LightJoke, MiniGame, ShareFunnyThing, MockSeriousCompliment, SpontaneousDetour,
        // Security
        ReassureCommitment, SafetyCheck, CreateCalmEnv, QuietPresence, ProtectiveGesture,
        // Conflict Repair
        NameTheTension, OfferRepairPlan, TimeOutMutual, ReflectBack, LightenAfterRepair
    }

    public static class PersonalityBiasPresets
    {
        public static PersonalityBiasEntry Bias(Axis a, float pos, float neg)
            => new PersonalityBiasEntry { axis = a, posMultiplier = pos, negMultiplier = neg };

        /// <summary>
        /// Returns the recommended bias list for the given preset.
        /// If a preset doesn't need personality, return empty list.
        /// </summary>
        public static List<PersonalityBiasEntry> Get(BiasPreset preset)
        {
            switch (preset)
            {
                // ---------- AFFECTION ----------
                case BiasPreset.HugWarm:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Nature,1.10f,0.95f) };
                case BiasPreset.CuddleLeanIn:
                    return new() { Bias(Axis.Energy,1.05f,1.00f), Bias(Axis.Nature,1.10f,0.95f) };
                case BiasPreset.HoldHands:
                    return new() { Bias(Axis.Energy,1.10f,0.95f) };
                case BiasPreset.GentleTouch:
                    return new() { Bias(Axis.Nature,1.10f,0.95f) };
                case BiasPreset.WordsOfComfort:
                    return new() { Bias(Axis.Nature,1.12f,0.98f) };

                // ---------- DESIRE ----------
                case BiasPreset.FlirtPlayful:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Identity,1.08f,0.98f) };
                case BiasPreset.KissQuick:
                    return new() { Bias(Axis.Energy,1.10f,0.95f) };
                case BiasPreset.KissDeep:
                    return new() { Bias(Axis.Identity,1.10f,0.95f) };
                case BiasPreset.ComplimentAppearance:
                    return new() { Bias(Axis.Energy,1.08f,1.00f), Bias(Axis.Nature,1.08f,0.98f) };
                case BiasPreset.PlayfulTease:
                    return new() { Bias(Axis.Energy,1.08f,0.98f), Bias(Axis.Tactics,0.98f,1.10f) };

                // ---------- BONDING ----------
                case BiasPreset.SharePersonalStory:
                    return new() { Bias(Axis.Mind,1.10f,1.00f), Bias(Axis.Nature,1.08f,0.98f) };
                case BiasPreset.AskAboutDayDeep:
                    return new() { Bias(Axis.Mind,0.98f,1.10f), Bias(Axis.Nature,1.08f,0.98f) };
                case BiasPreset.PlanFutureLight:
                    return new() { Bias(Axis.Mind,1.10f,0.98f), Bias(Axis.Tactics,1.10f,0.95f) };
                case BiasPreset.ReminisceMemory:
                    return new() { Bias(Axis.Mind,0.98f,1.10f), Bias(Axis.Nature,1.08f,0.98f) };
                case BiasPreset.InsideJoke:
                    return new() { Bias(Axis.Tactics,0.98f,1.10f) };

                // ---------- TRUST ----------
                case BiasPreset.KeepPromise:
                    return new() { Bias(Axis.Tactics,1.10f,0.95f) };
                case BiasPreset.ConfideVulnerability:
                    return new() { Bias(Axis.Nature,1.10f,0.98f), Bias(Axis.Identity,0.98f,1.10f) };
                case BiasPreset.AskConsent:
                    return new() { Bias(Axis.Nature,1.00f,1.10f) };
                case BiasPreset.AdmitFault:
                    return new() { Bias(Axis.Identity,0.98f,1.10f) };
                case BiasPreset.ShieldFromOverwhelm:
                    return new() { Bias(Axis.Nature,1.10f,0.98f) };

                // ---------- RESPECT ----------
                case BiasPreset.RespectBoundary:
                    return new() { Bias(Axis.Nature,0.98f,1.10f), Bias(Axis.Tactics,1.08f,0.98f) };
                case BiasPreset.GiveSpace:
                    return new() { Bias(Axis.Nature,0.98f,1.10f), Bias(Axis.Tactics,1.08f,0.98f) };
                case BiasPreset.ApologizeDirect:
                    return new() { Bias(Axis.Nature,1.08f,1.00f), Bias(Axis.Identity,0.98f,1.10f) };
                case BiasPreset.InviteChoice:
                    return new() { Bias(Axis.Nature,0.98f,1.10f), Bias(Axis.Tactics,0.98f,1.10f) };
                case BiasPreset.CreditContribution:
                    return new() { Bias(Axis.Nature,1.10f,0.98f) };

                // ---------- PLAYFULNESS ----------
                case BiasPreset.LightJoke:
                    return new() { Bias(Axis.Energy,1.12f,0.95f) };
                case BiasPreset.MiniGame:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Tactics,0.98f,1.10f) };
                case BiasPreset.ShareFunnyThing:
                    return new() { Bias(Axis.Energy,1.08f,0.98f), Bias(Axis.Mind,1.08f,1.00f) };
                case BiasPreset.MockSeriousCompliment:
                    return new() { Bias(Axis.Tactics,0.98f,1.10f) };
                case BiasPreset.SpontaneousDetour:
                    return new() { Bias(Axis.Energy,1.10f,0.95f), Bias(Axis.Tactics,0.98f,1.10f) };

                // ---------- SECURITY ----------
                case BiasPreset.ReassureCommitment:
                    return new() { Bias(Axis.Identity,0.95f,1.12f), Bias(Axis.Nature,1.08f,1.00f) };
                case BiasPreset.SafetyCheck:
                    return new() { Bias(Axis.Identity,0.98f,1.10f) };
                case BiasPreset.CreateCalmEnv:
                    return new() { Bias(Axis.Nature,0.98f,1.10f) };
                case BiasPreset.QuietPresence:
                    return new() { Bias(Axis.Nature,1.10f,0.98f) };
                case BiasPreset.ProtectiveGesture:
                    return new() { Bias(Axis.Identity,1.10f,0.95f) };

                // ---------- CONFLICT REPAIR ----------
                case BiasPreset.NameTheTension:
                    return new() { Bias(Axis.Nature,0.98f,1.10f) };
                case BiasPreset.OfferRepairPlan:
                    return new() { Bias(Axis.Nature,1.00f,1.10f), Bias(Axis.Tactics,1.10f,0.95f) };
                case BiasPreset.TimeOutMutual:
                    return new() { Bias(Axis.Identity,1.06f,1.06f) };
                case BiasPreset.ReflectBack:
                    return new() { Bias(Axis.Nature,1.12f,0.98f) };
                case BiasPreset.LightenAfterRepair:
                    return new() { Bias(Axis.Energy,1.08f,0.98f), Bias(Axis.Tactics,0.98f,1.10f) };
            }

            return new(); // None or fallback
        }
    }
}
