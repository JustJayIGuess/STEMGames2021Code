using UnityEngine;

// A singleton class to inherit from to make classes a singleton
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	// Check to see if we're about to be destroyed.
	private static bool shutdown = false;
	private static readonly object _lock = new object();
	private static volatile T instance;
	
	public static T Instance
	{
		get
		{
			if (shutdown)
			{
				Debug.LogWarning("Instance of '" + typeof(T) +
					"' requested, but its already destroyed! Returning null.");
				return null;
			}

			lock (_lock)
			{
				if (instance == null)
				{
					instance = (T)FindObjectOfType(typeof(T));

					if (instance == null)
					{
						Debug.LogError("MonoSingleton instance of '" + typeof(T) + "' requested, but there is none! Returning null.");
					}
				}

				return instance;
			}
		}
	}

	private void Awake()
	{
		shutdown = false;
	}


	private void OnApplicationQuit()
	{
		shutdown = true;
	}


	private void OnDestroy()
	{
		shutdown = true;
	}
}