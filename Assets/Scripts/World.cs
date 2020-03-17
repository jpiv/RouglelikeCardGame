using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {
	public static World instance;

    void Start() {
    	instance = this;
    	this.GenerateMap();
    }

    public void GenerateMap() {
    	LevelGraph.instance.RenderMap();
    }
}
