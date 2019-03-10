using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Speed = 4f;
    public float Gravity = 2f;
    private bool stealth = false;
   

    private Ray lookRay;
    private Plane groundPlane;
    private float distance;
    public Vector3 target;

    private CharacterController controller;
    private Vector3 velocity;

    Vector3 forward, right, down;

    Vector3 Movement, rightMovement, forwardMovement;

    public bool Stealth
    {
        get
        {
            return stealth;
        }
    }

    void Start()
    {
        ControlRotation();
        down = Vector3.zero;
        groundPlane = new Plane(Vector3.up, Vector3.zero);
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        //look direction
        lookRay = Camera.main.ScreenPointToRay(Input.mousePosition);               
        if (groundPlane.Raycast(lookRay, out distance))
        {
            ControlRotation();            
            target = lookRay.GetPoint(distance);
            if (!Input.GetMouseButton(1))
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(target.x, transform.position.y, target.z) - transform.position), Time.deltaTime*4);
        }
        //sprint and stealth
        float MovementSpeed = Speed;
        if (Input.GetButton("Sprint") && !stealth)
        {
            MovementSpeed *= 2f;
        }
        else if (Input.GetButtonDown("Crouch"))
        {
            if (!stealth)
            {
                stealth = true;
                Debug.Log("Stealth on");
            }
            else if (stealth)
            {
                stealth = false;
                Debug.Log("Stealth off");
            }

        }
        else if (stealth)
        {
            MovementSpeed *= 0.5f;
        }
        //movement
        if (controller.isGrounded)
        {           
            rightMovement = right * MovementSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
            forwardMovement = forward * MovementSpeed * Time.deltaTime * Input.GetAxis("Vertical");
            Movement = rightMovement + forwardMovement;
            down = Vector3.zero;
        }
        else
        {
            down.y = down.y - (Gravity * Time.deltaTime);        
        }
        controller.Move(Movement + down);
    }

    private void ControlRotation()
    {
        forward = transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

}
