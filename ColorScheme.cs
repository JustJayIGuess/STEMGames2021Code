using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorScheme : MonoBehaviour
{

	public enum SchemeColor {Green, LBlue, DBlue, Purple, Red, White, Black, Grey};

	public Dictionary<SchemeColor, Color> Colors = new Dictionary<SchemeColor, Color>();

	[HideInInspector]
	public bool hasLoaded = false;

	private static readonly byte[,] colorHexes = new byte[,]
	{
		{0x3C, 0xFF, 0x59, 0xFF},
		{0x3C, 0xDF, 0xE8, 0xFF},
		{0x4F, 0x61, 0xF0, 0xFF},
		{0xC1, 0x3C, 0xE8, 0xFF},
		{0xFF, 0x56, 0x4D, 0xFF},
		{0xFF, 0xFF, 0xFF, 0xFF},
		{0x00, 0x00, 0x00, 0xFF},
		{0x88, 0x88, 0x88, 0xFF}
	};

	private static Color GetColorOfIndex(int i)
	{
		return new Color32(colorHexes[i, 0], colorHexes[i, 1], colorHexes[i, 2], colorHexes[i, 3]);
	}

	private void Awake()
	{
		hasLoaded = false;
		int count = 0;
		Colors.Clear();
		foreach (SchemeColor color in Enum.GetValues(typeof(SchemeColor)))
		{
			Colors.Add(color, GetColorOfIndex(count));
			count++;
		}
		hasLoaded = true;
	}

// For switching colours easily in unity editor, without compiling game
#if UNITY_EDITOR
	private void OnValidate()
	{
		hasLoaded = false;

		int count = 0;
		Colors.Clear();
		foreach (SchemeColor color in Enum.GetValues(typeof(SchemeColor)))
		{
			Colors.Add(color, GetColorOfIndex(count));
			count++;
		}
		hasLoaded = true;
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			int count = 0;
			Colors.Clear();
			foreach (SchemeColor color in Enum.GetValues(typeof(SchemeColor)))
			{
				Colors.Add(color, GetColorOfIndex(count));
				count++;
			}
		}
	}
#endif
}
