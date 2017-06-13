using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager: MonoBehaviour {

	private static GameObject[] managerObjs = null;
	private static Dictionary<string, MonoBehaviour> components = new Dictionary<string, MonoBehaviour>();

	public static T Find<T>() where T: MonoBehaviour {

		if (managerObjs == null) {
			managerObjs = GameObject.FindGameObjectsWithTag("Manager");
		}

		if (!components.ContainsKey(typeof(T).ToString())) {
			managerObjs = GameObject.FindGameObjectsWithTag("Manager");
			for (int i = 0; i < managerObjs.Length; i++) {
				if (managerObjs[i].GetComponentInChildren<T>()) {
					components.Add(typeof(T).ToString(), managerObjs[i].GetComponentInChildren<T>());
				}
			}
		}
		return (T) components[typeof(T).ToString()];
	}

}
