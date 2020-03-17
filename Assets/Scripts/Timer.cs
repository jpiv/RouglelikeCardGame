using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    private static Timer instance;
	private static List<Action> queue = new List<Action>();
	private static float queueTime = 1.5f;
	private static float time = 0;
	private static bool timeRunning = false;
	private static Action OnComplete;

    void Awake() {
        instance = this;
    }

    void Update() {
   		if (timeRunning) {
   			Timer.time += Time.deltaTime;
   			if (Timer.time >= Timer.queueTime) {
   				Timer.time = 0f;
   				Timer.Dequeue();
   			}
   		}
    }

    private static void Dequeue() {
    	Timer.queue[0]();
    	Timer.queue.RemoveAt(0);
    	if (Timer.queue.Count == 0) {
    		Timer.timeRunning = false;
    		Timer.OnComplete();
    	}
    }

    public static void TimeoutAction(Action action, float timeout) {
        Timer.instance.StartCoroutine(Timer.NewTimeoutAction(action, timeout));
    }

    private static IEnumerator NewTimeoutAction(Action action, float timeout) {
        yield return new WaitForSeconds(timeout);
        action();
    }

    public static void RunQueue(Action OnComplete) {
    	if (Timer.queue.Count == 0) {
    		OnComplete();
    		return;
    	} 
        Timer.Dequeue();
    	Timer.timeRunning = true;
    	Timer.OnComplete = OnComplete;
    }

    public static void SetQueueTime(float time) {
    	Timer.queueTime = time;
    }

    public static void CreateQueue(List<Action> items) {
    	if (items.Count == 0) return;
    	// Delay for one queue unit at end
    	items.Add(() => {});
    	Timer.queue = items;
    }
}
