using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour {
	public GameObject map;
	// Value between 0 and 1 affects zoom sensetivity
	public float zoomScale = 0.05f;

	private Vector3 mouseStart;
	private Vector3 mapStart;

    void Start() {
    	Physics.queriesHitTriggers = false;
    }

    void OnGUI() {
    	float scrollDelta = Input.mouseScrollDelta.y;
    	if (scrollDelta != 0) {
	    	map.transform.localScale *= scrollDelta > 0 ? 1 + zoomScale : 1 - zoomScale;
            LineRenderer[] lines = map.GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer line in lines) {
                line.startWidth *= scrollDelta > 0 ? 1 + zoomScale : 1 - zoomScale;
                line.endWidth *= scrollDelta > 0 ? 1 + zoomScale : 1 - zoomScale;
            }
	    }
    }

    void OnMouseDown() {
    	mouseStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    	mapStart = map.transform.position;
    }

    void OnMouseDrag() {
    	Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) ;
    	map.transform.position = mapStart + (mousePos - mouseStart);
    }
}
