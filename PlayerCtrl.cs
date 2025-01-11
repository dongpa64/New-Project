using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCtrl : MonoBehaviour
{
    public Vector3 moveInput;
    public Rigidbody rb; 
    public float jumpPower; // 점프의 힘
    public float moveSpeed; // 걷기 속도
    bool isJump;
    Animator animator; 
    Camera camera;

    public float smoothness; // 회전의 부드러움을 조정하는 값
    public float mouseSensitivity = 1.0f; // 마우스 감도

    void Awake()
    {
        animator = GetComponent<Animator>(); 
        camera = Camera.main; 
        rb = GetComponent<Rigidbody>();
        isJump = false;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical");     
        moveInput = new Vector3(horizontal, 0, vertical).normalized;

        if (Input.GetButtonDown("Jump") && !isJump)
        {
            isJump = true;
            animator.SetBool("Jump", true);
            rb.AddForce(new Vector3(0, jumpPower, 0), ForceMode.Impulse);
        }
        
        // 마우스 위치를 월드 좌표로 변환
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = camera.transform.position.y; 
        Vector3 worldMousePosition = camera.ScreenToWorldPoint(mousePosition);

        Vector3 mouseDirection = worldMousePosition - transform.position;
        mouseDirection.y = 0; 

        if (mouseDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(mouseDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothness * Time.deltaTime);
        }
        MovePlayer();
        UpdateAnimation();
    }
    private void MovePlayer()
    {
        Vector3 moveDirection = moveInput * moveSpeed;
        moveDirection.y = rb.velocity.y;
        rb.velocity = moveDirection;
    }
    
    private void UpdateAnimation()
    {
        float forward = Vector3.Dot(moveInput.normalized, transform.forward);
        float right = Vector3.Dot(moveInput.normalized, transform.right);

        animator.SetFloat("Forward", forward); // Forward (앞, 뒤 방향)
        animator.SetFloat("Right", right); // Right (좌, 우 방향)

        if (moveInput.magnitude > 0)
        {
            animator.SetFloat("Speed", moveInput.magnitude); // 이동 속도 기반 애니메이션
        }
        else
        {
            animator.SetFloat("Speed", 0); // 정지 상태
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // 바닥에 닿으면 점프 상태를 해제
        if (collision.gameObject.tag == "Ground")
        {
            if (isJump)
            {
                isJump = false;
                animator.SetBool("Jump", false);
            }
        }
    }
}