using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerPixelArt : MonoBehaviour
{
    public float speed; // modulates movement
    private Vector2 move; // input system values, -1 to 1
    //public float groundDist; // For if we want elevated ground/slopes w/o jumping (https://www.youtube.com/watch?v=cqNBA9Pslg8)
    //public LayerMask terrainLayer;

    public Rigidbody rb;
    public SpriteRenderer sr;
    public Camera cam;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /* Potential vertical movement from https://www.youtube.com/watch?v=cqNBA9Pslg8
        Raycast hit;
        Vector3 castPos = transform.position;
        castPos.y += 1;
        if (Physics.Raycast(castPos, -transform.up, out hit, Mathf.Infinity, terrainLayer))
        {
            if (hit.collider != null)
            {
                Vector3 movePos = transform.position;
                movePos.y = hit.point.y + groundDist;
                transform.position = movePos;
            }
        }

        */

        movePlayerRelativeToCamera();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void movePlayer()
    {
        // Started with this tutorial: https://www.youtube.com/watch?v=xF19LIYfUmY
        // TODO GOAL: 
        // 2. Don't reset to facing camera when no movement
        // 3. Actually always rotate toward camera, but it's the movement/animation that plays differently
        Vector3 movementAmt = new Vector3(move.x, 0f, move.y);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementAmt), 0.15f);
        transform.Translate(movementAmt * speed * Time.deltaTime, Space.World);
    }

    public void movePlayerRelativeToCamera()
    {
        // Code adapted from https://www.youtube.com/watch?v=7kGCrq1cJew timestamp 1:00
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        // Remove y movement for now, assuming a flat plane
        // TODO: terrain height variability & jumping
        camForward.y = 0;
        camRight.y = 0;
        camForward = camForward.normalized;
        camRight = camRight.normalized;

        // Apply movement to camera vectors
        Vector3 forwardRelativeVerticalMovement = move.y * camForward;
        Vector3 rightRelativeHorizontalMovement = move.x * camRight;
        Vector3 cameraRelativeMovement = forwardRelativeVerticalMovement + rightRelativeHorizontalMovement;

        // Move the player
        transform.Translate(cameraRelativeMovement * speed * Time.deltaTime, Space.World);
    }
}
