using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Level 1 Drop Rates
    1: 40%
    2: 30%
    3: 20%
    4: 8%
    5: 2%

    Max Level Drop Rates
    1: 10%
    2: 39%
    3: 29%
    4: 14%
    5: 8%
*/
public static class LootTable {
    private static float[] defaultDropModifiers = new float[5] { 1f, 1f, 1f, 1f, 1f  };

    public static float[] CreateForceDropModifiers(int rarity) {
        float[] dropModifiers = new float[5] { 0f, 0f, 0f, 0f, 0f };
        dropModifiers[rarity] = 10000f;
        return dropModifiers;
    }

	public static float[] GetRates(float[] dropModifiers = null) {
        if (dropModifiers == null || dropModifiers.Length < 5) {
            dropModifiers = LootTable.defaultDropModifiers;
        }

        int level = Experience.level;
        int maxLevel = 9;
        float dropInc = ((float)(level - 1) / maxLevel) * 0.30f;
        float r1 = 0.40f - dropInc;
        float r2 = 0.70f - 7 * dropInc / 10f;
        float r3 = 0.90f - 4 * dropInc / 10f;
        float r4 = 0.98f - 2 * dropInc / 10f;
        float r5 = 1;
        return new float[5] {
        	r1 * dropModifiers[0],
    		r2 * dropModifiers[1],
        	r3 * dropModifiers[2],
        	r4 * dropModifiers[3],
        	r5 * dropModifiers[4]
        };
	}

	public static int GetRarity(float[] dropModifiers = null) {
        if (dropModifiers == null || dropModifiers.Length < 5) {
            dropModifiers = LootTable.defaultDropModifiers;
        }

        float chance = Random.Range(0, 1f);
        float[] dropRates = LootTable.GetRates(dropModifiers);
        if (chance <= dropRates[0]) {
            return 0;
        } else if (chance <= dropRates[1]) {
            return 1;
        } else if (chance <= dropRates[2]) {
            return 2;
        } else if (chance <= dropRates[3]) {
            return 3;
        } else if (chance <= dropRates[4]) {
            return 4;
        } else {
            return 0;
        }
	}
}