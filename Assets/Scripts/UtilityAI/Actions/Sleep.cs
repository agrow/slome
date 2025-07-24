using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Sleep", menuName = "UtilityAI/Actions/Sleep")]
    public class Sleep : Action
    {
        public override void Execute(NPCController npc)
        {
            // Dependacy Injection: your class doesn't have a local refernence to an GameObject, but you can still require it. 
            npc.DoSleep(3);
        }
    }
}