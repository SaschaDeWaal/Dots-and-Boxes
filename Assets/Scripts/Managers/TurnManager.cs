using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurnManager : NetworkBehaviour {

	private CustomNetworkManager networkManager;
	private PlayerManager playerManager;
	private int playerTurn = 0;

	private void Start () {
		networkManager = CustomNetworkManager.Instance;
		playerManager = Manager.Find<PlayerManager>();
	}

	private void Update() {
		//check for when a user may left
		if (playerTurn > playerManager.CountPlayers()-1) {
			playerTurn = 0;
		}
	}

	public void ReqeustAMove(PlayerData player, Vector2 position, RelativePosition relativePosition) {
		if (player.playerID == playerTurn) {
			Grid grid = Manager.Find<Grid>();
			Block block = grid.GetBlock((int) position.x, (int) position.y);

			if (block.GetLineOwner(relativePosition) == null && block.GetOwner() == null) {
				block.SetLine(player, relativePosition);

				playerTurn = (playerTurn + 1) % playerManager.CountPlayers();
			}
		}
	}
	 
	public PlayerData TurnToPlayer {
		get {
			int players = (playerManager) ? playerManager.CountPlayers() : 0;
			return (players > 0) ? playerManager.GetList()[playerTurn% players] : null;
		}
	}

	
}
