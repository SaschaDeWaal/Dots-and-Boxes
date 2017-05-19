using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Scores : NetworkBehaviour {

	[SyncVar]
	public int scorePlayer1 = 0;

	[SyncVar]
	public int scorePlayer2 = 0;

	[SyncVar]
	public int scorePlayer3 = 0;

	[SyncVar]
	public int scorePlayer4 = 0;

	[ServerCallback]
	public void AddScore(int index, int ammount) {
		switch (index) {
			case 0:
				scorePlayer1 += ammount;
				break;
			case 1:
				scorePlayer2 += ammount;
				break;
			case 2:
				scorePlayer3 += ammount;
				break;
			case 3:
				scorePlayer4 += ammount;
				break;
		}
	}

	public int[] ScoreArray {
		get { return new[] { scorePlayer1, scorePlayer2, scorePlayer3, scorePlayer4 }; }
	}

}
