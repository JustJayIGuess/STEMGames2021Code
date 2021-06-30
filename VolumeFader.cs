using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeFader : MonoBehaviour
{
	public AudioSource song;

	[Range(0f, 1f)]
	public float fadeToVolume = 1f;
	[Range(0f, 1f)]
	public float speed = 1f;

	private float currVol;

	private void Start()
	{
		if (song == null)
		{
			Destroy(this);
			Debug.LogWarning("Destroyed Self: No song provided");
			return;
		}
		currVol = song.volume;
	}

	void Update()
    {
		currVol = Mathf.Lerp(currVol, fadeToVolume, Time.deltaTime * speed);
		song.volume = currVol;
    }
}
