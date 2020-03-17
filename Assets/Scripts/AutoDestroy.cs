using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour {
	void Start() {
		AnimationClip[] clips = GetComponent<Animator>().runtimeAnimatorController.animationClips;
		foreach (AnimationClip clip in clips) {
			AnimationEvent evt = new AnimationEvent();
			evt.functionName = "AutoDestroyObject";
			evt.time = clip.length;
			clip.AddEvent(evt);
		}
	}

	void AutoDestroyObject() {
		Destroy(this.gameObject);
	}
}
