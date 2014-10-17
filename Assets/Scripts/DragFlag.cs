using UnityEngine;
using System.Collections;

public class DragFlag : MouseDirectee {
	public GameObject follower;

	protected override void MouseUp()
	{
		base.MouseUp();
		LetGo();
	}

	protected override void LetGo()
	{
		base.LetGo();
		if (follower != null)
		{
			follower.SendMessage("FlagLetGo");
		}
	}
}
