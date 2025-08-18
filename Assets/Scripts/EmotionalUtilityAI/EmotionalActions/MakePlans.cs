using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "MakePlans", menuName = "UtilityAI/EmotionalActions/Make Plans")]
    // Making plans will be instantaneous
    public class MakePlans : Action
    {
        public override void Execute(NPCController npc)
        {
            // Dependency Injection: your class doesn't have a local reference to an GameObject, but you can still require it. 
            // Don't need a coroutine since time isn't concerned with this design of planning interaction

            Debug.Log("I'm making plans! :)");
            // Logic for updating everything involved with making plans 

            // Update relationship stats that exist in Stats class
            npc.stats.intimacy += 0.15f; // Primary increase in intimacy through planning together (already exists in Stats)
            npc.stats.belonging += 0.12f; // Strong increase in belonging through shared planning (already exists in Stats)
            npc.stats.romantic += 0.08f; // Moderate romantic increase through future planning (already exists in Stats)
            
            // Making plans might cost some energy and money (planning activities, reservations, etc.)
            npc.stats.energy -= 8; // Moderate energy cost for planning
            npc.stats.money -= 12; // Cost for making reservations or planning activities

            // Decide our new best action after you finished this one...
            npc.aiBrain.finishedExecutingBestAction = true;
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            // For now, making plans happens at current location
            // Later could be expanded to require specific planning locations
            RequiredDestination = npc.transform;
        }
    }
}