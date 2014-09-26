using UnityEngine;
using System.Collections;

public class DragFollower : MouseDirectee {
	public float moveSpeed;
	public GameObject flagPrefab;
	private GameObject flag;
	private bool moveToFlag = false;
	private int defaultLayer;

	void Start()
	{
		base.Start();
		defaultLayer = gameObject.layer;
	}

	void Update()
	{
		if (moveToFlag && flag != null)
		{
			Vector3 movement = (flag.transform.position - transform.position);
			float moveDist = movement.magnitude;
			if (moveDist > moveSpeed * Time.deltaTime)
			{
				movement = (movement / moveDist) * moveSpeed;
			}
			else
			{
				Destroy(flag);
				flag = null;
			}
			transform.position += movement * Time.deltaTime;
		}
	}

	protected override void MouseDown()
	{
		if (flag != null)
		{
			Destroy(flag);
			flag = null;
		}
		flag = (GameObject)Instantiate(flagPrefab, MousePointer.Instance.MouseHit, Quaternion.identity);
		gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	protected override void MouseUp()
	{
		// TODO need to not use this callback and get flag to drag rather than this. Ignore Raycast works at first.

		moveToFlag = true;
		MouseDirectee flagDirectee = flag.GetComponent<MouseDirectee>();
		if (flagDirectee != null)
		{
			flagDirectee.enabled = false;
		}
		gameObject.layer = defaultLayer;
	}
}
