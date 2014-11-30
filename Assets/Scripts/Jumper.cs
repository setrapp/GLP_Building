﻿using UnityEngine;
using System.Collections;

public class Jumper : MonoBehaviour {
	public float jumpForce;
	public LayerMask groundLayer;
	private bool jumped;
	private Vector3 navigatorDestination;
	private bool navigateUponLanding;

	public void Jump()
	{
		rigidbody.AddForce(new Vector3(0, jumpForce, 0));
		NavMeshAgent navigator = GetComponent<NavMeshAgent>();
		if (navigator)
		{
			navigator.enabled = false;
			navigatorDestination = navigator.destination;
		}
		rigidbody.useGravity = true;
		jumped = true;
		navigateUponLanding = true;
	}

	public void JumpAndStop()
	{
		Jump();
		navigateUponLanding = false;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (jumped && Mathf.Pow(2, collision.collider.gameObject.layer) == groundLayer)
		{
			if (navigateUponLanding)
			{
				NavMeshAgent navigator = GetComponent<NavMeshAgent>();
				if (navigator)
				{
					navigator.enabled = true;
					navigator.SetDestination(navigatorDestination);
				}
			}
			rigidbody.useGravity = false;
			jumped = false;
		}
	}
}
