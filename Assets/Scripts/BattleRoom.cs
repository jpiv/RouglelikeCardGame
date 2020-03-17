using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleRoom : MapIcon {
    public override void OnSelected() {
    	SceneHelper.Open(SceneHelper.BATTLEFIELD);
    }
}
