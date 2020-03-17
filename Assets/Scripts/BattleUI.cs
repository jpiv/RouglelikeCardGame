using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUI : MonoBehaviour {
    void Start() {
		this.AddGear();        
    }

    void AddGear() {
    	string image = Items.instance.armor.image;
    	Vector3 pos = new Vector3(
    		-382.4764f,
    		-140.5189f,
    		0f
    	);
    	Vector3 scale = new Vector3(
	    	5.585836f,
	    	6.325902f,
	    	0f
    	);
    	Creator.CreateArmor(image, pos, scale, this.transform);	
    }
}
