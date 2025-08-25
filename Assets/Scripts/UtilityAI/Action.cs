using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;

namespace TL.UtilityAI
{
    /*
    abstract class create the template of the class
    */
    public abstract class Action : ScriptableObject, IAction
    {
        //Action is anything the NPC can physically do
 
        [SerializeField] private string actionName; // Make it a private field

        private float _score; //how urgent the action is
        
        // Implement IAction.Name as a property so it can use IAction interface!

        public string Name
        {
            get { return actionName; }
            set { actionName = value; }
        }

        public float score
        {
            get { return _score; }
            set
            {
                this._score = Mathf.Clamp01(value); //normalizing between 0.0-1.0
            }
        }

        public Consideration[] considerations;
        // Considerations are information of the world about how urgent an action is. 
        public Transform RequiredDestination {get; protected set;} // only things of the action class can set the variable, but publicly available

        public virtual void Awake() //instaniate score with 0, virtual? 
        {
            score = 0;
        }

        public abstract void Execute(NPCController npc); //general method that an action can run once its picked out! Later on GOAP. 
        public virtual void SetRequiredDestination(NPCController npc) {}
    }
}