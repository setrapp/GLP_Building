using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceableBlock : MonoBehaviour {
	public GameObject attachedWall;
	public List<PlaceableBlock> placeableSpots;
	public float placeRange;
	public BlockType blockType;
	public BlockCarrier blockCarrier;
	public BlockCarrier carriedBy;
	public float fallAcceleration;
	private float fallSpeed;
	public float maxFallSpeed;
	private bool falling = false;
	public string groundLayer = "Ground";
	public string gapLayer = "Block Gap";
	private FixedJoint joint;
	[HideInInspector]
	public bool preparedForCarry = false;
	public MouseDirectee highlighter;

	void Start()
	{
		if (highlighter == null)
		{
			highlighter = GetComponent<MouseDirectee>();
		}
	}

	void Update()
	{
		// Stop body from moving too much when being carried.
		if (carriedBy != null)
		{
			rigidbody.velocity = new Vector3();
			rigidbody.angularVelocity = new Vector3();

			transform.rotation = carriedBy.transform.rotation;
			transform.position = carriedBy.transform.position + carriedBy.transform.TransformDirection(carriedBy.carryOffset + blockCarrier.carryOffset);
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
			transform.position = carriedBy.transform.position + carriedBy.transform.TransformDirection(carriedBy.dropOffset + blockCarrier.dropOffset);
		}

		carriedBy = null;
		rigidbody.useGravity = true;
		rigidbody.velocity = rigidbody.angularVelocity = new Vector3();
		falling = true;
		Destroy(joint);
		joint = null;
		SendMessage("LetGo");
		highlighter.colorChangeable = false;
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
		BlockCarrier potentialCarrier = collision.collider.GetComponent<BlockCarrier>();
		if (carriedBy == null && potentialCarrier != null)
		{
			// Tell carrier to drop current block
			if (potentialCarrier.carriedBlock != null)
			{
				potentialCarrier.carriedBlock.transform.rotation = this.transform.rotation;
				potentialCarrier.carriedBlock.Drop();
			}
			transform.rotation = potentialCarrier.transform.rotation;
			transform.position = potentialCarrier.transform.position + potentialCarrier.transform.TransformDirection(potentialCarrier.carryOffset + blockCarrier.carryOffset);
			potentialCarrier.carriedBlock = this;
			carriedBy = potentialCarrier;
			highlighter.colorChangeable = true;
			rigidbody.useGravity = false;
			potentialCarrier.SendMessage("StopSeeking", SendMessageOptions.DontRequireReceiver);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		CheckFallCollsion(other);
	}

	private void OnTriggerStay(Collider other)
	{
		CheckFallCollsion(other);
	}

	private void CheckFallCollsion(Collider other)
	{
		// TODO Say the text on the block when set in and when a whole gap group (onset and rime) is filled.

		if (falling)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer(groundLayer))
			{
				falling = false;
				float penetration = Mathf.Abs(transform.localScale.y - (transform.position.y - other.transform.position.y));
				transform.position += new Vector3(0, penetration, 0);
			}
			else if (other.gameObject.layer == LayerMask.NameToLayer(gapLayer))
			{
				BlockGap blockGap = other.GetComponent<BlockGap>();
				if (blockGap != null && CanFillGap(blockGap))
				{
					FillGap(blockGap);
				}
			}
		}
	}

	public bool CanFillGap(BlockGap gap)
	{
		if (blockType != gap.blockType)
		{
			return false;
		}

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
		rigidbody.useGravity = false;
		rigidbody.velocity = new Vector3();
		rigidbody.angularVelocity = new Vector3();
		Destroy(GetComponent<FixedJoint>());

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
}

public enum BlockType
{
	EMPTY = 0,
	ONSET,
	RIME
}
