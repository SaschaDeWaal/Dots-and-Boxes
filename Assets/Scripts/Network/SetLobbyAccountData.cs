using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class SetLobbyAccountData : NetworkBehaviour {

	public string name     = "";
	public string avatar   = "";

	private bool firstTime = true;
	
	private void Update () {

		if (isLocalPlayer && firstTime) {

			firstTime               = false;

			LobbyPlayer lobbyPlayer = GetComponent<LobbyPlayer>();
			name                    = Manager.Find<AccountManager>().AccountData["name"].Value;
			avatar                  = Manager.Find<AccountManager>().AccountData["avatar"].Value;

			lobbyPlayer.SetName(name);
			CmdSetData(name, avatar);
		}
	}

	[Command]
	private void CmdSetData(string name, string avatar) {
		this.name   = name;
		this.avatar = avatar;
	}
}
