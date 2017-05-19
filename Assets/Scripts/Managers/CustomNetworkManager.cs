using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
 
public class CustomNetworkManager : NetworkManager {

	public GameObject prefab;

	private Dictionary<int, Player> players = new Dictionary<int, Player>();

	private static CustomNetworkManager instance = null;
	public static CustomNetworkManager Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindWithTag("Network").GetComponent<CustomNetworkManager>();
			}

			return instance;
		}
	}

	public override void OnServerConnect(NetworkConnection conn) {
		Debug.Log("connect user: " + conn.address);

		Player newPlayer = new Player(players.Count, conn.connectionId);
		players.Add(conn.connectionId, newPlayer);

		PlayerManager.Instance.SetList(PlayerDictionaryToArray(players));

		base.OnServerConnect(conn);
	}

	public override void OnServerDisconnect(NetworkConnection conn) {
		Debug.Log("Disconnect user: " + conn.address);
		players.Remove(conn.connectionId);

		PlayerManager.Instance.SetList(PlayerDictionaryToArray(players));

		base.OnServerDisconnect(conn);
	}

	private Player[] PlayerDictionaryToArray(Dictionary<int, Player> dictionary) {
		List<Player> list = new List<Player>();
		list.AddRange(players.Values);
		return list.ToArray();
	}
}
 