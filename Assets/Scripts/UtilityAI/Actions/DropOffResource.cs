using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "DropOffResource", menuName = "UtilityAI/Actions/DropOffResource")]
    // Drop off resources instantaneously for money
    public class DropOffResource : Action
    {
        public override void Execute(NPCController npc)
        {
            // Dependency Injection: your class doesn't have a local reference to a GameObject, but you can still require it.
            // Don't need a coroutine since time isn't concerned with this design

            Debug.Log($"{npc.name}: I dropped off resources! :)");
            // Logic for updating everything involved with dropping off resources

            if (npc.stats != null)
            {
                int oldMoney = npc.stats.money;
                npc.stats.money += 20; // Gain money from dropping off
                Debug.Log($"{npc.name}: Drop off complete - Money: {oldMoney} → {npc.stats.money}");
            }

            // Clear inventory if available
            if (npc.Inventory != null)
            {
                // Assuming inventory has method to clear resources
                // npc.Inventory.ClearResources();
                Debug.Log($"{npc.name}: Inventory cleared");
            }

            // Signal completion to AIBrain
            if (npc.aiBrain != null)
            {
                npc.aiBrain.finishedExecutingBestAction = true;
            }
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            // Go to storage/drop-off location if available, otherwise stay in place
            if (npc.context != null && npc.context.storage != null)
            {
                RequiredDestination = npc.context.storage.transform;
            }
            else
            {
                RequiredDestination = npc.transform; // Stay in place if no storage found
            }
        }
    }
}