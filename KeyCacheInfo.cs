using UnityEngine;

public class KeyCacheInfo
{
	private float keyUp;
	private float keyDown;
	private bool enabled;

	public bool Enabled
	{
		get => enabled;
		set
		{
			Clear();
			enabled = value;
		}
	}

	public KeyCacheInfo()
	{
		keyDown = Mathf.NegativeInfinity;
		keyUp = Mathf.NegativeInfinity;
		Enabled = true;
	}

	public void SetKeyDown(float t)
	{
		keyDown = t;
	}

	public void SetKeyUp(float t)
	{
		keyUp = t;
	}

	public float GetKeyDown()
	{
		return keyDown;
	}

	public float GetKeyUp()
	{
		return keyUp;
	}

	public void Clear()
	{
		keyDown = Mathf.NegativeInfinity;
		keyUp = Mathf.NegativeInfinity;
	}
}
