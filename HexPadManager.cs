using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPadManager : MonoBehaviour
{
	public enum Orientation
	{
		up,
		down,
		left,
		right,
		towards,
		away
	}

	public GameObject template;

	private List<GameObject> hexagons = new List<GameObject>();

	private Quaternion GetOrientation(Orientation dir)
	{
		switch (dir)
		{
			case Orientation.up:
				return Quaternion.Euler(90f, 0f, 0f);
			case Orientation.down:
				return Quaternion.Euler(-90f, 0f, 0f);
			case Orientation.left:
				return Quaternion.Euler(0f, 90f, 0f);
			case Orientation.right:
				return Quaternion.Euler(0f, -90f, 0f);
			case Orientation.towards:
				return Quaternion.Euler(0f, 0f, 0f);
			case Orientation.away:
				return Quaternion.Euler(180f, 0f, 0f);
			default:
				return Quaternion.Euler(0f, 0f, 0f);
		}
	}

	public void SpawnHex(Vector3 position, Orientation dir = Orientation.up, int quantity = 1)
	{
		hexagons.Clear();

		for (int i = 0; i < quantity; i++)
		{
			hexagons.Add(Instantiate(template, transform));
			hexagons[i].transform.position = position;
			hexagons[i].transform.rotation = GetOrientation(dir);
		}
	}
}
