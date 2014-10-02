using UnityEngine;
using System.Collections;

public class DragFlag : MouseDirectee {
	public GameObject follower;

	protected virtual void MouseUp()
	{
		base.MouseUp();
		LetGo();
	}

	protected virtual void LetGo()
	{
		base.LetGo();
		if (follower != null)
		{
			follower.SendMessage("FlagLetGo");
		}
	}
}
