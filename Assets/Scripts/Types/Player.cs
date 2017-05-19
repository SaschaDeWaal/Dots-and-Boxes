using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player {

	private Color[] colors = new Color[] {new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1), new Color(1, 1, 0), new Color(1, 0, 1)};

	//for now default names. when login and database is ready, the account name will be here.
	private String[] names = new[] {"klaas", "Jan", "huis", "pc"};

	//vars must be public so we can send this object over the internet.
	public int playerID       = 1;
	public Color color        = Color.black;
	public string displayName = "nameless";
	public int networkID      = -1;

	public Player(int playerID, int netID) {
		this.playerID    = playerID;
		this.networkID   = netID;
		this.color       = colors[playerID % colors.Length];
		this.displayName = names[playerID % names.Length];
	}

	public Player() {
		
	}

}
