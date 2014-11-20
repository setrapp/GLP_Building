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

			Vector3 toFlag = flag.transform.position - transform.position;
			if (toFlag.y < navigator.height / 2)
			{
				toFlag.y = 0;
			}

			if (toFlag.sqrMagnitude <= Mathf.Pow(navigator.stoppingDistance, 2))
			{
				transform.position = new Vector3(flag.transform.position.x, transform.position.y, flag.transform.position.z);
				if (seekSpecialTarget)
				{
					flag.transform.position = specialTarget;
					seekSpecialTarget = false;
					transform.LookAt(specialTarget, Vector3.up);
					navigator.velocity = new Vector3();
					destinationSet = false;
					ToggleObstacleAvoidance(false);
				}
				else
				{
					Destroy(flag);
					flag = null;
					moveToFlag = false;
					ToggleObstacleAvoidance(true);
				}
			}
		}
		else 
		{
			StopSeeking();
		}
	}

	private void StopSeeking()
	{
		if (navigator != null && navigator.enabled)
		{
			navigator.Stop();
			navigator.velocity = Vector3.zero;
			
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
		ToggleObstacleAvoidance(true);
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

	private void ToggleObstacleAvoidance(bool avoid)
	{
		if (navigator.enabled)
		{
			if (avoid)
			{
				navigator.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
			}
			else
			{
				navigator.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
			}
		}
	}
}
