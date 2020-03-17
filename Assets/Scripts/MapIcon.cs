using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapIcon : MonoBehaviour {
	public GraphNode node;
	public bool disabled = false;

	void OnMouseUpAsButton() {
		if (this.disabled || this.node.locked) return;
    	LevelGraph.instance.SetCurrentNode(this.node);
    	LevelGraph.instance.gameObject.SetActive(false);
		this.OnSelected();
	}

	public void Init(GraphNode node) {
		this.node = node;
		if (this.node.visited) {
			this.Disable();
		}
		if (this.node.locked) {
			this.Lock();
		}
	}

	private void Lock() {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float darkenScale = 0.35f;
        sr.color = new Color(
        	sr.color.r * darkenScale,
        	sr.color.g * darkenScale,
        	sr.color.b * darkenScale,
        	sr.color.a * 0.9f
        );
	}

	private void Disable() {
		this.disabled = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(0.6792453f, 0f, 0f, 1f);
	}

	public abstract void OnSelected();
}
