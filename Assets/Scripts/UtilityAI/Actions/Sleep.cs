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

        public override void SetRequiredDestination(NPCController npc)
        {
            Debug.Log("=== Sleep.SetRequiredDestination DEBUG ===");

            // CHECK 1: Is npc null?
            if (npc == null)
            {
                Debug.LogError("Sleep: NPC is null!");
                return;
            }
            Debug.Log($"Sleep: NPC found: {npc.name}");

            // CHECK 2: Is npc.context null?
            if (npc.context == null)
            {
                Debug.LogError("Sleep: npc.context is null!");
                Debug.LogError("Solution: Create Context GameObject in scene with Context component");
                Debug.LogError("OR: Create GameObject tagged 'Home' as fallback");

                // FALLBACK: Try to find tagged home
                GameObject homeObject = GameObject.FindWithTag("Home");
                if (homeObject != null)
                {
                    RequiredDestination = homeObject.transform;
                    Debug.Log($"Sleep: Using fallback tagged home at {RequiredDestination.position}");
                }
                else
                {
                    RequiredDestination = npc.transform;
                    Debug.LogWarning("Sleep: No home found anywhere, sleeping in place");
                }

                // Skip the mover line for now to isolate the problem
                Debug.Log("Sleep: Skipping mover.destination to test context issue");
                return;
            }
            Debug.Log($"Sleep: npc.context found: {npc.context.name}");

            // CHECK 3: Is npc.context.home null?
            if (npc.context.home == null)
            {
                Debug.LogError("Sleep: npc.context.home is null!");
                Debug.LogError("Solution: In Context component, assign a GameObject to 'Home' field");

                // FALLBACK: Try to find tagged home
                GameObject homeObject = GameObject.FindWithTag("Home");
                if (homeObject != null)
                {
                    RequiredDestination = homeObject.transform;
                    Debug.Log($"Sleep: Using fallback tagged home at {RequiredDestination.position}");
                }
                else
                {
                    RequiredDestination = npc.transform;
                    Debug.LogWarning("Sleep: No home found anywhere, sleeping in place");
                }

                // Skip the mover line for now
                Debug.Log("Sleep: Skipping mover.destination to test context.home issue");
                return;
            }
            Debug.Log($"Sleep: npc.context.home found: {npc.context.home.name}");

            // CHECK 4: Set RequiredDestination
            RequiredDestination = npc.context.home.transform;
            Debug.Log($"Sleep: RequiredDestination set to: {RequiredDestination.position}");

            // CHECK 5: Is npc.mover null?
            if (npc.mover == null)
            {
                Debug.LogError("Sleep: npc.mover is null!");
                Debug.LogError("This should not happen with the NavMeshMoverWrapper");
                Debug.LogError($"NPC has NavMeshAgent: {npc.agent != null}");
                return;
            }
            Debug.Log($"Sleep: npc.mover found: {npc.mover.GetType().Name}");

            // CHECK 6: Try to set destination
            try
            {
                Debug.Log("Sleep: About to set mover.destination...");
                npc.mover.destination = RequiredDestination;
                Debug.Log("Sleep: Successfully set mover.destination");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Sleep: Error setting mover.destination: {e.Message}");
                Debug.LogError($"Sleep: Exception type: {e.GetType().Name}");
            }

            Debug.Log("=== Sleep.SetRequiredDestination COMPLETE ===");
        }
    }
    
}