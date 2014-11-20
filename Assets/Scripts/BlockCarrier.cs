using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockCarrier : MonoBehaviour {
	public PlaceableBlock carriedBlock = null;
	public Vector3 carryOffset = Vector3.zero;
	public Vector3 dropOffset = Vector3.zero;
}
