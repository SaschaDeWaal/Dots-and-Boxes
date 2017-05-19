using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager: MonoBehaviour {

	private static GameObject managerObj = null;
	private static Dictionary<string, MonoBehaviour> components = new Dictionary<string, MonoBehaviour>();

	public static T Find<T>() where T: MonoBehaviour {
		if (managerObj == null) {
			managerObj = GameObject.FindWithTag("Manager");
		}
		if (!components.ContainsKey(typeof(T).ToString())) {
			components.Add(typeof(T).ToString(), managerObj.GetComponentInChildren<T>());
		}
		return (T) components[typeof(T).ToString()];
	}

}
