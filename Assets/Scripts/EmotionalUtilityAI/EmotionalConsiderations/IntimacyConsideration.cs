using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "IntimacyConsideration", menuName = "UtilityAI/Considerations/Intimacy Consideration")]

    public class IntimacyConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(NPCController npc)
        {
            score = responseCurve.Evaluate(Mathf.Clamp01(npc.stats.intimacy)); // max 1 intimacy
            return score;
        }
    }
}
