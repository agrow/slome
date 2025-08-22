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
        ComplimentLooks, ComplimentSkill, Flirt, HoldHands, Hug,
        KissQuick, KissDeep, GiftSmall, GiftLarge, Apology,
        TeasePlayful, TeaseHarsh, KeepPromise, InviteActivity
    }

    public enum Intent { Affection, Desire, Bonding, Trust, Respect, Playfulness, Security, Conflict, Manipulation }

    public enum EmotionOctant { Sad, Resigned, Anxious, Angry, Tender, ProudSecure, Awe, Joy }
}
