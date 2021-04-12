using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 2f;
    [SerializeField] float turnSpeed = 20f;
    

    void Update()
    {

        if(Input.GetKey(KeyCode.Q))
            transform.Rotate(Time.deltaTime * turnSpeed * Vector3.down);
        
        if(Input.GetKey(KeyCode.E))
            transform.Rotate(Time.deltaTime * turnSpeed * Vector3.up);

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            transform.position += Time.deltaTime * movementSpeed * transform.right;
        
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            transform.position += Time.deltaTime * movementSpeed * -transform.right;
        
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            transform.position += Time.deltaTime * movementSpeed * transform.forward;
        
    }
    
}
