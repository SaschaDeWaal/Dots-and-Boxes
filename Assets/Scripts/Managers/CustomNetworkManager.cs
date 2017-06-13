using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
 
public class CustomNetworkManager : NetworkManager {

	public GameObject prefab;

	private Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();

	/*private static CustomNetworkManager instance = null;
	public static CustomNetworkManager Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindWithTag("Network").GetComponent<CustomNetworkManager>();
			}

			return instance;
		}
	}*/

	public override void OnServerConnect(NetworkConnection conn) {
		Debug.Log("connect user: " + conn.address);
		base.OnServerConnect(conn);
	}

	public override void OnServerDisconnect(NetworkConnection conn) {
		Debug.Log("Disconnect user: " + conn.address);
		base.OnServerDisconnect(conn);
	}

}
 