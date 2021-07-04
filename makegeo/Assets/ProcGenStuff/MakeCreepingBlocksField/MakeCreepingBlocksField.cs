﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker
// to use:
//	- default empty scene with light and camera
//	- make an empty GameObject, drop this script on it.

// later versions of Unity have a better version of this so
// comment this struct out if your version has it...
public struct Vector2Int {
	public int x; public int y;
	public Vector2Int(int x = 0, int y = 0) { this.x = x; this.y = y; }
}

public class MakeCreepingBlocksField : MonoBehaviour
{
	// TODO: you can put something in here if you like
	GameObject WhatToMake;

	// all things hang off this object
	GameObject Parent;

	Dictionary<Vector2Int, GameObject> Field = new Dictionary<Vector2Int, GameObject>();

	const float BlockWidth = 0.95f;
	const float BlockThickness = 0.2f;
	const float BlockSpacing = 1.00f;

	Vector3 WorldPositionFromV2(Vector2Int pos)
	{
		return new Vector3( pos.x, 0, pos.y) * BlockSpacing;
	}

	void CameraPrep()
	{
		// move the camera up
		var cam = Camera.main;
		cam.transform.position = new Vector3(0, 30, -15);
		cam.transform.rotation = Quaternion.Euler(60, 0, 0);
	}

	void AddBlock(int x, int y)
	{
		var key = new Vector2Int(x, y);

		var copy = Instantiate<GameObject>(WhatToMake, Parent.transform);

		copy.transform.position = WorldPositionFromV2(key);
		copy.SetActive(true);

		// keep track of it so we can join others here
		Field[key] = copy;
	}

	bool TryAddBlockToField()
	{
		// choose an existing cube
		var keys = new List<Vector2Int>( Field.Keys);

		if (keys.Count > 0)
		{
			int which = Random.Range(0, keys.Count);

			var key = keys[which];

			// random neighbor offset
			int x = Random.Range(0, 2) * 2 - 1;
			int y = Random.Range(0, 2) * 2 - 1;

			// enforce only on axes
			if (Random.Range(0, 2) == 0)
			{
				x = 0;
			}
			else
			{
				y = 0;
			}

			// offset from randomly-chosen tile
			x += key.x;
			y += key.y;

			var proposedPosition = new Vector2Int(x, y);

			// is one here already?
			if (Field.ContainsKey(proposedPosition))
			{
				return false;
			}

			// nope, we can add it
			AddBlock(x, y);

			return true;
		}

		// starting block!
		AddBlock(0, 0);
		return true;
	}

	IEnumerator Start()
	{
		// remove this code if you put something in the above slot
		WhatToMake = GameObject.CreatePrimitive(PrimitiveType.Cube);
		WhatToMake.transform.localScale = new Vector3(BlockWidth, BlockThickness, BlockWidth);
		WhatToMake.SetActive(false);

		CameraPrep();       // just for demo

		Parent = new GameObject("Parent");

		while (true)
		{
			// this retry attempts to keep the rate of production steady
			// as long as possible, but eventually it will be hard to
			// place randomly since the field will mostly be hemmed in.
			for (int i = 0; i < 100; i++)
			{
				if (TryAddBlockToField())
				{
					break;
				}
			}

			yield return new WaitForSeconds(0.1f);
		}
	}
}
