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
            Debug.Log($"{npc.name}: Executing Sleep action");
            // Dependency Injection: your class doesn't have a local reference to a GameObject, but you can still require it.
            npc.DoSleep(3);
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            if (npc.context != null && npc.context.home != null)
            {
                RequiredDestination = npc.context.home.transform; // only one home right now in the game.
                Debug.Log($"{npc.name}: Sleep destination set to home: {RequiredDestination.name}");
            }
            else
            {
                // Fallback: stay in place if no home found
                RequiredDestination = npc.transform;
                Debug.LogWarning($"{npc.name}: No home found in context, staying in place for sleep");
            }
            
            // Note: The NPCController FSM will handle agent.SetDestination() in HandleDecideState()
            // We don't need to set npc.mover.destination anymore
        }
    }
}