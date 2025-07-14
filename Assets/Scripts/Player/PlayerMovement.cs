using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeed = 3f;
    private Vector2 _moveDirection;

    private Animator animator;
    public InputActionReference move;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        _moveDirection = move.action.ReadValue<Vector2>();
        Debug.Log(_moveDirection);
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(_moveDirection.x * moveSpeed, _moveDirection.y * moveSpeed);
    }

    void HandleAnimation()
    {
        if (_moveDirection == Vector2.zero)
        {
            animator.Play("Idle");
        }
        else if (_moveDirection.y > 0)
        {
            animator.Play("Forward");
        }
        else if (_moveDirection.y < 0)
        {
            animator.Play("Down");
        }
        else if (_moveDirection.x > 0)
        {
            animator.Play("Right");
        }
        else if (_moveDirection.x < 0)
        {
            animator.Play("Left");
        }
    }
}
