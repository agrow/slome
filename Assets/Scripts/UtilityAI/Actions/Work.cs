using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Work", menuName = "UtilityAI/Actions/Work")]
    public class Work : Action
    {
        public override void Execute(NPCController npc)
        {
            Debug.Log($"{npc.name}: Executing Work action");
            npc.DoWork(3);
        }

        public override void SetRequiredDestination(NPCController npc)
        {
            if (npc.context != null && npc.context.Destinations != null && 
                npc.context.Destinations.ContainsKey(DestinationType.resource))
            {
                float distance = Mathf.Infinity;
                Transform nearestResource = null;

                List<Transform> resources = npc.context.Destinations[DestinationType.resource];
                
                if (resources != null && resources.Count > 0)
                {
                    foreach (Transform resource in resources)
                    {
                        if (resource != null)
                        {
                            float distanceFromResource = Vector3.Distance(resource.position, npc.transform.position);
                            if (distanceFromResource < distance)
                            {
                                nearestResource = resource;
                                distance = distanceFromResource;
                            }
                        }
                    }
                }

                if (nearestResource != null)
                {
                    RequiredDestination = nearestResource;
                    Debug.Log($"{npc.name}: Work destination set to nearest resource: {RequiredDestination.name} (distance: {distance:F1}m)");
                }
                else
                {
                    // Fallback: stay in place if no resources found
                    RequiredDestination = npc.transform;
                    Debug.LogWarning($"{npc.name}: No valid resources found, staying in place for work");
                }
            }
            else
            {
                // Fallback: stay in place if no context or destinations
                RequiredDestination = npc.transform;
                Debug.LogWarning($"{npc.name}: No context or resource destinations found, staying in place for work");
            }
            
            // Note: The NPCController FSM will handle agent.SetDestination() in HandleDecideState()
            // We don't need to set npc.mover.destination anymore
        }
    }
}