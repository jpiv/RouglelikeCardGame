using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoulRoom : MapIcon {
    public override void OnSelected() {
    	WindowManager.ShowScreen(WindowManager.soulRoom);
    }
}
