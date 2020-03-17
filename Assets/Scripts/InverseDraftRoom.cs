using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InverseDraftRoom : MapIcon {
    public override void OnSelected() {
    	WindowManager.ShowScreen(WindowManager.inverseDraft);
    }
}
