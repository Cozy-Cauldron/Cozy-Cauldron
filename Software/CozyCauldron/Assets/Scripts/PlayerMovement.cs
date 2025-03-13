using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;

    Animator m_Animator;
    Rigidbody m_Rigidbody;
    Vector3 m_Movement;
    Quaternion m_Rotation = Quaternion.identity;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement.Normalize();

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f); //boolean for horizontal input (if not approx 0) 
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f); //boolean for vertical input (if not approx 0)
        bool isWalking = hasHorizontalInput || hasVerticalInput; //boolean for walking (if either horizontal or vertical input is true)
        m_Animator.SetBool("isWalking", isWalking); //set the IsWalking parameter in the animator to the value of isWalking

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f); //calculate the desired forward vector
        m_Rotation = Quaternion.LookRotation(desiredForward); //set the rotation to face the desired forward vector
    }

    private void OnAnimatorMove()
    {
        //Debug.Log($"DeltaPos: {m_Animator.deltaPosition}, Movement: {m_Movement}");
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude * 10f); //move the rigidbody to the new position
        m_Rigidbody.MoveRotation(m_Rotation); //rotate the rigidbody to the new rotation
    }
}
