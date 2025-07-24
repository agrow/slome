using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TL.UtilityAI
{
    /*
    abstract class create the template of the class
    */
    public abstract class Action : ScriptableObject
    {
        //Action is anything the NPC can physically do
        public string Name;
        private float _score; //how urgent the action is

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

        public virtual void Awake() //instaniate score with 0, virtual? 
        {
            score = 0;
        }

        public abstract void Execute(); //general method that an action can run once its picked out! Later on GOAP. 
    }
}