using UnityEngine;
using System;
using System.Collections.Generic;

public class Demo : MonoBehaviour {

	[Header("Reorderable Arrays and Lists")]
	[Reorderable]
	public Vector3[] vectorArray;
	[Reorderable]
	public List<Color> colorList;

	[Header("Range Value Reorderable")]
	[Range(1, 10)]
	[Reorderable]
	public int[] intArray;

	[Header("Float Array Classic")]
	[Range(1, 10)]
	public float[] floatArray;

	[Header("Player List Reorderable")]
	[Reorderable]
	public Player[] playerArray;

	[Header("Player List Classic")]
	public List<Player> playerList;
}

[Serializable]
public class Player {
	public string name;
	public GameObject obj;
	public int life;
	public float damage;
	public int level;
}