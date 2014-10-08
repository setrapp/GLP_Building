using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockGap : MonoBehaviour {
	// TODO Separate gap code into hear and figure out dropping.

	/*public bool isStatic = false;
	public bool isVisible = false;
	public GameObject attachedWall;
	public float placeRange;
	public bool carried;
	public BlockType blockType;
	public BlockCarrier carrier;
	public PlaceableBlock attachedBlock;

	public enum BlockType
	{
		EMPTY = 0,
		ONSET,
		RIME
	}

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

	private void UsePath()
	{
		if (attachedWall != null)
		{
			Destroy(attachedWall);
			attachedWall = null;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!isStatic)
		{
			BlockCarrier potentialCarrier = collision.collider.GetComponent<BlockCarrier>();
			if (carrier == null && potentialCarrier != null && potentialCarrier.carriedBlock == null)
			{
				carried = true;
				transform.parent = collision.collider.gameObject.transform;
				transform.localRotation = Quaternion.identity;
				potentialCarrier.carriedBlock = this;
				carrier = potentialCarrier;
			}
			else
			{
				PlaceableBlock hitBlock =collision.collider.GetComponent<PlaceableBlock>();
				if (hitBlock != null)
				{
					if (blockType == BlockType.ONSET && hitBlock.blockType == BlockType.RIME)
					{
						ConnectOnsetRime(this ,hitBlock);
					}
					else if (blockType == BlockType.RIME && hitBlock.blockType == BlockType.ONSET)
					{
						ConnectOnsetRime(hitBlock, this);
					}
				}
			}
		}
	}

	private void ConnectOnsetRime(PlaceableBlock onset, PlaceableBlock rime)
	{
		if (rime.carrier != null && onset.carrier == null)
		{
			onset.transform.parent = rime.transform.parent;
			onset.transform.localPosition = rime.transform.localPosition;
			onset.transform.localRotation = rime.transform.localRotation;
			onset.carrier = rime.carrier;
			onset.carrier.carriedBlock = onset;
		}
		rime.transform.parent = onset.transform;
		rime.transform.localRotation = Quaternion.identity;
		rime.transform.localPosition = new Vector3(0, 0, 1);
		rime.carrier = null;
		onset.attachedBlock = rime;
	}*/
}
