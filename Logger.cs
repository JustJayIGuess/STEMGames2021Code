using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Logger : MonoBehaviour
{
	private static string filePath = "";

	void Start()
    {
		filePath = Application.persistentDataPath + "/DebugLog.txt";

		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
    }

    public static void WriteDebug(string str)
	{
		StreamWriter streamWriter = new StreamWriter(filePath, true);
		streamWriter.WriteLine(str);
		streamWriter.Close();
	}
}
