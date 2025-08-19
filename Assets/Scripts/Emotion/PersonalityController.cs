using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI
{
    public class MBTIPersonalityController : MonoBehaviour
    {
        [Header("MBTI Personality Configuration")]
        public PersonalityProfile.Energy energy = PersonalityProfile.Energy.Extraverted;
        public PersonalityProfile.Mind mind = PersonalityProfile.Mind.Intuitive;
        public PersonalityProfile.Nature nature = PersonalityProfile.Nature.Feeling;
        public PersonalityProfile.Tactics tactics = PersonalityProfile.Tactics.Prospecting;
        public PersonalityProfile.Identity identity = PersonalityProfile.Identity.Assertive;
        
        [Header("Dynamic Personality Effects")]
        [Range(0f, 1f)]
        public float personalityInfluenceRate = 0.1f;
        public bool allowPersonalityDrift = true;
        
        [Header("Debugging")]
        public bool showPersonalityDebug = true;

        private PersonalityProfile personalityProfile;
        private EmotionalState emotionalState;
        private bool isPlayer = false;

        public PersonalityProfile PersonalityProfile => personalityProfile;
        public string MBTIType => personalityProfile?.MBTIType ?? "Unknown";

        private void Start()
        {
            // Create personality profile from inspector values
            personalityProfile = new PersonalityProfile(energy, mind, nature, tactics, identity);
            
            // Check if this is the player
            isPlayer = gameObject.CompareTag("Player");
            
            // Get emotional state
            if (isPlayer)
            {
                emotionalState = GetComponent<EmotionalState>();
                if (emotionalState == null)
                {
                    emotionalState = gameObject.AddComponent<EmotionalState>();
                }
            }
            else
            {
                Stats stats = GetComponent<Stats>();
                if (stats != null)
                {
                    emotionalState = stats.GetEmotionalState();
                }
            }

            // Apply initial personality influence
            if (personalityProfile != null && emotionalState != null)
            {
                ApplyInitialPersonalityInfluence();
                
                if (showPersonalityDebug)
                {
                    Debug.Log($"{name}: MBTI Type: {personalityProfile.MBTIType}");
                    Debug.Log($"{name}: PAD Values - P:{emotionalState.Pleasure:F2} A:{emotionalState.Arousal:F2} D:{emotionalState.Dominance:F2}");
                }
            }
        }

        private void Update()
        {
            if (personalityProfile != null && emotionalState != null && allowPersonalityDrift)
            {
                ApplyPersonalityInfluence();
            }
        }

        private void ApplyInitialPersonalityInfluence()
        {
            Vector3 padValues = personalityProfile.GetPADValues();
            
            // Set PAD values based on MBTI personality
            emotionalState.Pleasure = Mathf.Lerp(emotionalState.Pleasure, padValues.x, 0.5f);
            emotionalState.Arousal = Mathf.Lerp(emotionalState.Arousal, padValues.y, 0.5f);
            emotionalState.Dominance = Mathf.Lerp(emotionalState.Dominance, padValues.z, 0.5f);
        }

        private void ApplyPersonalityInfluence()
        {
            if (personalityInfluenceRate <= 0f) return;

            Vector3 targetPAD = personalityProfile.GetPADValues();
            float influence = personalityInfluenceRate * Time.deltaTime;

            // Gradually pull PAD values toward personality base values
            emotionalState.Pleasure = Mathf.Lerp(emotionalState.Pleasure, targetPAD.x, influence);
            emotionalState.Arousal = Mathf.Lerp(emotionalState.Arousal, targetPAD.y, influence);
            emotionalState.Dominance = Mathf.Lerp(emotionalState.Dominance, targetPAD.z, influence);
        }

       

        // Get compatibility with another character's personality
        public float GetCompatibilityWith(MBTIPersonalityController other)
        {
            if (personalityProfile == null || other?.personalityProfile == null) return 0.5f;
            
            return personalityProfile.GetCompatibilityWith(other.personalityProfile);
        }

        // Personality-based preferences
        public bool PrefersPlayerInteraction => personalityProfile?.PrefersPlayerInteraction() ?? true;
        public bool AvoidsConflict => personalityProfile?.AvoidsConflict() ?? false;

        // MBTI trait accessors
        public PersonalityProfile.Energy EnergyTrait => personalityProfile?.EnergyTrait ?? PersonalityProfile.Energy.Extraverted;
        public PersonalityProfile.Mind MindTrait => personalityProfile?.MindTrait ?? PersonalityProfile.Mind.Observant;
        public PersonalityProfile.Nature NatureTrait => personalityProfile?.NatureTrait ?? PersonalityProfile.Nature.Thinking;
        public PersonalityProfile.Tactics TacticsTrait => personalityProfile?.TacticsTrait ?? PersonalityProfile.Tactics.Judging;
        public PersonalityProfile.Identity IdentityTrait => personalityProfile?.IdentityTrait ?? PersonalityProfile.Identity.Assertive;
    }
}