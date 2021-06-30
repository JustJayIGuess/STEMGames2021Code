using System.Collections;
using UnityEngine;

// Singleton Class that will run coroutines for foreign static functions (ngl I'm kinda proud of this)
public class StaticCoroutine : MonoBehaviour
{
	private static StaticCoroutine _instance;

	// Cleanup scripts
	private void OnDestroy()
	{
		StopAllCoroutines();
	}

	private void OnApplicationQuit()
	{
		StopAllCoroutines();
	}

	public static StaticCoroutine GetInstance()
	{
		//If there is an instance, return it
		if (_instance != null)
		{
			return _instance;
		}

		//If there is no instance, look for one
		_instance = (StaticCoroutine)FindObjectOfType(typeof(StaticCoroutine));

		if (_instance != null)
		{
			return _instance;
		}

		// If there no instances in scene, create one on empty DDOL gameobject
		GameObject staticCoroutineObject = new GameObject("StaticCoroutineSingleton");
		staticCoroutineObject.AddComponent<DDOL>();
		return staticCoroutineObject.AddComponent<StaticCoroutine>();
	}

	public static void Begin(string coroutine)
	{
		GetInstance().StartCoroutine(coroutine);
	}

	public static void Begin(IEnumerator coroutine)
	{
		GetInstance().StartCoroutine(coroutine);
	}

	public static void Begin(string coroutine, object parameters)
	{
		GetInstance().StartCoroutine(coroutine, parameters);
	}

	public static void StopAll()
	{
		GetInstance().StopAllCoroutines();
	}
}
