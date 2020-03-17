using System;
using System.Collections.Generic;

public class AnimationItem {
	public int id;
	public bool isRunning = false;
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
	private static int completeId = 0;
	private static List<AnimationItem> queue = new List<AnimationItem>();

	// Add wait time override for simultaneous animations?
	public static Action Add(Action StartAnimation,  float wait = 0) {
		Action OnComplete = () => AnimationQueue.OnComplete(nextId);
		AnimationItem item = new AnimationItem(nextId, StartAnimation);
		queue.Add(item);
		nextId++;

		if (queue.Count == 1) {
			item.StartAnimation();
		}
		if (wait != 0) {
			Timer.TimeoutAction(OnComplete, wait);
		}
		return OnComplete;
	}

	private static void OnComplete(int id) {
		queue.RemoveAt(0);
		if (queue.Count > 0 && !queue[0].isRunning) {
			queue[0].StartAnimation();
		}
	}
}