using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerOverview : NetworkBehaviour {

	public GameObject playerUITemplate;

	private PlayerManager playerManager;
	private TurnManager turnManager;

	private PlayerData[] players = new PlayerData[0];
	private PlayerData lastTurn;

	private void Start() {
		playerManager = Manager.Find<PlayerManager>();
		turnManager   = Manager.Find<TurnManager>();
	}

	public void Update() {
		if (!players.Equals(playerManager.GetList())) {
			players = playerManager.GetList();
			StartCoroutine(CreateList());
		}

		if (isServer) {
			PlayerData turnToPlayer = turnManager.TurnToPlayer;

			if (turnToPlayer != lastTurn) {
				lastTurn = turnToPlayer;
				RpcTurnChanged(turnToPlayer);
			}
		}
	}

	[ClientRpc]
	public void RpcTurnChanged(PlayerData player) {
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild(i).gameObject;
			child.transform.Find("Turn").gameObject.SetActive(child.name == player.playerID.ToString());

			if(child.name == player.playerID.ToString()) {
				child.transform.Find("Score").GetComponent<Text>().text = player.score.ToString();
			}
		}

		Debug.Log(player.ToString() + " <<");
	}

	private IEnumerator CreateList() {
		for(int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}

		yield return null;

		foreach(PlayerData player in players) {
			GameObject newUI  = Instantiate(playerUITemplate, transform);
			bool playerIsTurn = lastTurn != null && lastTurn.displayName == player.displayName;

			newUI.name                                                   = player.playerID.ToString();
			newUI.GetComponent<Image>().color                            = player.color;
			newUI.transform.Find("Name").GetComponent<Text>().text  = player.displayName ;
			newUI.transform.Find("Score").GetComponent<Text>().text = player.score.ToString();

			newUI.transform.Find("Turn").gameObject.SetActive(playerIsTurn);
		}

		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
	}



}
