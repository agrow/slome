using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "EnergyConsideration", menuName = "UtilityAI/Considerations/Energy Consideration")]

    public class EnergyConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        // Need to implement other scripts....
        public override float ScoreConsideration(NPCController npc)
        {
            score = responseCurve.Evaluate(Mathf.Clamp01(npc.stats.energy / 100f)); // max 100 hunger, divide by max. 
            return score;
        }
    }
}
