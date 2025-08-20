using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "BelongingConsideration", menuName = "UtilityAI/Considerations/Belonging Consideration")]

    public class BelongingConsideration : Consideration 
    {
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(NPCController npc)  
        {
            score = responseCurve.Evaluate(Mathf.Clamp01(npc.stats.belonging)); // max 1 belonging
            return score;
        }
    }
}
