using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make it show in inspectors and stuff
[Serializable]
public class Sound
{
	public string ID = "No Name";

	public AudioClip clip;

	[Header("Basic Info")]
	[Range(0f, 2f)]
	public float volume = 1f;
	public bool isSong = false;
	public float startFrom = 0f;
	public bool loop = false;
	public bool playOnStart = false;
	[Header("Fade")]
	public bool fadeIn = false;
	[Range(0f, 2f)]
	public float fadeToVolume = 1f;
	[Range(0f, 1f)]
	public float fadeSpeed = 1f;

	[HideInInspector]
	public AudioSource audioSource;

	// Overloaded in case i have to copy sound settings over to eachother
	public void UpdateAudioSource(Sound sound, GameObject gameObj)
	{
		audioSource = gameObj.AddComponent<AudioSource>();

		audioSource.clip = sound.clip;
		audioSource.loop = sound.loop;
		audioSource.volume = sound.volume;
		audioSource.time = sound.startFrom;
	}

	public void UpdateAudioSource(GameObject gameObj)
	{
		audioSource = gameObj.AddComponent<AudioSource>();

		audioSource.clip = clip;
		audioSource.loop = loop;
		audioSource.volume = volume;
		audioSource.time = startFrom;
	}

	public void Mute(bool doMute = true)
	{
		audioSource.mute = doMute;
	}

	public void Play(float delay = 0f)
	{
		audioSource.PlayDelayed(delay);
	}

	public void Stop()
	{
		audioSource.Stop();
	}

	public IEnumerator PlayRandomContinuous(float delay = 0f, List<Sound> selection = null)
	{
		int? lastSongIndex = null;
		yield return new WaitForSeconds(delay);
		while (true)
		{
			int songIndex = UnityEngine.Random.Range(0, selection.Count);

			// Stop same song from being played twice (branchless); adds one to index if it was the same as the last one
			songIndex += Utils.BoolToInt(songIndex == lastSongIndex);
			lastSongIndex = songIndex;

			// Modulo is to prevent index out of bound when index was incremented prior
			Sound selectedSound = selection?[songIndex % selection.Count];

			selectedSound.Play(delay);

			yield return new WaitForSeconds(selectedSound.clip.length);
		}
	}
}
