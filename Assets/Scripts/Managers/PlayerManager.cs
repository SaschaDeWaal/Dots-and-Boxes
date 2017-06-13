using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

	const int maxPlayers = 8;

	private PlayerData[] currentPlayers = new PlayerData[0];
	private List<PlayerObject> playerObjects = new List<PlayerObject>();
	private string lastData = "";

	private void UpdatePlayerData() {
		if (isServer) {
			List<PlayerData> list = new List<PlayerData>();
			string total = "";
			foreach (PlayerObject obj in playerObjects) {
				list.Add(obj.playerData);
				total += obj.playerData.ToString();
			}
			currentPlayers = list.ToArray();

			//string compare is less heavy then Rpc calls
			if (total != lastData) {
				RpcUpdatePlayerList(currentPlayers);
			}

			lastData = total;
		}
	}

	public void ReqeustJoinGame(PlayerObject player) {
		if (currentPlayers.Length < maxPlayers) {

			List<PlayerData> newList = new List<PlayerData>(currentPlayers);

			if(!newList.Contains(player.playerData)) {
				newList.Add(player.playerData);
				playerObjects.Add(player);

				player.playerData.playerID = currentPlayers.Length;
				currentPlayers = newList.ToArray();

				StartCoroutine(SendNewList(currentPlayers));

				Debug.Log("new player joined " + player.playerData.playerID);

				player.RpcJoinReqeustResult(false, 202);
				player.RpcOnPlayerDataChanged(player.playerData);
			} else {
				player.RpcJoinReqeustResult(true, 409);
			}
		} else {
			player.RpcJoinReqeustResult(true, 410);
		}
	}

	public void ReqeustLeaveGame(PlayerData player) {
		List<PlayerData> newList = new List<PlayerData>(currentPlayers);

		if(newList.Contains(player)) {
			newList.Remove(player);
			currentPlayers = newList.ToArray();

			StartCoroutine(SendNewList(currentPlayers));
		}
	}

	public PlayerData[] GetList() {
		UpdatePlayerData();
		return currentPlayers;
	}

	public PlayerData GetLocalPlayer() {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in players) {
			if (player.GetComponent<PlayerObject>().isLocalPlayer) {
				return player.GetComponent<PlayerObject>().playerData;
			}
		}

		return null;
	}

	public int CountPlayers() {
		return currentPlayers.Length;
	}

	public PlayerData FindPlayerWithID(int id) {
		foreach (PlayerData player in currentPlayers) {
			if (player.networkID == id) {
				return player;
			}
		}
		return null;
	}

	public PlayerObject FindPlayerObjectWithPlayerData(PlayerData data) {
		foreach(PlayerObject playerObject in playerObjects) {
			if (playerObject.playerData == data) {
				return playerObject;
			}
		}
		return null;
	}

	private IEnumerator SendNewList(PlayerData[] list) {
		yield return new WaitForSeconds(1f);
		RpcOnNewPlayerList(list);
	} 

	[ClientRpc]
	private void RpcOnNewPlayerList(PlayerData[] list) {
		currentPlayers = list;
	}

	[ClientRpc]
	private void RpcUpdatePlayerList(PlayerData[] list)
	{
		currentPlayers = list;
	}
}
