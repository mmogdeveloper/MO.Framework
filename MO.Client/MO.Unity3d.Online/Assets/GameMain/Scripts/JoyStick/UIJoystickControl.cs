using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIJoystickControl : MonoBehaviour
{
	//public Transform border;
	public float moveDistance = 100.0f;

	private Vector3 startPosition;
	private Vector3 startRotation;

	public static Vector3 destination;
	public static Vector3 GetDestination()
	{
		return destination;
	}

	void Start()
	{
		startPosition = transform.position;
	}

	//drag event callback
	public void OnJoystickDrag()
	{
		if (Vector3.Distance(Input.mousePosition, startPosition) < moveDistance)
		{	
			transform.position = Input.mousePosition;
		}
		else
		{
			transform.position = startPosition + moveDistance * destination;
		}
		destination = (Input.mousePosition - startPosition).normalized;
	}

	//drop event callback
	public void OnJoystickDrop()
	{
		transform.position = startPosition;
		destination = startRotation;
	}
}
