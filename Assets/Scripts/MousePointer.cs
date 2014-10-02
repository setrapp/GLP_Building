using UnityEngine;
using System.Collections;

public class MousePointer : MonoBehaviour {
	private static MousePointer instance = null;
	public static MousePointer Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindGameObjectWithTag("Globals").GetComponent<MousePointer>();
			}
			return instance;
		}
	}

	private Vector3 mouseHit = Vector3.zero;
	public Vector3 MouseHit
	{
		get { return mouseHit; }
	}
	private GameObject mouseTarget = null;
	public GameObject MouseTarget
	{
		get { return mouseTarget; }
	}
	private GameObject targetPressed = null;

	// TODO allow for layermask ignoring.

	void Update()
	{
		// Prepare to ignore currently pressed object.
		int targetLayer = 0;
		if (targetPressed != null)
		{
			targetLayer = targetPressed.layer;
			if (!Input.GetMouseButtonUp(0))
			{
				targetPressed.layer = LayerMask.NameToLayer("Ignore Raycast");
			}
		}

		// Cast a ray from the mouse to determine if any object in the world is under the cursor.
		Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit = new RaycastHit();
		GameObject mousedOver = null;
		if (Physics.Raycast(mouseRay,out hit, Mathf.Infinity))
		{
			mousedOver = hit.collider.gameObject;
			mouseHit = hit.point;
		}

		// Stop ignoring currently pressed object.
		if (targetPressed != null)
		{
			targetPressed.layer = targetLayer;
		}

		// If the object being moused over has changed, update the old and new mouse targets.
		if (mousedOver != mouseTarget)
		{
			if (mouseTarget != null && mouseTarget != targetPressed)
			{
				mouseTarget.BroadcastMessage("MouseOut", SendMessageOptions.DontRequireReceiver);
			}
			mouseTarget = mousedOver;
			if (mouseTarget != null && mouseTarget != targetPressed)
			{
				mouseTarget.BroadcastMessage("MouseOver", SendMessageOptions.DontRequireReceiver);
			}
		}

		// Handle the status of the left mouse button.
		if (Input.GetMouseButtonDown(0))
		{
			if (mouseTarget != null)
			{
				mouseTarget.BroadcastMessage("MouseDown", SendMessageOptions.DontRequireReceiver);
			}
			targetPressed = mouseTarget;
		} 
		else if (Input.GetMouseButton(0))
		{
			if (targetPressed != null)
			{
				targetPressed.BroadcastMessage("MouseHold", SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (Input.GetMouseButtonUp(0))
		{
			if (mouseTarget != null)
			{
				mouseTarget.BroadcastMessage("MouseUp", SendMessageOptions.DontRequireReceiver);
				if (mouseTarget == targetPressed)
				{
					mouseTarget.BroadcastMessage("MouseClick", SendMessageOptions.DontRequireReceiver);
				}
			}
			if (targetPressed != null && targetPressed != mouseTarget)
			{
				targetPressed.BroadcastMessage("MouseUpOutside", SendMessageOptions.DontRequireReceiver);
			}
			targetPressed = null;
		}
	}

	public void TargetObject(GameObject newTarget, bool mouseDownSendable = true, bool mouseOverSendable = true)
	{
		// Notifiy current target that is no longer the target.
		if (mouseTarget != null)
		{
			mouseTarget.BroadcastMessage("MouseOut", SendMessageOptions.DontRequireReceiver);
			mouseTarget.BroadcastMessage("MouseUp", SendMessageOptions.DontRequireReceiver);
		}

		// Set new target and notify
		mouseTarget = newTarget;
		if (mouseOverSendable)
		{
			mouseTarget.BroadcastMessage("MouseOver", SendMessageOptions.DontRequireReceiver);
		}
		if (mouseOverSendable)
		{
			mouseTarget.BroadcastMessage("MouseDown", SendMessageOptions.DontRequireReceiver);
		}

		if (Input.GetMouseButton(0))
		{
			targetPressed = mouseTarget;
		} 
	}
}
