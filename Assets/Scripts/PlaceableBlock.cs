using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaceableBlock : MonoBehaviour {
	public bool isStatic = false;
	public bool isVisible = false;
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

	public enum BlockType
	{
		EMPTY = 0,
		ONSET,
		RIME
	}

	void Update()
	{
		// Stop body from moving too much when being carried.
		if (carriedBy != null)
		{
			rigidbody.velocity = new Vector3();
			rigidbody.angularVelocity = new Vector3();
		}

		// Check for block connecting.
		if (carriedBy != null)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(transform.position, Vector3.down, out hitInfo, carriedBy.transform.localScale.y, (int)Mathf.Pow(2, gameObject.layer)))
			{
				//Debug.Log(gameObject.layer + " " + hitInfo.collider.gameObject.layer);
				PlaceableBlock hitBlock = hitInfo.collider.GetComponent<PlaceableBlock>();
				if (hitBlock != null)
				{
					if (blockType == BlockType.ONSET && hitBlock.blockType == BlockType.RIME)
					{
						ConnectOnsetRime(this, hitBlock);
					}
					else if (blockType == BlockType.RIME && hitBlock.blockType == BlockType.ONSET)
					{
						ConnectOnsetRime(hitBlock, this);
					}
				}
			}
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
		rigidbody.useGravity = true;
		rigidbody.velocity = rigidbody.angularVelocity = new Vector3();
		falling = true;
		Destroy(joint);
		joint = null;
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
				transform.parent = collision.collider.gameObject.transform;
				transform.localRotation = Quaternion.identity;
				transform.localPosition = potentialCarrier.carryOffset + blockCarrier.carryOffset;
				potentialCarrier.carriedBlock = this;
				carriedBy = potentialCarrier;
				ConnectJoint();
				//preparedForCarry = true;
			}
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
		//rime.transform.localPosition = new Vector3(0, 0, 1);
		BlockCarrier onsetCarrier = onset.GetComponent<BlockCarrier>();
		rime.carriedBy = onsetCarrier;
		onsetCarrier.carriedBlock = rime;

		//onset.transform.localPosition = onset.carriedBy.carryOffset + onset.blockCarrier.carryOffset;
		//onset.preparedForCarry = true;
		//rime.transform.localPosition = rime.carriedBy.carryOffset + rime.blockCarrier.carryOffset;
		//rime.preparedForCarry = true;
		onset.ConnectJoint();
		rime.ConnectJoint();
	}

	public void PrepareToBeCarried(BlockCarrier potentialCarrier)
	{
		transform.parent = potentialCarrier.transform;
		transform.localRotation = Quaternion.identity;
		transform.localPosition = potentialCarrier.carryOffset + blockCarrier.carryOffset;
		potentialCarrier.carriedBlock = this;
		carriedBy = potentialCarrier;
		preparedForCarry = true;
	}

	public void ConnectJoint()
	{
		if (carriedBy != null)
		{
			if (transform.parent != carriedBy.transform)
			{
				transform.parent = carriedBy.transform;
			}
			transform.localPosition = carriedBy.carryOffset + blockCarrier.carryOffset;
			Debug.Log(transform.localPosition);
			if (joint == null)
			{
				joint = gameObject.AddComponent<FixedJoint>();
			}
			joint.connectedBody = carriedBy.rigidbody;
			
			rigidbody.useGravity = false;
			rigidbody.velocity = new Vector3();
			rigidbody.angularVelocity = new Vector3();
		}
		
	}
}
