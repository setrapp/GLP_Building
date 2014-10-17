using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {
	public GameObject triggerer;
	public Vector3 targetPosition;

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == triggerer)
		{
			SimpleMover cameraMover = Camera.main.GetComponent<SimpleMover>();
			if (cameraMover != null)
			{
				cameraMover.MoveTo(targetPosition, true);
			}
		}
	}
}
