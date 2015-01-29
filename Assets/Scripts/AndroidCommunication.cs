using UnityEngine;
using System.Collections;

public class AndroidCommunication : MonoBehaviour {
	public TextMesh inText;
	public TextMesh outText;
	int frams = 0;
	private AndroidJavaObject jo;

	void Start()
	{
		outText.text = "out";

		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		jo.Call("UnityTest", inText.text);

		jo.Call("sendLog", "myName", "sam");
	}

	void Update()
	{
		frams++;
		jo.Call("UnityTest", inText.text+frams);
	}

	public void AndroidTest(string test)
	{
		outText.text = test + "f";
	}
}
