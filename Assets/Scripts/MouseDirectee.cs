using UnityEngine;
using System.Collections;

public class MouseDirectee : MonoBehaviour {
	public Color highlightColor = Color.yellow;
	private Color defaultColor;

	protected virtual void Start()
	{
		defaultColor = renderer.material.color;
	}

	protected virtual void MouseOver()
	{
		renderer.material.color = highlightColor;
	}

	protected virtual void MouseOut()
	{
		LetGo();
	}

	protected virtual void MouseDown() { }

	protected virtual void MouseUp() { }

	protected virtual void MouseUpOutside()
	{
		LetGo();
	}

	protected virtual void LetGo()
	{
		renderer.material.color = defaultColor;
	}

	protected virtual void MouseHold()
	{
		transform.position = MousePointer.Instance.MouseHit;
	}
}
