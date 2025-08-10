using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Gift", menuName = "UtilityAI/Actions/Gift")]
    // Gifting will be instantaneous
    public class Gift : Action
    {
        public override void Execute(NPCController npc)
        {
            // Dependency Injection: your class doesn't have a local reference to an GameObject, but you can still require it. 
            // Don't need a coroutine since time isn't concerned with this design of gift giving

            Debug.Log("I'm giving a gift! <3");
            // Logic for updating everything involved with gift giving 

            // Update relationship stats that exist in Stats class
            npc.stats.intimacy += 0.25f; // Strong increase in intimacy through thoughtful gifting (already exists in Stats)
            npc.stats.romantic += 0.18f; // Good romantic increase through gift giving (already exists in Stats)
            npc.stats.belonging += 0.08f; // Small increase in belonging through caring gesture (already exists in Stats)
            
            // Gifting costs money and some energy
            npc.stats.energy -= 6; // Small energy cost for gift giving
            npc.stats.money -= 25; // Higher cost for purchasing gifts

            // Decide our new best action after you finished this one...
            npc.aiBrain.finishedExecutingBestAction = true;
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            // For now, gifting happens at current location
            // Later could be expanded to require gift shops or special locations
            RequiredDestination = npc.transform;
        }
    }
}