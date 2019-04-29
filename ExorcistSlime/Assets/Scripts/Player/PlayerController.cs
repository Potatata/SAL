﻿using UnityEngine;
using Prime31;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //Constants
    private const float SPEED = 16f;
    private const int DASH_TIME = 3;
    private const float DASH_SPEED = SPEED*SPEED;

    // Movement configuration
    public float speed = SPEED;
    public int dashTime = 0;

    //Fields
    private TopDownMovementController2D _controller;
	private Vector3 _velocity;
    public GameObject saltPrefab;

    void Awake()
	{
		_controller = GetComponent<TopDownMovementController2D>();
    }

    void Dash()
    {
        if (Input.GetAxis("Horizontal") + Input.GetAxis("Vertical") != 0)
        {
            //Player dash mechanic
            if (Input.GetButtonDown("Fire1"))
            {
                dashTime = DASH_TIME;
                speed = DASH_SPEED;
                Debug.Log("Dashing!");
            }
            else
            {
                if (dashTime <= 0) speed = SPEED;
                else
                {
                    --dashTime;
                    LeaveTrail();
                }
            }
        }
    }

    private void LeaveTrail() {
        Instantiate(saltPrefab, this.transform.position, this.transform.rotation);
    }

    void Move()
    {
        //Player movement
        _velocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        _controller.move(_velocity * Time.deltaTime * speed);
        // Grab our current _velocity to use as a base for all calculations
        _velocity = _controller.velocity;
    }

    // The Update loop contains a very simple example of moving the character around
    void Update()
    {
        Dash();
        Move();
    }

}
