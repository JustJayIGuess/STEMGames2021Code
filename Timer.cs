using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
	private TextMeshPro text;
	private static float levelBeginTime;

	void UpdateHighScore(int level)
	{
		GameData oldData = StoreData.Load<GameData>(StoreData.gameDataSavePath, true);

		float highscore = 0f;

		if (oldData != null)
		{
			try
			{
				highscore = oldData.highscores[level];
			}
			catch (Exception e)
			{
				Debug.LogWarning($"highscore of level {level} requested, but is not defined in file at {StoreData.gameDataSavePath}!");
				Debug.LogException(e);
			}

			if (GetTime() < highscore || highscore == 0f)
			{
				float[] newScores = new float[GameController.levelCount];
				for (int i = 0; i < oldData.highscores.Length; i++)
				{
					newScores[i] = oldData.highscores[i];
				}
				newScores[level] = GetTime();

				StoreData.SaveGameData(newScores);
			}
		}
		else
		{
			float[] newScores = new float[GameController.levelCount];
			for (int i = 0; i < newScores.Length; i++)
			{
				newScores[i] = Mathf.Infinity;
			}
			newScores[level] = GetTime();

			StoreData.SaveGameData(newScores);
		}
	}

	public static float GetTime()
	{
		return Time.time - levelBeginTime;
	}

	public static void SetTime(float newTime)
	{
		levelBeginTime = Time.time - newTime;
	}

	private void OnEnable()
	{
		LevelEnd.OnLevelEnd += UpdateHighScore;
	}

	private void OnDisable()
	{
		LevelEnd.OnLevelEnd -= UpdateHighScore;
	}

	private void Awake()
	{
		levelBeginTime = Time.time;
		text = GetComponent<TextMeshPro>();
	}

	private void Update()
	{
		text.SetText("Time:\n{0:1}", GetTime());
	}
}
