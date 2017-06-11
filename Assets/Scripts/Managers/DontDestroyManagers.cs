using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyManagers : MonoBehaviour {

	private void Start () {
		DontDestroyOnLoad(gameObject);
	}
	
}
