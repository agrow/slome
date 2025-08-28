using UnityEngine;

namespace TL.EmotionalAI
{
    /// <summary>
    /// Manages Triangle of Love values and relationship type classification
    /// </summary>
    public class RelationshipProfile : MonoBehaviour
    {
        [Header("Triangle of Love Values (0..1)")]
        [SerializeField, Range(0f, 1f)] private float intimacy = 0.4f;     // I
        [SerializeField, Range(0f, 1f)] private float passion = 0.4f;      // Pa  
        [SerializeField, Range(0f, 1f)] private float commitment = 0.4f;   // C

        [Header("Current Relationship Status")]
        [SerializeField] private RelationshipType currentRelationshipType;
        
        private Triangle triangle;

        private void Start()
        {
            // Initialize triangle with starting values
            triangle = new Triangle { I = intimacy, Pa = passion, C = commitment };
            
            // Classify initial relationship type
            currentRelationshipType = ClassifyRelationshipType(triangle);
            
            Debug.Log($"RelationshipProfile: Initialized as {currentRelationshipType}");
            Debug.Log($"RelationshipProfile: TOL Values - I:{intimacy:F2}, Pa:{passion:F2}, C:{commitment:F2}");
        }

        /// <summary>
        /// Gets the current Triangle of Love values
        /// </summary>
        public Triangle GetTriangle()
        {
            // Update triangle with current values
            triangle.I = intimacy;
            triangle.Pa = passion;
            triangle.C = commitment;
            
            return triangle;
        }

        /// <summary>
        /// Updates Triangle of Love values and reclassifies relationship
        /// </summary>
        public void UpdateTriangle(float deltaIntimacy, float deltaPassion, float deltaCommitment)
        {
            intimacy = Mathf.Clamp01(intimacy + deltaIntimacy);
            passion = Mathf.Clamp01(passion + deltaPassion);
            commitment = Mathf.Clamp01(commitment + deltaCommitment);
            
            // Update triangle
            triangle.I = intimacy;
            triangle.Pa = passion;
            triangle.C = commitment;
            
            // Reclassify relationship type
            RelationshipType newType = ClassifyRelationshipType(triangle);
            
            if (newType != currentRelationshipType)
            {
                Debug.Log($"RelationshipProfile: Relationship evolved from {currentRelationshipType} to {newType}");
                currentRelationshipType = newType;
            }
        }

        /// <summary>
        /// Sets Triangle of Love values directly
        /// </summary>
        public void SetTriangle(float newIntimacy, float newPassion, float newCommitment)
        {
            intimacy = Mathf.Clamp01(newIntimacy);
            passion = Mathf.Clamp01(newPassion);
            commitment = Mathf.Clamp01(newCommitment);
            
            triangle.I = intimacy;
            triangle.Pa = passion;
            triangle.C = commitment;
            
            currentRelationshipType = ClassifyRelationshipType(triangle);
            
            Debug.Log($"RelationshipProfile: Set to {currentRelationshipType} - I:{intimacy:F2}, Pa:{passion:F2}, C:{commitment:F2}");
        }

        /// <summary>
        /// Classifies relationship type based on Triangle of Love values
        /// Uses thresholds to determine relationship category
        /// </summary>
        private RelationshipType ClassifyRelationshipType(Triangle t)
        {
            float lowThreshold = 0.3f;
            float highThreshold = 0.7f;
            
            bool highIntimacy = t.I >= highThreshold;
            bool highPassion = t.Pa >= highThreshold;
            bool highCommitment = t.C >= highThreshold;
            
            bool lowIntimacy = t.I <= lowThreshold;
            bool lowPassion = t.Pa <= lowThreshold;
            bool lowCommitment = t.C <= lowThreshold;

            // Classification logic based on Sternberg's Triangle of Love
            if (highIntimacy && highPassion && highCommitment)
            {
                return RelationshipType.Partner; // Consummate Love
            }
            else if (highIntimacy && highCommitment && !highPassion)
            {
                return RelationshipType.CloseFriend; // Companionate Love
            }
            else if (highIntimacy && !lowPassion && !lowCommitment)
            {
                return RelationshipType.Friend; // Liking/Friendship
            }
            else if (highPassion && !lowIntimacy && !lowCommitment)
            {
                return RelationshipType.Crush; // Infatuation/Romantic
            }
            else if(lowPassion && lowIntimacy && lowCommitment)
            
            {
                return RelationshipType.Dislike; // Hostile-ish relationship??
            }
            else
                    {
                        return RelationshipType.Stranger; // Non-love or very low connection
                    }
        }

        /// <summary>
        /// Gets current relationship type
        /// </summary>
        public RelationshipType GetRelationshipType()
        {
            return currentRelationshipType;
        }

        /// <summary>
        /// Gets a description of the current relationship
        /// </summary>
        public string GetRelationshipDescription()
        {
            return GetRelationshipDescription(currentRelationshipType);
        }

        /// <summary>
        /// Gets description for a specific relationship type
        /// </summary>
        public static string GetRelationshipDescription(RelationshipType type)
        {
            switch (type)
            {
                case RelationshipType.Stranger:
                    return "Unknown person with minimal connection";
                case RelationshipType.Crush:
                    return "Romantic attraction with developing feelings";
                case RelationshipType.Friend:
                    return "Friendly bond with mutual trust and care";
                case RelationshipType.CloseFriend:
                    return "Deep friendship with strong emotional bond";
                case RelationshipType.Partner:
                    return "Romantic partnership with deep love and commitment";
                case RelationshipType.Dislike:
                    return "Negative feelings with low connection";
                default:
                    return "Undefined relationship";
            }
        }

        // Inspector helper methods for debugging
        [ContextMenu("Log Current Status")]
        private void LogCurrentStatus()
        {
            Debug.Log($"=== RelationshipProfile Status ===");
            Debug.Log($"Type: {currentRelationshipType}");
            Debug.Log($"Description: {GetRelationshipDescription()}");
            Debug.Log($"Triangle: I={intimacy:F2}, Pa={passion:F2}, C={commitment:F2}");
        }

        [ContextMenu("Test Relationship Evolution")]
        private void TestRelationshipEvolution()
        {
            Debug.Log("Testing relationship evolution...");
            UpdateTriangle(0.1f, 0.1f, 0.1f);
        }
    }
}