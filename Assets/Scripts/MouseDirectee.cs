using UnityEngine;
using System.Collections;

public class MouseDirectee : MonoBehaviour {
	public bool draggable = true;
	public bool colorChangeable = true;
	public Color highlightColor = Color.yellow;
	public Color defaultColor;
	public MeshRenderer colorRenderer;

	protected virtual void Awake()
	{
		if (colorRenderer == null)
		{
			colorRenderer = GetComponent<MeshRenderer>();
		}
		defaultColor = colorRenderer.material.color;
	}

	protected virtual void MouseOver()
	{
		if (colorChangeable)
		{
			colorRenderer.material.color = highlightColor;
		}
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

	protected virtual void MouseHold()
	{
		if (draggable)
		{
			transform.position = MousePointer.Instance.MouseHit;
		}
	}

	protected virtual void LetGo()
	{
		if (colorChangeable)
		{
			colorRenderer.material.color = defaultColor;
		}
	}

	
}
