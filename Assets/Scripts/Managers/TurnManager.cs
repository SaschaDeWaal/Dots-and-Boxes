using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TurnManager : NetworkBehaviour {

	private int playerTurn = 0;

	private void Start () {
	}

	private void Update() {
		if (playerTurn > Manager.Find<PlayerManager>().CountPlayers()-1) {
			playerTurn = 0;
		}
	}

	public void ReqeustAMove(PlayerData player, Vector2 position, RelativePosition relativePosition) {
		if (player.playerID == playerTurn) {
			Grid grid = Manager.Find<Grid>();
			Block block = grid.GetBlock((int) position.x, (int) position.y);

			if (block.GetLineOwner(relativePosition) == null && block.GetOwner() == null) {
				if (!block.SetLine(player, relativePosition)) {
					playerTurn = (playerTurn + 1) % Manager.Find<PlayerManager>().CountPlayers();
				}
			}
		}
	}
	 
	public PlayerData TurnToPlayer {
		get {
			int players = (Manager.Find<PlayerManager>()) ? Manager.Find<PlayerManager>().CountPlayers() : 0;
			return (players > 0) ? Manager.Find<PlayerManager>().GetList()[playerTurn% players] : null;
		}
	}

	
}
