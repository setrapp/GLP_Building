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
			flagFollower.flag.transform.position = transform.position + turnOffset;
		}
		flagFollower.AddSpecialTarget(transform.position + stopOffset);
	}
}
