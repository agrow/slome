using System;
using UnityEngine;

/*  Purpose Statement:
    EmotionalCore.cs — Defines PAD, Triangle (relationship), 
    PlayerAction, Intent, EmotionOctant
*/
namespace TL.EmotionalAI
{
    [Serializable] public struct PAD { public float P, A, D; }     // 0..1
    [Serializable] public struct Triangle { public float I, Pa, C; } // 0..1

    public enum PlayerAction
    {
        // Affection
        ComplimentLooks, Hug, HoldHands, Comfort, Encourage, GiftSmall,

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
        KeepPromise, Reassure, Protect, Shelter, Steady,

        // Conflict
        TeaseHarsh, Confront, Criticize, Withdraw, Demand,

        // Manipulation
        GiftLarge, GuiltTrip, Flatter, Pressure, Withhold
    }

    public enum Intent { Affection, Desire, Bonding, Trust, Respect, Playfulness, Security, Conflict, Manipulation }

    public enum EmotionOctant { Sad, Resigned, Anxious, Angry, Tender, ProudSecure, Awe, Joy }
}