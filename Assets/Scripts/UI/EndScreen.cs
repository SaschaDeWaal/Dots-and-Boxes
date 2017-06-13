using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;

public class EndScreen : NetworkBehaviour {

	private const int TOTAL_SCORE = 100;

	public GameObject[] placesUI;

	private PlayerManager playerManager;
	private bool showEndScreen = false;
	
	private void Start () {
		playerManager        = Manager.Find<PlayerManager>();
		transform.localScale = Vector3.zero;
	}

	private void Update () {
		if (isServer && !showEndScreen) {
			CheckScore();
		}
	}

	[ServerCallback]
	private void CheckScore() {
		PlayerData[] newList = playerManager.GetList();
		int totalScore       = 0;

		for(int i = 0; i < newList.Length; i++) {
			totalScore += newList[i].score;
		}

		if (totalScore >= TOTAL_SCORE) {
			RpcSetWinner(SortPlacer(newList));
		}
	}

	[ServerCallback]
	private PlayerData[] SortPlacer(PlayerData[] list) {
		return list.OrderBy((o) => (-o.score)).ToArray<PlayerData>();
	}

	[ClientRpc]
	private void RpcSetWinner(PlayerData[] places) {
		showEndScreen        = true;
		transform.localScale = Vector3.one;

		for(int i = 0; i < placesUI.Length; i++) {
			if (i < places.Length) {
				SetPlayerUI(placesUI[i], places[i].displayName, places[i].avatar, places[i].score);
				placesUI[i].SetActive(true);
			} else {
				placesUI[i].SetActive(false);
			}
		}

		UploadScore(Manager.Find<PlayerManager>().GetLocalPlayer(), places);
	}

	private void UploadScore(PlayerData data, PlayerData[] places) {
		Manager.Find<AccountManager>().UploadScore(data.score, Array.IndexOf(places, data));
	}

	private void SetPlayerUI(GameObject obj, string name, string avatar, int score) {
		obj.transform.Find("name").GetComponent<Text>().text   = name;
		obj.transform.Find("points").GetComponent<Text>().text = score.ToString();

		StartCoroutine(AccountManager.LoadAvatar(avatar, obj.GetComponent<Image>()));
	}


	public void OpenHighscore() {
		Application.OpenURL("http://studenthome.hku.nl/~Sascha.deWaal/land/index.php?p=highScore");
	}

	
}
