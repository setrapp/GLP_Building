using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockCarrier : MonoBehaviour {
	public PlaceableBlock carriedBlock = null;
	public List<PlaceableBlock> carriedBlocks;
	public Vector3 carryOffset = Vector3.zero;
}
