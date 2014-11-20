using UnityEngine;
using System.Collections;

public class LauncherDrop : MouseDirectee {
	public DragFollower flagFollower;
	public Vector3 turnOffset;
	public Vector3 stopOffset;

	protected void MouseUp()
	{
		if (flagFollower != null && flagFollower.flag != null)
		{
			//Debug.Log(transform.InverseTransformPoint(turnOffset));
			flagFollower.flag.transform.position = transform.position + transform.TransformDirection(turnOffset);
		}
		flagFollower.AddSpecialTarget(transform.position + transform.TransformDirection(stopOffset));
	}
}
