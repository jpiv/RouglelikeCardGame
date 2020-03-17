using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHelper {
	public const string BATTLEFIELD = "Battlefield";
	public const string MAP = "MainScene";
	public const string CHEST = "ChestRoom";

	private static string[] scenes = new string[3] { BATTLEFIELD, CHEST, MAP };

	public static void Open(string sceneName) {
		AnimationQueue.Clear();
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
	}

	public static void Close(string sceneName) {
		if (SceneHelper.IsLoaded(sceneName)) {
			SceneManager.UnloadSceneAsync(sceneName);
		}
	}

	public static bool IsLoaded(string sceneName) {
		bool isLoaded = SceneManager.GetSceneByName(sceneName).IsValid();
		return isLoaded;
	}

	public static void CloseAll() {
		foreach (string sceneName in scenes) {
			if (SceneHelper.IsLoaded(sceneName)) {
				SceneHelper.Close(sceneName);
			}
		}
	}
}
