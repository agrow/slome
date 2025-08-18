using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "RomanticConsideration", menuName = "UtilityAI/Considerations/RomanticConsideration")]

    public class RomanticConsiderationConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(NPCController npc)
        {
            score = responseCurve.Evaluate(Mathf.Clamp01(npc.stats.romantic)); // max 1 romantic
            return score;
        }
    }
}
