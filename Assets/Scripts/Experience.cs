using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Experience : MonoBehaviour {
	public GameObject _text;
	public GameObject _levelText;
	public GameObject _bar;
	public static int level = 1;

	private static TextMeshProUGUI barText;
	private static TextMeshProUGUI levelText;
	private static LineRenderer bar;
	private static int XP = 0;
	private static int maxXP = 100;
	private static Vector3 fullBarPos;

    void Awake() {
    	Experience.barText = _text.GetComponent<TextMeshProUGUI>(); 
    	Experience.levelText = _levelText.GetComponent<TextMeshProUGUI>(); 
    	Experience.bar = _bar.GetComponent<LineRenderer>();
    	fullBarPos = bar.GetPosition(1);
		Canvas canvasChild = this.GetComponentInChildren<Canvas>();
		canvasChild.overrideSorting = true;
		canvasChild.sortingLayerName = "UI";
		canvasChild.sortingOrder = WindowManager.baseUILayer + 2;
	   	UpdateBar();
    }

    public static void AddExp(int exp) {
    	XP += exp;
    	if (XP >= maxXP) {
    		XP -= maxXP;
    		LevelUp();
    	}
    	UpdateTickValue();
    	AnimationQueue.Add(() => {
    		UpdateBar();
		}, 0f);
    }

    private static void UpdateTickValue() {
    }

    private static void LevelUp() {
    	level ++;
    }

    private static void UpdateBar() {
		float barStartPos = bar.GetPosition(0).y;
		float barEndPosition = barStartPos + ((float)XP / maxXP) * (fullBarPos.y - barStartPos);
		bar.SetPosition(1, new Vector3(fullBarPos.x, barEndPosition, fullBarPos.z));
		barText.text = XP.ToString() + "/" + maxXP.ToString();
		levelText.text = level.ToString();
    }
}
