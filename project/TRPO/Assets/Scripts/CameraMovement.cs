using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    public float TurnSpeed = 4.0f;

    private Quaternion rotation;
    private float currentX;

    void Start()
    {
        currentX = 11.25f;
    }

    void Update()
    {       
        transform.parent.position = player.transform.position;
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X");
            rotation = Quaternion.Euler(0.0f, currentX * TurnSpeed, 0.0f);
            Camera.main.transform.parent.rotation = rotation;
        }
    }
}
