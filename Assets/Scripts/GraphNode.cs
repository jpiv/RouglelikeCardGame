using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTypes {
	public const int BATTLE = 0;
	public const int CHEST = 1;
	public const int INVERSE_DRAFT = 2;
	public const int SOUL = 3;
	public const int SHOP = 4;
	public static int[] rooms = new int[5] { BATTLE, CHEST, INVERSE_DRAFT, SOUL, SHOP };
}

public class GraphNode {
	public ArrayList children = new ArrayList();
	public Vector3 position;
	public int index = -1;
	public int depth;
	public int type;
	public bool visited = false;
	public bool locked = false;
	public bool hidden = false;

	public GraphNode(int index = -1, int depth = 0, int type = 0) {
		this.depth = depth;
		this.type = type;
		this.index = index;
	}

	public void Lock() {
		this.locked = true;
	}

	public void Unlock() {
		this.locked = false;
	}

	public bool HasChild(GraphNode node) {
		return this.children.Contains(node);	
	}

	public GraphNode AttachNode(GraphNode newNode) {
		this.children.Add(newNode);
		return newNode;
	}

	public void SetHidden(bool isHidden) {
		this.hidden =  isHidden;
	}

	public GraphNode AddSibling(int index = -1, int type = 0) {
		return this.AddNode(index, this.depth, type);
	}

	public GraphNode AddLevel(int index = -1, int type = 0) {
		return this.AddNode(index, this.depth + 1, type);
	}

	public GraphNode AddPrevLevel(int index = -1, int type = 0) {
		return this.AddNode(index, this.depth - 1, type);
	}

	private GraphNode AddNode(int index, int depth, int type) {
		GraphNode newNode = new GraphNode(index, depth, type);
		newNode.index = index;
		this.children.Add(newNode);
		return newNode;
	}
}
