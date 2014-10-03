using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceableBlock : MonoBehaviour {
	public bool isStatic = false;
	public bool isVisible = false;
	public GameObject attachedWall;
	public GameObject moveableBy;
	public List<PlaceableBlock> placeableSpots;
	public float placeRange;
	public bool carried;

	void Update()
	{
		renderer.enabled = isVisible;

		if (!isStatic)
		{
			// Find nearest placeable spot
			float minSqrDist = Mathf.Infinity;
			PlaceableBlock nearSpot = null;
			for (int i = 0; i < placeableSpots.Count; i++)
			{
				float sqrDist = (transform.position - placeableSpots[i].transform.position).sqrMagnitude;
				if (sqrDist < minSqrDist && placeableSpots[i].attachedWall != null)
				{
					minSqrDist = sqrDist;
					nearSpot = placeableSpots[i];
				}
			}

			// Place in nearest spot if possible.
			if (minSqrDist <= Mathf.Pow(placeRange, 2))
			{
				carried = false;
				isStatic = false;
				transform.parent = null;
				transform.position = nearSpot.transform.position;
				transform.rotation = nearSpot.transform.rotation;
				nearSpot.UsePath();
			}
		}
		
	}

	void UsePath()
	{
		if (attachedWall != null)
		{
			Destroy(attachedWall);
			attachedWall = null;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (!isStatic && collision.collider.gameObject == moveableBy)
		{
			carried = true;
			transform.parent = collision.collider.gameObject.transform;
			transform.localRotation = Quaternion.identity;
		}
	}
}
