using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using Prototype.NetworkLobby;


public class NetworkLobbyHook : LobbyHook {


	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		LobbyPlayer lobby           = lobbyPlayer.GetComponent<LobbyPlayer>();
		SetLobbyAccountData account = lobbyPlayer.GetComponent<SetLobbyAccountData>();
		PlayerObject playerObject   = gamePlayer.GetComponent<PlayerObject>();


		string name                 = account.name;
		string avatar               = account.avatar;


		playerObject.InitPlayer(name, lobby.playerColor, avatar);

	}
}
