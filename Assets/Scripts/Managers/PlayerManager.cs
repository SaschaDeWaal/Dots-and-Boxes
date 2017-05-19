using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

	private Player[] currentPlayers = new Player[0];

	private static PlayerManager instance = null;
	public static PlayerManager Instance {
		get {
			if (instance == null) {
				instance = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
			}
			return instance;
		}
	}

	public void SetList(Player[] list) {
		if (isServer) {
			StartCoroutine(SendNewList(list));
		}
	}

	public Player[] GetList() {
		return currentPlayers;
	}

	public int CountPlayers() {
		return currentPlayers.Length;
	}

	public Player FindPlayerWithID(int id) {
		foreach (Player player in currentPlayers) {
			if (player.networkID == id) {
				return player;
			}
		}
		return null;
	}

	private IEnumerator SendNewList(Player[] list) {
		yield return new WaitForSeconds(1f);
		RpcOnNewPlayerList(list);
	} 

	[ClientRpc]
	private void RpcOnNewPlayerList(Player[] list) {
		currentPlayers = list;
	}
}
