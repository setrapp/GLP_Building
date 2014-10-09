using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceableBlock : MonoBehaviour {
	public bool isStatic = false;
	public bool isVisible = false;
	public GameObject attachedWall;
	public List<PlaceableBlock> placeableSpots;
	public float placeRange;
	public bool carried;
	public BlockType blockType;
	public BlockCarrier carrier;
	public float fallAcceleration;
	private float fallSpeed;
	public float maxFallSpeed;
	private bool falling = false;
	public string groundLayer = "Ground";

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

		if (!isStatic && falling)
		{
			fallSpeed = Mathf.Max(fallSpeed + fallAcceleration * Time.deltaTime, maxFallSpeed);
			transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);
		}
	}

	private void OnMouseUp()
	{
		Drop();
	}

	public void Drop()
	{
		if (carrier != null)
		{
			BlockCarrier carrierBlock = carrier.GetComponent<BlockCarrier>();
			if (carrierBlock != null)
			{
				carrierBlock.carriedBlock = null;
			}
		}

		carrier = null;
		transform.parent = null;
		falling = true;
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
				transform.localPosition += potentialCarrier.carryOffset;
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

		if (falling && collision.collider.gameObject.layer == LayerMask.NameToLayer(groundLayer))
		{
			falling = false;
			float penetration = Mathf.Abs(transform.localScale.y - (transform.position.y - collision.collider.transform.position.y));
			transform.position += new Vector3(0, penetration, 0);
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
		BlockCarrier onsetCarrier = onset.GetComponent<BlockCarrier>();
		rime.carrier = onsetCarrier;
		onsetCarrier.carriedBlock = rime;
	}
}
