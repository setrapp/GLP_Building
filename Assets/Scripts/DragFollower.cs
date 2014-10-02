using UnityEngine;
using System.Collections;

public class DragFollower : MouseDirectee {
	public GameObject flagPrefab;
	private GameObject flag;
	private bool moveToFlag = false;
	private bool destinationSet = false;
	public NavMeshAgent navigator = null;

	void Awake()
	{
		base.Awake();
		
		if (navigator == null)
		{
			navigator = GetComponent<NavMeshAgent>();
		}
	}

	void Update()
	{
		if (moveToFlag && flag != null)
		{
			if (!destinationSet && navigator != null)
			{
				navigator.SetDestination(flag.transform.position);
				destinationSet = true;
			}

			if ((flag.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(navigator.stoppingDistance, 2))
			{
				Destroy(flag);
				flag = null;
			}
		}
		else
		{
			navigator.Stop();
		}
	}

	protected virtual void FlagLetGo()
	{
		moveToFlag = true;
		destinationSet = false;
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
