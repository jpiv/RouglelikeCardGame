using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGraph : MonoBehaviour {
	public static LevelGraph instance;
	public GameObject battleRoomPre;
    public GameObject pathLine;
    public GameObject pathLineVisited;
	public GraphNode currentNode;

	private GraphNode spawnNode;
	private delegate void SearchAction(ArrayList nodeList);
    private int[] layerPaths = new int[4];
    private int mapLayer = 0;

	public void Awake() {
		instance = this;
        this.MakeStartingNodes();
		// this.FillMap();
	}

    public int GetCurrentRealDepth() {
        return this.GetRealDepth(this.currentNode.depth);
    }

    public void SetCurrentNode(GraphNode node) {
        int currentDepth = this.GetRealDepth(this.currentNode.depth);
        int nextNodeDepth = this.GetRealDepth(node.depth);
        if (nextNodeDepth > currentDepth) {
            this.LockDepths();
        }
        this.currentNode = node;
        this.ScaleGame();
    }

    private void ScaleGame() {
        int realDepth = this.GetRealDepth(this.currentNode.depth);
        Enemy.SetScale((realDepth - 1) * 0.2f);
    }

    private void LockDepths() {
        SearchAction action = nodeList => {
            foreach (GraphNode node in nodeList) {
                if (this.GetRealDepth(node.depth) <= this.GetRealDepth(this.currentNode.depth)) {
                    node.Lock();
                }
            }
        };
        this.SearchGraph(action);
    }

    private void MakeStartingNodes() {
        spawnNode = new GraphNode(0);
        spawnNode.visited = true;
        this.currentNode = spawnNode;
        this.LockDepths();


        // Add initial routes
        GraphNode n;
        n = spawnNode.AddLevel(0);
        n = spawnNode.AddLevel(1);
        n = spawnNode.AddLevel(2);
        n = spawnNode.AddLevel(3);
    }

    private void GotoNextDepth() {
        if(this.mapLayer >= 2) {
            App.ResetGame();
        }
        this.layerPaths = new int[4];
        App.ClearChildren(this.transform);
        this.MakeStartingNodes();
        this.mapLayer++;
    }

	public void ReturnToMap() {
        DemonicInfluence.Tick();
        this.gameObject.SetActive(true);
        WindowManager.ClearOverlays();
        Deck.instance.Reset();
        AnimationQueue.Clear();
        SceneHelper.Close(SceneHelper.BATTLEFIELD);
        SceneHelper.Close(SceneHelper.CHEST);
        CardWindow.instance.PopulateFullDeck();
		this.ExploreFrom(this.currentNode);
        if (this.IsFinalRoom()) {
            this.GotoNextDepth();
        }
		this.RenderMap();
	}

	public void RenderMap() {
		foreach (Transform child in this.transform) {
			Destroy(child.gameObject);
		}
	    SearchAction action = nodeList => {
	    	float xOffset = 0;
	    	float yOffset = 0;
	    	for (int i = 0; i < nodeList.Count; i ++) {
	    		GraphNode node = (GraphNode)nodeList[i];
		    	GameObject nextMapNode = Creator.CreateRoom(node, this.transform);
		    	if (node.position != Vector3.zero) {
		    		nextMapNode.transform.localPosition = node.position;
		    	} else {
                    Vector2 nodePos = this.GetNodePos(node);
			    	nextMapNode.transform.localPosition = new Vector3(8f + nodePos.x + xOffset, 4.5f + nodePos.y + yOffset, 0);
			    	node.position = nextMapNode.transform.localPosition;
		    	}
		    }
    	};
    	this.SearchGraph(action);
        this.DrawLines();
    }

    private Vector2 GetNodePos(GraphNode node) {
        float radius = this.GetRadius(node.depth);
        float ratio = (float)node.index / this.GetLayerSize(node.depth);
        float theta = Mathf.Deg2Rad * (ratio * 360);
        float xPos = radius * Mathf.Cos(theta);
        float yPos = radius * Mathf.Sin(theta);
        return new Vector2(xPos, yPos);
    }

    private int GetRealDepth(int depth) {
        return depth / 2 + depth % 2;
    }

    private float GetRadius(int depth) {
        if (depth == 0) return 0;
        float depthMultiplier = 3f;
        float layerSpacing = 1.3f;
        return (float)this.GetRealDepth(depth) * depthMultiplier + this.GetRealDepth(depth - 1) * layerSpacing;
    }

    private void CreateLine(Vector3[] positions, GraphNode fromNode, GraphNode toNode, bool visited = false) {
        float width = 0.085f;
        float darkenScale = 0.5f;
        GameObject pre = visited ? this.pathLineVisited : this.pathLine;
        GameObject lr = Instantiate(pre, this.transform);
        LineRenderer line = lr.GetComponent<LineRenderer>();
        lr.transform.localPosition = new Vector3(8f, 4.5f, 0);
        line.startWidth = width;
        line.endWidth = width;
        line.useWorldSpace = false;
        line.positionCount = positions.Length;
        line.SetPositions(positions);
        if (fromNode.locked) {
            line.startColor = new Color(line.startColor.r, line.startColor.g, line.startColor.b, line.startColor.a * darkenScale);
        }
        if (toNode.locked) {
            line.endColor = new Color(line.endColor.r, line.endColor.g, line.endColor.b, line.endColor.a * darkenScale);
        }
    }

    private void DrawLines() {
        SearchAction action = nodeList => {
            foreach (GraphNode node in nodeList) {
                Vector2 pos1 = this.GetNodePos(node);
                foreach (GraphNode child in node.children) {
                    Vector2 pos2 = this.GetNodePos(child);
                    float midX = pos1.x + (pos2.x - pos1.x) / 2;
                    float midY = pos1.y + (pos2.y - pos1.y) / 2;
                    Vector3[] positions = new Vector3[3] {
                        new Vector3(pos1.x, pos1.y, 0),
                        new Vector3(midX, midY, 0),
                        new Vector3(pos2.x, pos2.y, 0)
                    };
                    bool visited = node.visited && child.visited;
                    this.CreateLine(positions, node, child, visited);
                }
            }
        };
        this.SearchGraph(action);
    }

    private int GetNodeType() {
        float chance = Random.Range(0, 1f);
        if (chance <= 0.10f) {
            return NodeTypes.INVERSE_DRAFT;
        } else if (chance <= 0.20f) {
            return NodeTypes.CHEST;
        } else if (chance <= 0.30f) {
            return NodeTypes.SOUL;
        } else if (chance <= 0.40f) {
            return NodeTypes.SHOP;
        } else {
            return NodeTypes.BATTLE;
        }
    }

    private bool HasEdge(GraphNode n1, GraphNode n2) {
        return n1.children.Contains(n2) || n2.children.Contains(n1);
    }

    private bool NodesOnSamePlane(GraphNode n1, GraphNode n2) {
        float ratio1 = (float)n1.index / this.GetLayerSize(n1.depth);
        float theta1 = Mathf.Deg2Rad * (ratio1 * 360);
        float ratio2 = (float)n2.index / this.GetLayerSize(n2.depth);
        float theta2 = Mathf.Deg2Rad * (ratio2 * 360);
        return theta1 == theta2;
    }

    private void ExploreNodeAt(GraphNode node, int depth, int index) {
        GraphNode nextNode = this.GetNodeAt(depth, index);
        int nextRealDepth = this.GetRealDepth(depth);
        if (node.depth == depth) {
            if (nextNode == null) {
                node.AddSibling(index, this.GetNodeType());
            } else if (!this.HasEdge(node, nextNode)) {
                node.AttachNode(nextNode);
            }
        } else {
            if (nextNode == null && depth > node.depth) {
                // Chance to generate path to next layer
                bool spawnPath = Random.Range(0, 1f) <= 0.5f;
                if (nextRealDepth > this.GetRealDepth(node.depth)) {
                    if (!spawnPath || layerPaths[nextRealDepth - 1] > 2) return;
                    int nodeType = this.IsFinalRoom(depth) ? NodeTypes.BATTLE : this.GetNodeType();
                    GraphNode newNode = node.AddLevel(index, nodeType);
                    newNode.SetHidden(true);
                    layerPaths[nextRealDepth - 1] ++;
                } else {
                    node.AddLevel(index, this.GetNodeType());
                }
            } else if (nextNode == null && depth < node.depth && this.GetRealDepth(node.depth) == nextRealDepth) {
                node.AddPrevLevel(index, this.GetNodeType());
            } else if (nextNode != null
                &&!this.HasEdge(node, nextNode)
                && this.NodesOnSamePlane(node, nextNode)
                && this.GetRealDepth(node.depth) == nextRealDepth) {
                node.AttachNode(nextNode);
            }
        }
    }

    public bool IsFinalRoom(int depth = -1) {
        if (depth == -1) depth = this.currentNode.depth;
        return this.GetRealDepth(depth) == 4;
    }

    public void ExploreFrom(GraphNode node) {
    	if (!node.visited) {
	    	node.visited = true;
            node.SetHidden(false);
            if (this.IsFinalRoom()) return;
    		int layerSize = this.GetLayerSize(node.depth);
    		int nextIndex = this.GetLayerIndex(node, node.depth + 1);
            int previousIndex = this.GetLayerIndex(node, node.depth - 1);
            int rightIndex = Mathf.Min(node.index + 1, layerSize - 1);
            int leftIndex = Mathf.Max(node.index - 1, 0);
            // Explore node in next layer
            this.ExploreNodeAt(node, node.depth + 1, nextIndex);
            // Explore node to left
            this.ExploreNodeAt(node, node.depth, leftIndex);
            // Explore node to right
            this.ExploreNodeAt(node, node.depth, rightIndex);
            // Explore node in previous layer
            this.ExploreNodeAt(node, node.depth - 1, previousIndex);
	    }
    }

    // Gets the index that corresponds to the given node in another layer
    private int GetLayerIndex(GraphNode node, int layer) {
		int layerSize = this.GetLayerSize(node.depth);
		int nextLayerSize = this.GetLayerSize(layer);
		int nextIndex = (int)(((float)node.index / layerSize) * nextLayerSize);
		return nextIndex;
    }

    private int GetLayerSize(int depth) {
    	if (depth == 0) {
    		return 1;
    	} else if (depth == 1) {
    		return 4;
    	} else {
    		return 12;
    	}
    }

    private void SearchGraph(SearchAction action) {
    	ArrayList rootNode = new ArrayList();
        HashSet<GraphNode> traversed = new HashSet<GraphNode>();
    	rootNode.Add(this.spawnNode);
    	this._SearchGraph(rootNode, action, traversed);
    }

    // private void _SearchGraph(ArrayList rootNodes, SearchAction action, List<List<GraphNode>> edgeList) {
    private void _SearchGraph(ArrayList rootNodes, SearchAction action, HashSet<GraphNode> traversed) {
    	action(rootNodes);
    	ArrayList nextNodes = new ArrayList();
    	foreach (GraphNode node in rootNodes) {
            traversed.Add(node);
            foreach (GraphNode child in node.children) {
                if (!traversed.Contains(child)) {
                    nextNodes.Add(child);
                }
            }
    	}
    	if (nextNodes.Count > 0) this._SearchGraph(nextNodes, action, traversed);
    }

    private GraphNode GetNodeAt(int depth, int index) {
    	GraphNode foundNode = null;
    	SearchAction action = nodeList => {
    		foreach(GraphNode node in nodeList) {
    			if (node.depth == depth && node.index == index) {
    				foundNode = node; 
    			}
    		}
    	};
    	this.SearchGraph(action);
    	return foundNode;
    }

    private void FillMap() {
    	GraphNode n1 = null;
    	int index = 0;
    	for (int i = 0; i < this.currentNode.children.Count; i++) {
	    	GraphNode node = (GraphNode)this.currentNode.children[i];
	    	index = this.GetLayerIndex(node, node.depth + 1);
	    	n1 = node.AddLevel(index);
    	}
    	for (int i = 0; i < 12; i++) {
    		GraphNode foundNode = this.GetNodeAt(2, i);
    		if (foundNode == null) {
    			n1 = n1.AddSibling(i);
    		} else {
    			n1 = foundNode;
    		}
    	}
    	index = this.GetLayerIndex(n1, n1.depth + 1);
    	n1 = n1.AddLevel(index);
    	for (int i = 0; i < 12; i++) {
    		GraphNode foundNode = this.GetNodeAt(3, i);
    		if (foundNode == null) {
    			n1 = n1.AddSibling(i);
    		} else {
    			n1 = foundNode;
    		}
    	}
    	index = this.GetLayerIndex(n1, n1.depth + 1);
    	n1 = n1.AddLevel(index);
    	for (int i = 0; i < 12; i++) {
    		GraphNode foundNode = this.GetNodeAt(4, i);
    		if (foundNode == null) {
    			n1 = n1.AddSibling(i);
    		} else {
    			n1 = foundNode;
    		}
    	}
    }
}
