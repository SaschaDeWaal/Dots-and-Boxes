using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class PlayerObject : NetworkBehaviour {

	private Color[] colors = new Color[] { new Color(1, 0, 0), new Color(0, 1, 0), new Color(0, 0, 1), new Color(1, 1, 0), new Color(1, 0, 1) };
	private string[] names = new string[] { "player 1", "player 2", "player 3", "player 4" };

	public string PlayerString = "";

	//Every client needs to acces this information, but only the server is allowed to set/change it value.
	//So instead of using SyncVar, I used ClientRPC to set this variable.
	private PlayerData _playerData = new PlayerData();
	public PlayerData playerData {
		get {
			return _playerData;
		}
	}

	private void Start() {
		if(isServer) {
			StartCoroutine(SetPlayerData());
		}
	}

	[ServerCallback]
	private IEnumerator SetPlayerData() {
		int random = Random.Range(0, 4);

		_playerData.networkID   = GetId();
		_playerData.displayName = names[random];
		_playerData.color       = colors[random];

		RpcOnPlayerDataChanged(_playerData);
		yield return null;
		ReqeustJoinGame();
	}

	public override void OnNetworkDestroy() {
		CmdReqeustLeaveGame();
	}

	public void Update() {
		if (isLocalPlayer && Input.GetMouseButtonDown(0)) {
			Clicked();
		}
		PlayerString = playerData.ToString();
	}

	public int GetId() {
		return GetComponent<NetworkIdentity>().GetInstanceID();
	}

	private void Clicked() {
		Vector2 mousePosition  = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

		if (hitCollider && hitCollider.tag == "Line") {
			LineObject line = hitCollider.GetComponent<LineObject>();

			CmdReqeustAMove(line.GetPossition(), line.position); 
		}
	}

	[ClientRpc]
	public void RpcJoinReqeustResult(bool problemAppeared, int result) {
		if(problemAppeared) {
			Debug.LogError("Coudn't joint game. Error: " + result.ToString());
		}
	}

	[ClientRpc]
	public void RpcOnPlayerDataChanged(PlayerData newData) {
		_playerData = newData;
	}

	[Command]
	private void CmdReqeustAMove(Vector2 position, RelativePosition relativePosition) {
		Manager.Find<TurnManager>().ReqeustAMove(playerData, position, relativePosition);
	}

	[ServerCallback]
	private void ReqeustJoinGame() {
		Manager.Find<PlayerManager>().ReqeustJoinGame(this);
	}

	[Command]
	private void CmdReqeustLeaveGame() {
		if(Manager.Find<PlayerManager>()) {
			Manager.Find<PlayerManager>().ReqeustLeaveGame(playerData);
		}
	}
}
