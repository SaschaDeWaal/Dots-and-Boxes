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
	private Scores scores;

	private Player[] players = new Player[0];
	private Player lastTurn;

	private void Start() {
		playerManager = Manager.Find<PlayerManager>();
		turnManager   = Manager.Find<TurnManager>();
		scores        = Manager.Find<Scores>();
	}

	public void Update() {
		if (!players.Equals(playerManager.GetList())) {
			players = playerManager.GetList();
			StartCoroutine(CreateList());
		}

		if (isServer) {
			Player turnToPlayer = turnManager.TurnToPlayer;

			if (turnToPlayer != lastTurn) {
				lastTurn = turnToPlayer;
				RpcTurnChanged(turnToPlayer);
			}
		}
	}

	[ClientRpc]
	public void RpcTurnChanged(Player player) {
		for (int i = 0; i < transform.childCount; i++) {
			GameObject child = transform.GetChild(i).gameObject;
			child.transform.FindChild("Turn").gameObject.SetActive(child.name == player.displayName);
			child.transform.FindChild("Score").GetComponent<Text>().text = scores.ScoreArray[i].ToString();
		}
	}

	private IEnumerator CreateList() {
		for(int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}

		yield return null;

		foreach(Player player in players) {
			GameObject newUI  = Instantiate(playerUITemplate, transform);
			bool playerIsTurn = lastTurn != null && lastTurn.displayName == player.displayName;

			newUI.name                                                   = player.displayName;
			newUI.GetComponent<Image>().color                            = player.color;
			newUI.transform.FindChild("Name").GetComponent<Text>().text  = player.displayName;
			newUI.transform.FindChild("Score").GetComponent<Text>().text = scores.ScoreArray[player.playerID].ToString();

			newUI.transform.FindChild("Turn").gameObject.SetActive(playerIsTurn);
		}

		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
	}



}
