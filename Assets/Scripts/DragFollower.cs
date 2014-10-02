using UnityEngine;
using System.Collections;

public class DragFollower : MouseDirectee {
	public float moveSpeed;
	public GameObject flagPrefab;
	private GameObject flag;
	private bool moveToFlag = false;

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

	protected virtual void FlagLetGo()
	{
		moveToFlag = true;
		MouseDirectee flagDirectee = flag.GetComponent<MouseDirectee>();
		if (flagDirectee != null)
		{
			flagDirectee.enabled = false;
		}
	}

	protected override void MouseDown()
	{
		// Destory old flag.
		if (flag != null)
		{
			Destroy(flag);
			flag = null;
		}

		// Create new flag to seek.
		flag = (GameObject)Instantiate(flagPrefab, MousePointer.Instance.MouseHit, Quaternion.identity);
		DragFlag flagDragFlag = flag.GetComponent<DragFlag>();
		if (flagDragFlag != null)
		{
			flagDragFlag.follower = gameObject;
		}

		// Tell mouse pointer to target the flag instead of this.
		MousePointer.Instance.TargetObject(flag);
		moveToFlag = false;
	}
}
