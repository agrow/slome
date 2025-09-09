using System.Collections;
using UnityEngine;
using UnityEngine.AI;


namespace TL.Core

{
    // Purpose Statement: Moving NPC around the worldspace and animating sprite
    public class MoveController : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private Animator anim;

        // no need for rigid body because of the nav mesh
        private Vector2 lastMove = new Vector2(0, -1); // Default facing down

        void Start()
        {
            // Only assign if not set in Inspector
            if (agent == null) agent = GetComponent<NavMeshAgent>();
            if (anim == null) anim = GetComponent<Animator>();
        }

        void Update()
        {
            Vector3 velocity = agent.velocity;
            bool isMoving = velocity.magnitude > 0.05f;

            // Set isMoving parameter for animator
            if (anim != null)
                anim.SetBool("isMoving", isMoving);

            // If moving, update lastMove direction for idle facing
            if (isMoving)
            {
                lastMove = new Vector2(velocity.normalized.x, velocity.normalized.z);

                if (anim != null)
                {
                    anim.SetFloat("lastMoveX", lastMove.x);
                    anim.SetFloat("lastMoveY", lastMove.y);
                }
            }
            else
            {
                // When idle, keep lastMoveX/Y as last direction
                if (anim != null)
                {
                    anim.SetFloat("lastMoveX", lastMove.x);
                    anim.SetFloat("lastMoveY", lastMove.y);
                }
            }
        }

        public void MoveTo(Vector3 position)
        {
            agent.destination = position;
        }
    }
}