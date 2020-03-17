using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemonicInfluence : MonoBehaviour {
	public GameObject _text;
	public GameObject _tickText;
	public GameObject _bar;
	public GameObject _debuffBar;

	// Active Debuffs
	public static bool _fearOfDemons = false;
	public static bool _damned = false;

	private static TextMeshProUGUI barText;
	private static TextMeshProUGUI tickText;
	private static GameObject DebuffBar;
	private static LineRenderer bar;
	private static int time = 1;
	private static int tick = 1;
	private static int DI = 0;
	private static int maxDI = 100;
	private static Vector3 fullBarPos;

    void Awake() {
    	DemonicInfluence.barText = _text.GetComponent<TextMeshProUGUI>(); 
    	DemonicInfluence.bar = _bar.GetComponent<LineRenderer>();
    	DemonicInfluence.tickText = _tickText.GetComponent<TextMeshProUGUI>();
    	DemonicInfluence.DebuffBar = _debuffBar;
    	fullBarPos = bar.GetPosition(1);
		Canvas canvasChild = this.GetComponentInChildren<Canvas>();
		canvasChild.overrideSorting = true;
		canvasChild.sortingLayerName = "UI";
		canvasChild.sortingOrder = WindowManager.baseUILayer + 2;
	   	UpdateBar();
    }

    // Reduce Healing on player
    public static float Damned(float amount) {
    	float healModifier = _damned ? 0.65f : 1f;
    	return amount * healModifier;
    }

    // Enemies sometimes attack first
    public static bool FearOfDemons() {
    	if (!_fearOfDemons) return false;
    	return Random.Range(0f, 1f) <= 0.25f;
    }

    public static void Tick() {
    	DI = Mathf.Min(DI + tick, maxDI);
    	UpdateTickValue();
    	AnimationQueue.Add(() => {
    		UpdateBar();
	    	ApplyDebuffs();
		}, 0f);
    	time = Mathf.Min(time + 1, 14);
    }

    private static void ApplyDebuffs() {
    	App.ClearChildren(DebuffBar.transform);	
    	if (DI >= 25) {
	    	Tooltip tooltip = Creator.CreateDIDebuff("RedBuff", DebuffBar.transform).GetComponent<Tooltip>();
	    	DemonicInfluence._fearOfDemons = true;
			tooltip.SetText("<color=yellow>Fear of Demons</color> - Occasionally enemies will attack before the player when entering a combat room.");
	    }
	    if (DI >= 50) {
	    	Tooltip tooltip = Creator.CreateDIDebuff("RedBuff", DebuffBar.transform).GetComponent<Tooltip>();
	    	DemonicInfluence._damned = true;
			tooltip.SetText("<color=yellow>Damned</color> - Healing effects reduced by 35%.");
	    }
    }

    private static int Fib(int n) {
    	if (n == 0) return 0;
    	return n > 1 ? Fib(n - 1) + Fib(n - 2) : 1;
    }

    private static void UpdateTickValue() {
    	tick = Fib(time + 2);
    }

    private static void UpdateBar() {
		float barStartPos = bar.GetPosition(0).x;
		float barEndPosition = barStartPos + ((float)DI / maxDI) * (fullBarPos.x - barStartPos);
		bar.SetPosition(1, new Vector3(barEndPosition, fullBarPos.y, fullBarPos.z));
		barText.text = DI.ToString() + "/" + maxDI.ToString();
		tickText.text = tick.ToString();
    }
}
