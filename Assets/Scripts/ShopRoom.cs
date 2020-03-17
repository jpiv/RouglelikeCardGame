using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopRoom : MapIcon {
    public override void OnSelected() {
    	WindowManager.ShowScreen(WindowManager.shopRoom);
    }
}
