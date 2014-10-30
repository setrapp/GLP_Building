using UnityEngine;
using System.Collections;

public class FixedJointTest : MonoBehaviour {
	public FixedJoint joint;

	void Update()
	{
		//joint.autoConfigureConnectedAnchor = false;
		joint.anchor = new Vector3(0, 0, 1);
	}
}
