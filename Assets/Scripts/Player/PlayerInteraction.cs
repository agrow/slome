using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private NPC nearbyNPC;

    private void OnTriggerEnter(Collider other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null)
        {
            Debug.Log($"Entered trigger with: {npc.npcName}");
            nearbyNPC = npc;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        NPC npc = other.GetComponent<NPC>();
        if (npc != null && npc == nearbyNPC)
        {
            Debug.Log($"Exited trigger with: {npc.npcName}");
            nearbyNPC = null;
        }
    }

    public NPC GetNearbyNPC()
    {
        return nearbyNPC;
    }

    private void Update()
    {
       
    }
}
