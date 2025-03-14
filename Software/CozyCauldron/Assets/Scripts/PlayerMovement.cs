using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 20f;
    private bool isWalking = false;
    private bool isRunning = false;
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float rotationDeadzone = 0.1f;

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
        isWalking = hasHorizontalInput || hasVerticalInput; //boolean for walking (if either horizontal or vertical input is true)
        m_Animator.SetBool("isWalking", isWalking); //set the IsWalking parameter in the animator to the value of isWalking

        isRunning = (hasHorizontalInput || hasVerticalInput) && Input.GetKey(KeyCode.LeftShift); //boolean for running (if either horizontal or vertical input is true and the left shift key is pressed)
        m_Animator.SetBool("isRunning", isRunning); //set the IsRunning parameter in the animator to the value of isRunning

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f); //calculate the desired forward vector
        m_Rotation = Quaternion.LookRotation(desiredForward); //set the rotation to face the desired forward vector

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector3 move = m_Movement * currentSpeed * Time.fixedDeltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + move);
        if (m_Movement.magnitude > rotationDeadzone)
        {
            m_Rigidbody.MoveRotation(m_Rotation);
        }

        //if (m_Movement != Vector3.zero)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(m_Movement);
        //    Quaternion smoothedRotation = Quaternion.RotateTowards(m_Rigidbody.rotation, targetRotation, turnSpeed * Time.fixedDeltaTime);
        //    m_Rigidbody.MoveRotation(smoothedRotation);
        //}
        bool isWaving = Input.GetKey(KeyCode.Space);
        Debug.Log("waving?" + isWaving);
        m_Animator.SetBool("isWaving", isWaving);

        //bool isWaving = Input.GetKey(KeyCode.Q); //boolean for waving (if the Q key is pressed)
        //if (isWaving)
        //{
        //    m_Animator.SetBool("isWaving", isWaving); //set the Wave trigger in the animator
        //}
    }

    //private void OnAnimatorMove()
    //{
    //    if (isWalking)
    //    {   // * 5f
    //        Debug.Log("DeltaPosition: " + m_Animator.deltaPosition);
    //        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Animator.deltaPosition.magnitude * m_Movement); //move the rigidbody to the new position
    //        m_Rigidbody.MoveRotation(m_Rotation); //rotate the rigidbody to the new rotation
    //    }
    //    else if (isRunning)
    //    {   // * 10f
    //        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude); //move the rigidbody to the new position
    //        m_Rigidbody.MoveRotation(m_Rotation); //rotate the rigidbody to the new rotation        }
    //    }
    //}
}
