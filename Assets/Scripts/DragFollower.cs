using UnityEngine;
using System.Collections;

public class DragFollower : MouseDirectee {
	public GameObject flagPrefab;
	[HideInInspector]
	public GameObject flag;
	public bool moveToFlag = false;
	private bool destinationSet = false;
	public NavMeshAgent navigator = null;
	private bool seekSpecialTarget = false;
	private Vector3 specialTarget;

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
			if (!destinationSet && navigator != null && navigator.enabled)
			{
				navigator.SetDestination(flag.transform.position);
				destinationSet = true;
			}

			if ((flag.transform.position - transform.position).sqrMagnitude <= Mathf.Pow(navigator.stoppingDistance, 2))
			{
				if (seekSpecialTarget)
				{
					flag.transform.position = specialTarget;
					seekSpecialTarget = false;
					transform.LookAt(specialTarget, Vector3.up);
					navigator.velocity = new Vector3();
					destinationSet = false;
				}
				else
				{
					Destroy(flag);
					flag = null;
					moveToFlag = false;
				}
			}
		}
		else if (navigator.enabled)
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
		seekSpecialTarget = false;
	}

	public void AddSpecialTarget(Vector3 target)
	{
		specialTarget = target;
		seekSpecialTarget = true;
	}
}
