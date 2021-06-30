using System;
using TMPro;
using UnityEngine;

[Serializable]
public class TextQueueElement
{
	public string name;

	[Header("Timing")]
	public float preTextTime;
	public float duration;

	[Header("Text Information")]
	public TextProperties properties;

	[Header("Fade Information")]
	[Range(0f, 10f)]
	public float fadeSpeed;
	public bool doFade;
	public bool fadeOutWhenDone;

	[Header("Extra Information")]
	public string skipIfNotDone = "";
	public string skipIfDone = "";
	public bool skip;

	public TextQueueElement()
	{
		name = "No Name";
		preTextTime = 0f;
		duration = 0f;
		doFade = true;
		properties = new TextProperties("", FontStyles.Normal, 400f, Color.white);
		fadeSpeed = 4f;
		fadeOutWhenDone = false;
		skip = false;
	}
}
