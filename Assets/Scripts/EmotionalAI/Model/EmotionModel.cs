using Unity.VisualScripting;
using UnityEngine;

namespace TL.EmotionalAI
{

/*
     * EMOTIONAL MODEL WORKFLOW:
     * 
     * 1. PLAYER ACTION → ApplyPlayerAction(action, intensity)
     *    └─ Maps PlayerAction to Intent via IntentMapper
     *    └─ Gets base PAD delta from BasePadByIntent 
     *    └─ Applies action intensity modifier
     * 
     * 2. PERSONALITY AMPLIFICATION
     *    └─ PersonalityAmplifier modifies PAD based on NPC personality traits
     * 
     * 3. RELATIONSHIP AMPLIFICATION  
     *    └─ RelationshipAmplifier modifies PAD based on Triangle of Love values
     * 
     * 4. PAD UPDATE
     *    └─ Final delta applied to Pleasure/Arousal/Dominance values
     *    └─ Emotion classified from new PAD state
     * 
     * 5. RELATIONSHIP EVOLUTION (separate call)
     *    └─ UpdateRelationship() changes Triangle values over time
     *    └─ Type transitions (Stranger→Friend→Partner) trigger additional PAD effects
     * 
     * RESULT: Dynamic emotional responses shaped by personality, relationships, and context
     */

    
    public class EmotionModel : MonoBehaviour
    {
        [Header("Personality Integration")]
        public PersonalityTypeDefinition personalityType;

        // Add this field for relationship status
        [Header("Relationship")]
        public RelationshipProfile relationshipProfile = new RelationshipProfile(); // Default to new profile


        [Header("PAD State (0..1)")]
        public PAD pad = new PAD { P = 0.5f, A = 0.5f, D = 0.5f };

        [Header("Love Triangle (0..1)")]
        public Triangle tri = new Triangle { I = 0.4f, Pa = 0.4f, C = 0.4f };

        [Header("Diagnostics")]
        public Vector3 lastDeltaApplied;
        public EmotionOctant lastEmotion;

        public Intent lastIntent = Intent.Bonding; // default before any player actions occur 



        private void Start()
        {
            InitializePAD();
            InitializeTriangle();
        }

        // prps stmt: apply player actions with personality and relationship integration!  this is called from npc controller
        /// Map Player Action By Intent → Get Intensity Modifier  → Calculate Relationship Delta
        /// → Calculate PAD Delta → Apply Relationship Amp → Apply PAD Delta -> Update Emotion from last one!
        public void ApplyPlayerAction(PlayerAction action, float intensity = 1.0f) // this intent pretty useless rn 
        {
            // 1. Map action to intent
            lastIntent = IntentMapper.Map(action);
        
            // 2. Get the action's intensity modifier (now just a float, not per-triangle)
            float actionIntensity = GetActionIntensityModifier(action);

            // 3. Use intent to get relationship triangle delta
            Vector3 relationshipDelta = BaseRelationshipDeltaByIntent.Get(lastIntent) * actionIntensity;
            UpdateRelationship(relationshipDelta.x, relationshipDelta.y, relationshipDelta.z);
                                
            // 4. Calculate PAD delta using intent and action intensity
            Vector3 baseDelta = GetEmotionalDelta(action, actionIntensity);
        
           
            // 5. Apply relationship amplification
            Triangle currentTriangle = relationshipProfile != null ? relationshipProfile.GetTriangle() : tri;
            baseDelta = RelationshipAmplifier.Apply(baseDelta, currentTriangle);
        
            // 6. Store for diagnostics and apply to PAD
            lastDeltaApplied = baseDelta;
            ApplyPADDelta(baseDelta);        
        
            // 7. Update emotion
            lastEmotion = EmotionClassifier.From(pad);
        
            // 8. Log everything
            Debug.Log($"{name}: Action '{action}' (Intent: '{lastIntent}') | PAD Δ: {baseDelta} | Relationship: {GetCurrentRelationshipType()} | Triangle: I={currentTriangle.I:F2}, Pa={currentTriangle.Pa:F2}, C={currentTriangle.C:F2} | Emotion: {lastEmotion}");
        }


        // purpose stmt : update relationship and apply type transition effects
        // brainstorming needed: when should the relationship change in teh flow of the system? in the ApplyPlayerAction method maybe?
        public void UpdateRelationship(float deltaIntimacy, float deltaPassion, float deltaCommitment)
        {
            if (relationshipProfile == null)
            {
                Debug.LogWarning($"{name}: No RelationshipProfile attached! Cannot update relationship.");

                return;
            }

            RelationshipType previousType = relationshipProfile.GetRelationshipType();

            // Update relationship values
            relationshipProfile.UpdateTriangle(deltaIntimacy, deltaPassion, deltaCommitment);
            tri = relationshipProfile.GetTriangle(); // Keep local copy in sync

            RelationshipType newType = relationshipProfile.GetRelationshipType();
            Debug.Log($"{name}: Updated Relationship Triangle: I={tri.I:F2}, Pa={tri.Pa:F2}, C={tri.C:F2} → Type: {previousType} → {newType}");

            // Apply relationship transition effects
            if (newType != previousType)
            {
                Vector3 transitionDelta = GetRelationshipTransitionDelta(previousType, newType);
                ApplyPADDelta(transitionDelta);

                Debug.Log($"{name}: Relationship: {previousType} → {newType}");

            }
        }

        /// <summary>
        /// Get current relationship type
        /// </summary>
        public RelationshipType GetCurrentRelationshipType()
        {
            return relationshipProfile != null ? relationshipProfile.GetRelationshipType() : RelationshipType.Stranger;
        }

        // private helper methods!

        //purpose stmt: gets the initial PAD values based on the personality type of the NPC 
        private void InitializePAD()
        {
            if (personalityType != null)
            {
                pad.P = personalityType.pleasureBaseline;
                pad.A = personalityType.arousalBaseline;
                pad.D = personalityType.dominanceBaseline;
            }
            else
            {
                pad.P = 0.5f;
                pad.A = 0.5f;
                pad.D = 0.5f;
            }

            lastEmotion = EmotionClassifier.From(pad);
        }

        private float GetActionIntensityModifier(PlayerAction action)
        {
            return action switch
            {
                PlayerAction.Flirt => 0.8f,
                PlayerAction.HoldHands => 1.0f,
                PlayerAction.Hug => 1.1f,
                PlayerAction.KissQuick => 1.5f,
                PlayerAction.KissDeep => 1.8f,
                PlayerAction.ComplimentLooks => 0.9f,
                PlayerAction.GiftSmall => 1.0f,
                PlayerAction.GiftLarge => 1.4f,
                PlayerAction.TeasePlayful => 1.0f,
                PlayerAction.InviteActivity => 1.1f,
                PlayerAction.Apology => 0.8f,
                PlayerAction.KeepPromise => 1.3f,
                PlayerAction.TeaseHarsh => 1.2f,
                _ => 1.0f
            };
        }



        //purpose stmt: initializes the relationship triangle
        private void InitializeTriangle()
        {
            if (relationshipProfile != null)
            {
                tri = relationshipProfile.GetTriangle();
            }
        }

        // prps stmt: the GetEmotionalDelta method to use Intent → BasePAD flow
        private Vector3 GetEmotionalDelta(PlayerAction action, float intensity)
        {

            // Step 1: Map action to intent !
            Intent intent = IntentMapper.Map(action);


            // Step 2: Get base PAD delta for that intent
            Vector3 intentBaseDelta = BasePadByIntent.Get(intent);

            // Step 3: Apply action-specific intensity modifier (optional refinement)
            float actionModifier = GetActionIntensityModifier(action);

            // Step 4: Combine intent base + action intensity + player intensity
            Vector3 finalDelta = intentBaseDelta * actionModifier * intensity;

            return finalDelta;
        }


        // Add action intensity modifiers for fine-tuning within intents
        

        //prps stmt: gets pad delta for relationship TRANSITION (made a bit simpler for now since we have more statuses)
        private Vector3 GetRelationshipTransitionDelta(RelationshipType from, RelationshipType to)
        {
            // Simple positive progression rewards
            if ((int)to > (int)from) // Any upgrade
            {
                float intensity = ((int)to - (int)from) * 0.1f; // Bigger jumps = bigger rewards
                return new Vector3(0.15f * intensity, 0.1f * intensity, 0.08f * intensity);
            }

            // Simple negative progression penalties  
            if ((int)to < (int)from) // Any downgrade
            {
                float intensity = ((int)from - (int)to) * 0.1f; // Bigger drops = bigger penalties
                return new Vector3(-0.2f * intensity, 0.15f * intensity, -0.1f * intensity);
            }

            return Vector3.zero; // No change for same level
        }


        // purpose stmt: applies the PAD delta to the current PAD values
        private void ApplyPADDelta(Vector3 delta)
        {
            pad.P = Mathf.Clamp01(pad.P + delta.x);
            pad.A = Mathf.Clamp01(pad.A + delta.y);
            pad.D = Mathf.Clamp01(pad.D + delta.z);

            lastDeltaApplied = delta;
            lastEmotion = EmotionClassifier.From(pad);
        }
        //private helper methods 

    










        //debug methods 

        [ContextMenu("Reset to Baseline")]
        public void ResetToBaseline()
        {
            InitializePAD();
            Debug.Log($"{name}: Reset to baseline");
        }

        [ContextMenu("Log Current State")]
        public void LogCurrentState()
        {
            Debug.Log($"=== {name} State ===");
            Debug.Log($"PAD: P={pad.P:F2}, A={pad.A:F2}, D={pad.D:F2}");
            Debug.Log($"Emotion: {lastEmotion}");
            Debug.Log($"Relationship: {GetCurrentRelationshipType()}");
            Debug.Log($"Triangle: I={tri.I:F2}, Pa={tri.Pa:F2}, C={tri.C:F2}");
        }

        //debug methods
    }
}