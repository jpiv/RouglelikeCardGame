using UnityEngine;

public static class EnemyPositions {
	public static Vector3[] FOUR = new Vector3[4] {
		new Vector3(13.67f, 1.42f, -2f),
		new Vector3(10.18f, 1.98f, -1f),
		new Vector3(12f, 2.77f, 0f),
		new Vector3(15.05f, 2.6f, -0f)
	};

	public static Vector3[] ONE = new Vector3[1] { EnemyPositions.FOUR[3] };
	public static Vector3[] TWO = new Vector3[2] {
		EnemyPositions.FOUR[0],
		EnemyPositions.FOUR[1]
	};
	public static Vector3[] THREE = new Vector3[3] {
		EnemyPositions.FOUR[0],
		EnemyPositions.FOUR[1],
		EnemyPositions.FOUR[2]
	};

	public static Vector3[][] counts = new Vector3[4][] {
		EnemyPositions.ONE,
		EnemyPositions.TWO,
		EnemyPositions.THREE,
		EnemyPositions.FOUR
	};
}