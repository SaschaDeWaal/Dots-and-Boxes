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

	private Player[] linesTaken = new Player[4];

	public Player owner {
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

	public Player GetLineOwner(RelativePosition pos) {
		return linesTaken[(int) pos];
	}

	public Player GetOwner() {
		return owner;
	}

	[ServerCallback]
	public void SetLine(Player player, RelativePosition pos, bool tellNeiber = true) {
		if(tellNeiber && neibers[(int)pos] != null) {
			neibers[(int)pos].SetLine(player, pos.Mirror(), false);
		}

		linesTaken[(int)pos] = player;

		CloseCheck(player);

		RpcUpdateLineOwner(player, pos);
	}

	[ClientRpc]
	public void RpcUpdateLineOwner(Player newOwner, RelativePosition positions) {
		linesTaken[(int) positions] = newOwner;
		TurnLineOn(newOwner, positions);
	}

	[ServerCallback]
	public bool IsClosed(List<Block> checkedBlocks) {

		checkedBlocks.Add(this);

		for (int i = 0; i < linesTaken.Length; i++) {
			if (linesTaken[i] == null && neibers[i] == null) {
				return false;
			}

			if (linesTaken[i] == null && !checkedBlocks.Contains(neibers[i])) {
				if (!neibers[i].IsClosed(checkedBlocks)) return false;
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
	public void RpcUpdateOwner(Player player) {
		ownerID = player.networkID;
		GameObject takenObj = transform.FindChild("taken").gameObject;
		takenObj.GetComponent<SpriteRenderer>().color = owner.color;
	}


	private void TurnLineOn(Player player, RelativePosition pos) {
		GameObject line = transform.FindChild("lines").GetChild((int)pos).gameObject;
		line.SetActive(true);
		line.GetComponent<SpriteRenderer>().color = player.color;
	}

	[ServerCallback]
	private void CloseCheck(Player player) {
		if (IsClosed(new List<Block>())) {
			List<Block> blocks = FindAll((int) pos.x, (int) pos.y, new List<Block>());
			foreach (Block block in blocks) {
				block.RpcUpdateOwner(player);
			}
			Manager.Find<Scores>().AddScore(player.playerID, blocks.Count);
		}
	}

}
