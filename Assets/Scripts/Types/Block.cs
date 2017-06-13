using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Block : NetworkBehaviour {


	[SyncVar]
	public Vector2 pos = new Vector2(0, 0);

	[SyncVar]
	public int ownerID = -1;

	private PlayerData[] linesTaken = new PlayerData[4];

	public PlayerData owner {
		get { return Manager.Find<PlayerManager>().FindPlayerWithID(ownerID); }
	}

	#region neibers
	public Block up {
		get { return Manager.Find<Grid>().GetBlock((int) pos.x, (int) pos.y + 1); }
	}

	public Block left {
		get { return Manager.Find<Grid>().GetBlock((int)pos.x - 1, (int)pos.y); }
	}

	public Block down {
		get { return Manager.Find<Grid>().GetBlock((int)pos.x, (int)pos.y - 1); }
	}

	public Block right {
		get { return Manager.Find<Grid>().GetBlock((int)pos.x + 1, (int)pos.y); }
	}

	public Block[] neibers {
		get { return new Block[] {up, left, down, right};}
	}
#endregion
	 

	public void Set(Vector2 pos) {
		this.pos = pos;  
	}

	public PlayerData GetLineOwner(RelativePosition pos) {
		return linesTaken[(int) pos];
	}

	public PlayerData GetOwner() {
		return owner;
	}

	[ServerCallback]
	public bool SetLine(PlayerData player, RelativePosition pos, bool tellNeiber = true) {
		bool result = false;

		if(tellNeiber && neibers[(int)pos] != null) {
			if(neibers[(int)pos].SetLine(player, pos.Mirror(), false)){
				result = true;
			}
		}

		linesTaken[(int)pos] = player;

		if (CloseCheck(player)) {
			result = true;
		}

		RpcUpdateLineOwner(player, pos);

		return result;
	}

	[ClientRpc]
	public void RpcUpdateLineOwner(PlayerData newOwner, RelativePosition positions) {
		linesTaken[(int) positions] = newOwner;
		TurnLineOn(newOwner, positions);
	}

	[ServerCallback]
	public bool IsClosed() {


		for (int i = 0; i < linesTaken.Length; i++) {
			if (linesTaken[i] == null && neibers[i] == null) {
				return false;
			}

			if (linesTaken[i] == null) {
				return false;
			}
		}

		return true;
	}

	[ServerCallback]
	public List<Block> FindAll(int x, int y, List<Block> found) {

		found.Add(this);

		for (int i = 0; i < linesTaken.Length; i++) {
			if (!found.Contains(neibers[i]) && neibers[i] != null && linesTaken[i] == null) {
				found = neibers[i].FindAll((int)neibers[i].pos.x, (int)neibers[i].pos.y, found);
			}
		}

		return found;
	}


	[ClientRpc]
	public void RpcUpdateOwner(PlayerData player) {
		ownerID = player.networkID;
		GameObject takenObj = transform.Find("taken").gameObject;
		takenObj.GetComponent<SpriteRenderer>().color = owner.color;
	}


	private void TurnLineOn(PlayerData player, RelativePosition pos) {
		GameObject line = transform.Find("lines").GetChild((int)pos).gameObject;
		line.SetActive(true);
		line.GetComponent<SpriteRenderer>().color = player.color;
	}

	[ServerCallback]
	private bool CloseCheck(PlayerData player) {
		if (IsClosed()) {
			List<Block> blocks = FindAll((int) pos.x, (int) pos.y, new List<Block>());
			foreach (Block block in blocks) {
				block.RpcUpdateOwner(player);
			}
			player.score += blocks.Count;
			Manager.Find<PlayerManager>().FindPlayerObjectWithPlayerData(player).RpcOnPlayerDataChanged(player);

			return true;
		}

		return false;
	}

}
