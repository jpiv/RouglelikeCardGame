using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : MonoBehaviour {
	public List<GameObject> _allEnemies = new List<GameObject>();
	public List<GameObject>_allBosses = new List<GameObject>();
	public static List<EnemyType> allEnemyTypes;
	public static List<GameObject> allEnemyPrefabs;
	public static List<GameObject> allBosses;


	void Awake() {
    	TextAsset enemyDataText = Resources.Load<TextAsset>("enemies");
    	EnemiesType enemyData = JsonUtility.FromJson<EnemiesType>(enemyDataText.text);
		Enemies.allEnemyTypes = enemyData.enemies;
		Enemies.allEnemyPrefabs = this._allEnemies;
		Enemies.allBosses = this._allBosses;
	}

	private static GameObject GetEnemyPrefabByName(string name) {
		return Enemies.allEnemyPrefabs.Find(e => e.GetComponentInChildren<Enemy>().enemyName == name);
	}

    public static Enemy CreateEnemy(Vector3 pos, GameObject enemyPre, EnemyType? type = null) {
    	if (type == null) type = allEnemyTypes[0];
        GameObject enemyObj = Instantiate(enemyPre, Battlefield.instance.transform);
		Enemy enemy  = enemyObj.transform.GetChild(0).GetComponent<Enemy>();
		enemy.SetType((EnemyType)type);
    	enemyObj.transform.localPosition = pos; 
    	return enemy;
    }

	public static EnemyType RandomEnemy() {
		EnemyType type = Enemies.allEnemyTypes[Random.Range(0, Enemies.allEnemyTypes.Count)];
		return type;
	}

	public static GameObject RandomBoss() {
		return Enemies.allBosses[Random.Range(0, Enemies.allBosses.Count)];
	}

	public static Enemy SpawnBoss(Vector3 pos) {
		GameObject bossPre = Enemies.RandomBoss();
		Enemy boss = Enemies.CreateEnemy(pos, bossPre);
		return boss;
	}

	public static List<Enemy> SpawnEnemies(Vector3[] positions) {
		List<Enemy> spawnedEnemies = new List<Enemy>();
		for (int i = 0; i < positions.Length; i++) {
			Vector3 pos = positions[i];
			EnemyType type = Enemies.RandomEnemy();
			GameObject enemyPre = GetEnemyPrefabByName(type.name);
			Enemy enemy = Enemies.CreateEnemy(pos, enemyPre, type);
			spawnedEnemies.Add(enemy);
		}
		return spawnedEnemies;
	}
}

public struct EnemiesType {
	public List<EnemyType> enemies;
}
