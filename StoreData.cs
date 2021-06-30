using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
// not going to lie I was expecting save mechanics to be a lot easier than they ended up being.

public static class StoreData
{
	public static readonly string playerDataSavePath = $"{Application.persistentDataPath}/{Application.productName}SaveData.hks";	// jumped at the opportunity to make our own file extension
	public static readonly string gameDataSavePath = $"{Application.persistentDataPath}/{Application.productName}GameData.hks";		// hks stands for Hydraulic Kangaroo Studios

	public static void SavePlayerData(PlayerMovement player, BodyManager body, Elevator elevator = null, ElevatorDoor elevatorDoor = null)
	{
		BinaryFormatter formatter = new BinaryFormatter();

		SaveData data = new SaveData(player, body, elevator, elevatorDoor);

		FileStream file = new FileStream(playerDataSavePath, FileMode.Create);

		formatter.Serialize(file, data);

		file.Close();
	}

	public static void SaveGameData(float[] highscores)
	{
		Debug.Log("From SaveGameData():");

		BinaryFormatter formatter = new BinaryFormatter();

		GameData data = new GameData(highscores, GameController.uname);

		FileStream file = new FileStream(gameDataSavePath, FileMode.Create);

		formatter.Serialize(file, data);

		file.Close();
	}

	public static T Load<T>(string path, bool supressWarnings = false) where T : class
	{
		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = new FileStream(path, FileMode.Open);

			T data = (T)formatter.Deserialize(file);

			file.Close();

			return data;
		}
		else if (!supressWarnings)
		{
			Debug.LogError("Attempted to load data from nonexistant save file! Returning null.");
		}
		return null;
	}
}
