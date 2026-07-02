using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouvementDash : MonoBehaviour
{
    public float speed = 5f;
    public float jumpHeight = 12f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        CheckGround();
        MoveForward();
        HandleInput();
    }


    void CheckGround() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    void MoveForward() {
        rb.velocity = new Vector2(speed, rb.velocity.y);
    }

    void HandleInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            TryJump(jumpHeight);
        }
    }

    void TryJump(float height) {
        if (isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, height);
            Debug.Log(new Vector2(rb.velocity.x, height));
        }
    }
}
