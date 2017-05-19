using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class PlayerObject : NetworkBehaviour {

	[SyncVar]
	public int id = 0;

	private void Start() {
		if (isLocalPlayer) {
			id = GameObject.FindGameObjectsWithTag("Player").Length - 1;
			Debug.Log("Id = " + id);
		}
	}

	public void Update() {
		if (isLocalPlayer && Input.GetMouseButtonDown(0)) {
			Clicked();
		}
	}

	private void Clicked() {
		Vector2 mousePosition  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

		if (hitCollider && hitCollider.tag == "Line") {
			LineObject line = hitCollider.GetComponent<LineObject>();

			CmdReqeustAMove(id, line.GetPossition(), line.position); 
		}
	}

	[Command]
	private void CmdReqeustAMove(int playerId, Vector2 position, RelativePosition relativePosition) {
		Player player = Manager.Find<PlayerManager>().FindPlayerWithID(playerId);
		if (player != null) {
			Manager.Find<TurnManager>().ReqeustAMove(player, position, relativePosition);
		}
	}
}
