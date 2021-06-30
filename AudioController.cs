using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoSingleton<AudioController>
{
	public bool fadeIn = true;
	public float interTrackSpacing = 1f; // Time between music tracks of silence
	public Sound[] sounds;

	private void Awake()
	{
		List<Sound> musicSelection = new List<Sound>();
		// Initialise the sounds
		foreach (Sound sound in sounds)
		{
			sound.UpdateAudioSource(gameObject);
			if (sound.playOnStart)
			{
				if (sound.isSong)
				{
					musicSelection.Add(sound);
					sound.loop = false;	// Make sure music doesn't loop (mostly to protect against me accidentally setting music to loop)
				}
				else
				{
					sound.Play();
				}
			}
			if (sound.fadeIn)
			{
				VolumeFader fader = gameObject.AddComponent<VolumeFader>();
				fader.song = sound.audioSource;
				fader.fadeToVolume = sound.fadeToVolume;
				fader.speed = sound.fadeSpeed;
			}
		}
		StartCoroutine(musicSelection?[0].PlayRandomContinuous(interTrackSpacing, musicSelection));
	}
	
	// A singleton-like function to get the audiocontroller from the __app DDOL group
	public static AudioController GetAudioController()
	{
		return FindObjectOfType<AudioController>();
	}

	public static void Play(string ID)
	{
		// Get instance of audioController singleton-style

		Sound sound = Array.Find(Instance.sounds, x => x.ID.ToLower() == ID.ToLower());

		// Play sound with ID, if no sound has ID, warn editor
		if (sound == null)
		{
			Debug.LogWarning("No sound of ID: " + ID + " found!");
		} else
		{
			sound.Play();
		}
	}

	public static void Mute(string ID)
	{
		// Get instance of audioController singleton-style

		Sound sound = Array.Find(Instance.sounds, x => x.ID.ToLower() == ID.ToLower());

		// Mute sound with ID, if no sound has ID, warn editor
		if (sound == null)
		{
			Debug.LogWarning("No sound of ID: " + ID + " found!");
		}
		else
		{
			sound.Mute();
		}
	}

	public static void Stop(string ID)
	{
		// Get instance of audioController singleton-style

		Sound sound = Array.Find(Instance.sounds, x => x.ID.ToLower() == ID.ToLower());

		// Stop sound with ID, if no sound has ID, warn editor
		if (sound == null)
		{
			Debug.LogWarning("No sound of ID: " + ID + " found!");
		}
		else
		{
			sound.Stop();
		}
	}

}
