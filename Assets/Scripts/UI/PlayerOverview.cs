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

	private Text[] scoreText        = new Text[0];

	private void Start() {
		playerManager = Manager.Find<PlayerManager>();

		if (isServer) turnManager = Manager.Find<TurnManager>();
	}

	public void Update() {
		PlayerData[] newList = playerManager.GetList();


		if (players.Length != newList.Length || newList.Length != transform.childCount) {
			players = newList;
			CreateOrUpdateList();
		} else {
			players = newList;
			UpdateScore();
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
			

			if (child.name == player.playerID.ToString()) {
				child.transform.Find("Score").GetComponent<Text>().text = player.score.ToString();
			}
		}
	}

	//Update the score every frame because there can be a delay before the new score is recived
	private void UpdateScore() {
		for(int i = 0; i < scoreText.Length; i++) {
			scoreText[i].text = players[i].score.ToString();
			bool state = lastTurn != null && lastTurn.networkID == players[i].networkID;
		}
	}

	private void CreateOrUpdateList() {

		//stop current downloading of avatarts
		StopCoroutine("LoadAvatar");

		//remove all UI elements
		while(transform.childCount > 0) { 
			DestroyImmediate(transform.GetChild(0).gameObject);

		}

		//prepare variables for faster acces
		scoreText  = new Text[players.Length];

		//Loop thought all players and add them to the UI
		int index = 0;
		foreach (PlayerData player in players) {
			GameObject newUI  = Instantiate(playerUITemplate, transform);
			bool playerIsTurn = lastTurn != null && lastTurn.networkID == player.networkID;

			scoreText[index]  = newUI.transform.Find("Score").GetComponent<Text>();

			newUI.name                                                   = player.playerID.ToString();
			newUI.GetComponent<Image>().color                            = player.color;
			newUI.transform.Find("Name").GetComponent<Text>().text       = player.displayName;
			scoreText[index].text                                        = player.score.ToString();

			newUI.transform.Find("Turn").gameObject.SetActive(playerIsTurn);

			StartCoroutine(AccountManager.LoadAvatar(player.avatar, newUI.transform.Find("Avatar").GetComponent<Image>()));
			index++;
		}

		//Set offset
		RectTransform rectTransform = GetComponent<RectTransform>();
		rectTransform.offsetMin = Vector2.zero;
		rectTransform.offsetMax = Vector2.zero;
	}
}
