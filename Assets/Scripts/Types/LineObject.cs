using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LineObject : MonoBehaviour {

	public Vector2 GetPossition() {
		return transform.parent.parent.GetComponent<Block>().pos;
	}

	public RelativePosition position;
}
