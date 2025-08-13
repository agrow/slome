using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerPixelArt : MonoBehaviour
{
    public float speed; // modulates movement
    private Vector2 move; // input system values, -1 to 1
    private Vector2 lastMove = new Vector2(0, -1); // Used to save animator state to last direction when player stops moving, seed with facing the camera
    //public float groundDist; // For if we want elevated ground/slopes w/o jumping (https://www.youtube.com/watch?v=cqNBA9Pslg8)
    //public LayerMask terrainLayer;
    private Vector3 lastWorldSpaceMove = new Vector3(0, 0, -1); // Face the camera(?) at start

    public Rigidbody rb;
    public SpriteRenderer sr;
    public Camera cam;
    public Animator anim;


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

        
    }

    void FixedUpdate()
    {
        // Note, the player may not actually move
        movePlayerRelativeToCamera();
    }

    // After camera's moved
    void LateUpdate()
    {
        // TASK: Find the direction the player last moved, absolute
        // And figure out what sprite to play relative to the camera

        // Time to deal with animations. First, determine if we are idle or moving
        if (move.x != 0 || move.y != 0)
        {
            anim.SetBool("isMoving", true);

        }
        else
        {
            // If we are NOT MOVING, play the idle animation facing the last direction we were moving
            // Note: moveX & moveY should be ZERO if we are not moving, so we should be in the idle blendtree if that's true
            anim.SetBool("isMoving", false);
        }

        // Figure out the last movement direction RELATIVE to where ever the camera is -- DONE

        // Does the blendtree handle diagonals automatically?
        // Sort of... really hard to idle facing a diagonal. 
        // TODO: Test with joystick
        //anim.SetFloat("lastMoveX", lastMove.x);
        //anim.SetFloat("lastMoveY", lastMove.y);
        Vector3 camRelativeLastMove = cam.transform.InverseTransformDirection(lastWorldSpaceMove);

        //Debug.Log(lastMove);
        //Debug.Log(camRelativeLastMove);

        anim.SetFloat("lastMoveX", camRelativeLastMove.x);
        anim.SetFloat("lastMoveY", camRelativeLastMove.z);

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
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

        // Save these vectors for figuring out animations later
        if (move.x != 0 || move.y != 0)
        {
            lastMove = new Vector2(move.x, move.y);
            lastWorldSpaceMove = new Vector3(cameraRelativeMovement.x, cameraRelativeMovement.y, cameraRelativeMovement.z);
        }
        //Vector3 forwardRelVerticalLastMove = lastMove.x * camForward;
        //Vector3 rightRelHorizontalLastMove = lastMove.y * camRight;
        //lastCameraRelativeMove = forwardRelVerticalLastMove + rightRelHorizontalLastMove;

        // Move the player
        //transform.Translate(cameraRelativeMovement * speed * Time.deltaTime, Space.World);
        rb.MovePosition(transform.position + cameraRelativeMovement * speed * Time.fixedDeltaTime);
    }
}
