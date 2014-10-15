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
	public BlockCarrier blockCarrier;
	public BlockCarrier carriedBy;
	public float fallAcceleration;
	private float fallSpeed;
	public float maxFallSpeed;
	private bool falling = false;
	public string groundLayer = "Ground";
	public string gapLayer = "Block Gap";

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
		if (carriedBy != null)
		{
			BlockCarrier carrierBlock = carriedBy.GetComponent<BlockCarrier>();
			if (carrierBlock != null)
			{
				carrierBlock.carriedBlock = null;
			}
		}

		carriedBy = null;
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
			if (carriedBy == null && potentialCarrier != null && potentialCarrier.carriedBlock == null)
			{
				carried = true;
				transform.parent = collision.collider.gameObject.transform;
				transform.localRotation = Quaternion.identity;
				transform.localPosition += potentialCarrier.carryOffset;
				potentialCarrier.carriedBlock = this;
				carriedBy = potentialCarrier;
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

		if (falling)
		{
			if (collision.collider.gameObject.layer == LayerMask.NameToLayer(groundLayer))
			{
				falling = false;
				float penetration = Mathf.Abs(transform.localScale.y - (transform.position.y - collision.collider.transform.position.y));
				transform.position += new Vector3(0, penetration, 0);
			}
			else if (collision.collider.gameObject.layer == LayerMask.NameToLayer(gapLayer))
			{
				BlockGap blockGap = collision.collider.GetComponent<BlockGap>();
				if (blockGap != null && CanFillGap(blockGap))
				{
					FillGap(blockGap);
				}
			}
		}
	}

	public bool CanFillGap(BlockGap gap)
	{
		if (gap == null || gap.attachedGap == null)
		{
			return true;
		}
		else if (blockCarrier.carriedBlock == null)
		{
			return false;
		}
		return blockCarrier.carriedBlock.CanFillGap(gap.attachedGap);
	}

	public void FillGap(BlockGap gap)
	{
		falling = false;
		transform.position = gap.transform.position;
		transform.rotation = gap.transform.rotation;

		gap.filled = true;
		if (gap.attachedWall != null)
		{
			gap.attachedWall.SetActive(false);
		}

		if (blockCarrier.carriedBlock != null && gap.attachedGap != null)
		{
			blockCarrier.carriedBlock.FillGap(gap.attachedGap);
		}
	}

	private void ConnectOnsetRime(PlaceableBlock onset, PlaceableBlock rime)
	{
		if (rime.carriedBy != null && onset.carriedBy == null)
		{
			onset.transform.parent = rime.transform.parent;
			onset.transform.localPosition = rime.transform.localPosition;
			onset.transform.localRotation = rime.transform.localRotation;
			onset.carriedBy = rime.carriedBy;
			onset.carriedBy.carriedBlock = onset;
		}
		rime.transform.parent = onset.transform;
		rime.transform.localRotation = Quaternion.identity;
		rime.transform.localPosition = new Vector3(0, 0, 1);
		BlockCarrier onsetCarrier = onset.GetComponent<BlockCarrier>();
		rime.carriedBy = onsetCarrier;
		onsetCarrier.carriedBlock = rime;
	}
}
