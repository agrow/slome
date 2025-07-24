using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Eat", menuName = "UtilityAI/Actions/Eat")]
    // Eating will be Instaneously
    public class Eat : Action
    {
        public override void Execute(NPCController npc)
        {
            // Dependacy Injection: your class doesn't have a local refernence to an GameObject, but you can still require it. 
            // Dont need a coroutine since time isn't concenred with this design of hunger

            //npc.stats.hunger -= 1; eventually
            Debug.Log("I Ate Food! :)");
            // Logic for updating everything involved with eating 

            // Decide our new best action after you finished this one...
            npc.stats.hunger -= 30;
            npc.stats.money -= 10; //spend gold to eat
            npc.OnFinishedAction();
        }
    }
}