using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;

namespace TL.Core
{   
    //Purpose Statement: 
    public class NPCController : MonoBehaviour
    {
        public MoveController mover { get; set; }
        public AIBrain aiBrain { get; set; }
        public Action[] actionsAvailable; //populate in inspector, what actions can NPC perform

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}