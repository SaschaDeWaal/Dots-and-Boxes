using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

	const int maxPlayers = 4;

	private PlayerData[] currentPlayers = new PlayerData[0];
	private List<PlayerObject> playerObjects = new List<PlayerObject>();

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
		return currentPlayers;
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
}
