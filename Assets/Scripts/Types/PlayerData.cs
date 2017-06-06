using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerData: PropertyAttribute { 

	[SyncVar]
	public int playerID       = -1;

	[SyncVar]
	public Color color        = Color.black;

	[SyncVar]
	public string displayName = "nameless";

	[SyncVar]
	public int networkID      = -1;

	[SyncVar]
	public int score		  = 0;

	//public PlayerObject playerObject = null;

	public PlayerData(int playerID, int networkID, string displayName, Color color) {
		this.playerID    = playerID;
		this.networkID   = networkID;
		this.color       = color;
		this.displayName = displayName;
	}

	public PlayerData() {
		
	}

	public override string ToString() {
		return String.Format("PlayerData(name: {0}, color: {1}, id: {2}, score: {3})", displayName, color.ToString(), playerID.ToString(), score.ToString());
	}

}
