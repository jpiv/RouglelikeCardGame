using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour {
	public static float playerHealth;
	public static int bones;
	public static int souls;
	public static int exp;

	public static int GetPowerStat() {
		return Items.instance.armor.stats.power;
	}

	public static void AddBones(int bones) {
		Store.bones += bones;
		CurrencyUI.UpdateBonesText(Store.bones);
	}

	public static void AddSouls(int souls) {
		Store.souls += souls;
		CurrencyUI.UpdateSoulsText(Store.souls);
	}

	public static void AddExp(int exp) {
		Store.exp += exp;
		Experience.AddExp(exp);
	}

	public static void Reset() {
		Store.playerHealth = Hero.maxHeroHP;
	}
}
