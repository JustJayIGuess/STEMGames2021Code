using System;
using TMPro;
using UnityEngine;

public class HighscoreDisplay : MonoBehaviour
{
	private TextMeshPro[] text;

	private void Start()
	{
		GameData data = StoreData.Load<GameData>(StoreData.gameDataSavePath, false);
		float[] highscores;

		if (data == null)
		{
			highscores = new float[GameController.levelCount];
			for (int i = 0; i < highscores.Length; i++)
			{
				highscores[i] = 0f;
			}
		}
		else
		{
			highscores = data.highscores;
		}

		text = GetComponentsInChildren<TextMeshPro>();

		foreach (TextMeshPro textMeshPro in text)
		{
			textMeshPro.text = "";
		}

		// Cool formatted string that looks complicated but is actually not
		for (int i = 0; i < GameController.levelCount; i++)
		{
			text[i % text.Length].text = $"{text[i % text.Length].text}Level {i + 1}: {highscores[i]:#.00}s\n\n";
		}
	}
}
