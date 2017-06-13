using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Grid : NetworkBehaviour {

	public GameObject boxTemplate;
	private Block[,] blocks;
	private Vector2 center = new Vector2(-1.7f, 0);

	private void Start () {
		CreateGrid(10);
	}

	private void CreateGrid(int size) {
		if (isServer) {

			Debug.Log("CreateGrid");

			int halfSize = Mathf.RoundToInt(size * 0.5f);
			blocks = new Block[size, size];

			//create
			for (int x = 0; x < size; x++) {
				for (int y = 0; y < size; y++) {
					GameObject spawnedObject = Instantiate(boxTemplate, new Vector3(x - halfSize + center.x, y - halfSize + center.y, 0),
						Quaternion.Euler(0, 0, 0));
					Block block = spawnedObject.GetComponent<Block>();
					blocks[x, y] = block;
					block.transform.parent = transform;

					NetworkServer.Spawn(spawnedObject);
				}
			}

			//set
			for (int x = 0; x < size; x++) {
				for (int y = 0; y < size; y++) {
					blocks[x, y]
						.Set(new Vector2(x, y));
				}
			}
		}
	}

	public Block GetBlock(int x, int y) {
		if (x > -1 && x < blocks.GetLength(0) &&
			y > -1 && y < blocks.GetLength(1)) { 
			return blocks[x, y];
		}

		return null;
	}

	private Block FindNeiber(int x, int y, RelativePosition pos) {
		if (y > 0 && pos == RelativePosition.up) { return blocks[x, y-1]; }
		if (x > 0 && pos == RelativePosition.left) { return blocks[x-1, y]; }
		if (y < blocks.GetLength(1)-1 && pos == RelativePosition.down) { return blocks[x, y+1]; }
		if (x < blocks.GetLength(0)-1 && pos == RelativePosition.right) { return blocks[x+1, y]; }

		return null;
	}
}
