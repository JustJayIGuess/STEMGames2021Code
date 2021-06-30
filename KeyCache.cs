using System;
using System.Collections.Generic;
using UnityEngine;

/*
  This caches all keypresses into a dictionary with added functionality to make 
  the key still return true if it was pressed a bit before to make the game
  more forgiving when you press a key a little too late. Also allows control
  locking.
*/

public class KeyCache : MonoSingleton<KeyCache>
{
	// Make dictionary - [KeyCode.Space: Vector3(3.5s, 5.1s)] where x = when the key was pressed down and y = when the key was released
	private Dictionary<KeyCode, KeyCacheInfo> keyCache = new Dictionary<KeyCode, KeyCacheInfo>();
	private LifeIndicator lifeInd;

	public delegate void KeyDownAction(KeyCode key);
	public static event KeyDownAction OnKeyDown;

	private bool takeInput = true;

	public void ResetAll()
	{
		foreach(KeyValuePair<KeyCode, KeyCacheInfo> pair in keyCache)
		{
			pair.Value.Clear();
		}
	}

	public void LockControls()
	{
		ResetAll();
		takeInput = false;
		if (lifeInd != null)
		{
			lifeInd.SetColor(Color.grey);
		}
	}

	public void LockControl(KeyCode key)
	{
		if (!keyCache.ContainsKey(key))
		{
			keyCache[key] = new KeyCacheInfo();
		}

		keyCache[key].Enabled = false;
	}

	public void UnlockControl(KeyCode key)
	{
		if (!keyCache.ContainsKey(key))
		{
			keyCache[key] = new KeyCacheInfo();
			return;
		}

		keyCache[key].Enabled = true;
	}

	public void LockControls(List<KeyCode> keys)
	{
		foreach (KeyCode key in keys)
		{
			if (!keyCache.ContainsKey(key))
			{
				keyCache[key] = new KeyCacheInfo();
			}

			keyCache[key].Enabled = false;
		}
		
	}

	public void UnlockControls(List<KeyCode> keys)
	{
		foreach (KeyCode key in keys)
		{
			if (!keyCache.ContainsKey(key))
			{
				keyCache[key] = new KeyCacheInfo();
			}

			keyCache[key].Enabled = true;
		}
	}

	public void UnlockControls()
	{
		takeInput = true;

		foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKey(key))
			{
				if (!keyCache.ContainsKey(key))
				{
					keyCache[key] = new KeyCacheInfo();
				}

				keyCache[key].SetKeyDown(Time.unscaledTime);
				OnKeyDown?.Invoke(key);
			}
		}
	}

	// gets info from cache
	private KeyCacheInfo GetCacheInfo(KeyCode key)
	{
		if (!keyCache.ContainsKey(key))
		{
			keyCache[key] = new KeyCacheInfo();
		}

		return keyCache[key];
	}

	// Checks if key was pressed down less than  or equal to t seconds ago
	public bool WasKeyDown(KeyCode key, float t = 0f)
	{
		KeyCacheInfo keyInfo = GetCacheInfo(key);
		if (!keyInfo.Enabled)
		{
			return false;
		}

		if (keyInfo.GetKeyDown() > Time.unscaledTime - t)
		{
			return true;
		}

		if (keyInfo.GetKeyUp() < 0f)
		{
			return false;
		}

		return false;
	}

	// Checks if key is pressed or was pressed less than or equal to t seconds ago
	public bool WasKeyPressed(KeyCode key, float t = 0f)
	{

		KeyCacheInfo keyInfo = GetCacheInfo(key);
		if (!keyInfo.Enabled)
		{
			return false;
		}

		if (keyInfo.GetKeyUp() > keyInfo.GetKeyDown())
		{
			if (Time.unscaledTime - t > keyInfo.GetKeyUp())
			{
				return false;
			}
		}

		if (keyInfo.GetKeyDown() < 0f)
		{
			return false;
		}

		return true;
	}

	// Overload for multiple keys
	private KeyCacheInfo[] GetCacheInfo(KeyCode[] keys)
	{
		KeyCacheInfo[] res = new KeyCacheInfo[keys.Length];

		// If dictionary doesnt contain entry, create one
		int i = 0;
		foreach (KeyCode key in keys)
		{
			if (!keyCache.ContainsKey(key))
			{
				keyCache[key] = new KeyCacheInfo();
			}
			res[i] = keyCache[key];
			i++;
		}


		return res;
	}

	// Overload for multiple keys
	public bool WasKeyDown(KeyCode[] keys, float t = 0f)
	{
		foreach (KeyCode key in keys)
		{
			if (WasKeyDown(key, t))
			{
				return true;
			}
		}

		return false;
	}

	// Overload for multiple keys
	public bool WasKeyPressed(KeyCode[] keys, float t = 0f)
	{
		foreach (KeyCode key in keys)
		{
			if (WasKeyPressed(key, t))
			{
				return true;
			}
		}

		return false;
	}

	// Overload for multiple keys
	public bool WasKeyDown(List<KeyCode> keys, float t = 0f)
	{
		foreach (KeyCode key in keys)
		{
			if (WasKeyDown(key, t))
			{
				return true;
			}
		}

		return false;
	}

	// Overload for multiple keys
	public bool WasKeyPressed(List<KeyCode> keys, float t = 0f)
	{
		foreach (KeyCode key in keys)
		{
			if (WasKeyPressed(key, t))
			{
				return true;
			}
		}

		return false;
	}

	private void Awake()
	{
		if (FindObjectOfType<LifeIndicator>() != null)
		{
			lifeInd = FindObjectOfType<LifeIndicator>();
		}
	}

	// Updates keyCache when key pressed
	private void OnGUI()
	{
		if (takeInput)
		{
			Event e = Event.current;
			if (e.isKey)
			{
				if (Input.GetKeyDown(e.keyCode))
				{
					if (!keyCache.ContainsKey(e.keyCode))
					{
						keyCache[e.keyCode] = new KeyCacheInfo();
					}

					keyCache[e.keyCode].SetKeyDown(Time.unscaledTime);
					OnKeyDown?.Invoke(e.keyCode);
				}
				else if (Input.GetKeyUp(e.keyCode))
				{
					if (!keyCache.ContainsKey(e.keyCode))
					{
						keyCache[e.keyCode] = new KeyCacheInfo();
					}

					keyCache[e.keyCode].SetKeyUp(Time.unscaledTime);
				}
			}
		}
	}
}
