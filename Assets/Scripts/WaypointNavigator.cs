using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaypointNavigator : MonoBehaviour {

	[SerializeField]
	public List<Waypoint> waypoints;
	public bool showWaypoints;
	public int current;
	private int previous;
	private bool collideWithWaypoint = false;
	public GameObject waypointContainer;
	public NavMeshAgent navigator;

	protected void Start()
	{
		if (navigator == null)
		{
			navigator = GetComponent<NavMeshAgent>();
		}

		if (waypointContainer != null)
		{
			Waypoint[] waypointObjects = waypointContainer.GetComponentsInChildren<Waypoint>();
			int startIndex = -1;
			for (int i = 0; i < waypointObjects.Length && startIndex < 0; i++)
			{
				if (waypointObjects[i].isStart)
				{
					startIndex = i;
				}
			}
			if (startIndex >= 0)
			{
				waypoints = new List<Waypoint>();
				while (waypoints.Count < waypointObjects.Length)
				{
					if (startIndex > 0 && startIndex + waypoints.Count >= waypointObjects.Length)
					{
						startIndex = 0;
					}
					waypoints.Add(waypointObjects[startIndex + waypoints.Count]);
				}
			}
		}

		SeekNextWaypoint();
		collideWithWaypoint = false;
		if (previous >= 0 && previous < waypoints.Count)
		{
			Vector3 toWaypoint = waypoints[previous].transform.position - transform.position;
			transform.position += toWaypoint;
		}

		for (int i = 0; i < waypoints.Count; i++)
		{
			waypoints[i].renderer.enabled = showWaypoints;
		}
	}

	void Update()
	{
		// If the distance travelled has exceeded the span between the current waypoint, update it.
		if (collideWithWaypoint)
		{
			SeekNextWaypoint();
			collideWithWaypoint = false;

			// If the waypoint triggers a function, call the function.
			if (waypoints[previous].messageCallback != null && waypoints[previous].messageCallback != "")
			{
				SendMessage(waypoints[previous].messageCallback, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public Vector3 FindSeekingPoint(Vector3 velocity)
	{
		if (waypoints == null || waypoints.Count <= 0 || current >= waypoints.Count)
		{
			return transform.position;
		}

		Vector3 movement = velocity * Time.deltaTime;
		Vector3 projection = Helper.ProjectVector(waypoints[current].transform.position - waypoints[previous].transform.position, transform.position + movement - waypoints[previous].transform.position);

		return waypoints[previous].transform.position + projection;
	}

	private void SeekNextWaypoint()
	{
		if (waypoints == null || waypoints.Count <= 0 || current >= waypoints.Count)
		{
			return;
		}

		previous = current;
		collideWithWaypoint = false;

		// If the node loops back, place the target the waypoint being passed and move all the waypoints to create cycle.
		if (waypoints[previous].loopBackTo != null)
		{
			if (waypoints[previous].maxLoopBacks < 0 || waypoints[previous].maxLoopBacks > waypoints[previous].loopBacks)
			{
				waypoints[previous].loopBacks++;
				Vector3 newStart = waypoints[previous].transform.position;
				int newPrevious = 0;
				for (int i = 0; i < waypoints.Count; i++)
				{
					if (waypoints[i] == waypoints[previous].loopBackTo)
					{
						newPrevious = i;
					}
					else
					{
						Vector3 toNext = waypoints[i].transform.position - waypoints[previous].loopBackTo.transform.position;
						waypoints[i].transform.position = newStart + toNext;
					}
				}
				waypoints[previous].loopBackTo.transform.position = newStart;
				previous = newPrevious;
			}
			else
			{
				waypoints[previous].loopBacks = 0;
			}

		}
		current = previous + 1;

		if (navigator != null && navigator.enabled)
		{
			if (current < waypoints.Count && !waypoints[previous].isStop)
			{
				navigator.SetDestination(waypoints[current].transform.position);
			}
			else
			{
				navigator.Stop();
			}
		}
	}

	void OnTriggerEnter(Collider otherCol)
	{
		if (waypoints != null && current < waypoints.Count && otherCol.gameObject == waypoints[current].gameObject)
		{
			collideWithWaypoint = true;
		}
	}
}
