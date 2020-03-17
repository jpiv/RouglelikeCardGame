using System;
using System.Collections.Generic;
using UnityEngine;

public class AnimationItem {
	public int id;
	public bool isRunning = false;
	public bool complete = false;
	public Action StartAnimation;

	public AnimationItem(int id, Action StartAnimation) {
		this.StartAnimation = () => {
			this.isRunning = true;
			StartAnimation();
		};
		this.id = id;
	}
}

public class AnimationQueue {
	private static int nextId = 0;
	private static List<AnimationItem> queue = new List<AnimationItem>();

	// Add wait time override for simultaneous animations?
	public static Action Add(Action StartAnimation,  float wait = -1) {
		Action OnComplete = null;
		Action Start = () => {
			if (wait != -1) {
				Timer.TimeoutAction(OnComplete, wait);
			}
			StartAnimation();
		};
		AnimationItem item = new AnimationItem(nextId, Start);
		OnComplete = () => AnimationQueue.OnComplete(item);
		queue.Add(item);
		nextId++;

		if (queue.Count == 1) {
			item.StartAnimation();
		}
		return OnComplete;
	}

	private static void OnComplete(AnimationItem completedItem) {
		if (queue.Count == 0) return;
		AnimationItem item = queue[0];
		if (item.id != completedItem.id) {
			completedItem.complete = true;
		} else {
			queue.RemoveAt(0);
			if (queue.Count > 0 && !queue[0].isRunning) {
				AnimationItem nextItem = queue[0];
				nextItem.StartAnimation();
				if (nextItem.complete) {
					AnimationQueue.OnComplete(nextItem);
				}
			}
		}
	}

	public static void Clear() {
		queue.Clear();
		nextId = 0;
	}
}