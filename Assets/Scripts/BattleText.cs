using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour {
    public static void CreateText(float damage, GameObject damageAnimation, bool left, Vector3 pos, Transform transform) {
		GameObject damageText = Instantiate(damageAnimation, transform);
		TextMeshPro textComponent = damageText.transform.Find("AnimationContainer/Text").GetComponent<TextMeshPro>();
		textComponent.text = damage.ToString();
		damageText.transform.localPosition = pos;
    	Animator anim = damageText.GetComponent<Animator>();
    	anim.SetBool("left", left);
    }
}
