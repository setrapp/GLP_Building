using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour {
	public bool isStart = false;
	public bool isStop = false;
	public Waypoint loopBackTo;
	public int maxLoopBacks;
	public int loopBacks;
	public string messageCallback;
}