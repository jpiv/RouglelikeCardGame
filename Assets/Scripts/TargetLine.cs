using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class TargetLine : MonoBehaviour {
	public static TargetLine instance;

	private LineRenderer line;
	private bool isLineShowing = false;
	private int numPositions = 20;
	private float amplitutde = 2f;

	void Awake() {
		instance = this;
	}

    void Start() {
    	this.line = GetComponent<LineRenderer>(); 
    	this.line.positionCount = this.numPositions;
    }

    void Update() {
    	if (this.isLineShowing) {
	    	this.UpdateLine(); 
    	}
    }

    public void Show() {
    	this.isLineShowing = true;
    	this.line.positionCount = this.numPositions;
    }

    public void Hide() {
    	this.isLineShowing = false;
    	this.line.positionCount = 0;
    }

    void UpdateLine() {
    	Vector3 targetPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	Vector3 startPos = this.transform.position;
    	Func<float, int, float> yPosFn = (x, len) => -Mathf.Pow(x / (0.5f * (len - 1)) - 1, 2f) + 1;

    	for (float i = 0; i < numPositions; i++) {
    		float progress = i / (numPositions - 1);
    		float yOffset = startPos.y + (targetPoint.y - startPos.y) * progress;
    		Vector3 position = new Vector3(
    			startPos.x + (targetPoint.x - startPos.x) * progress, 
    			yPosFn(i, numPositions) * this.amplitutde + yOffset, 
    			-10
    		);
    		this.line.SetPosition((int)i, position);
    	}
    }
}
