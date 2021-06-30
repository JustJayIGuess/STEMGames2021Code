using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Subtitles : MonoBehaviour
{
	public enum FadeDirection
	{
		Out,
		In
	}

	[SerializeField]
	private TextMeshPro text;

	public List<TextQueueElement> textQueue = new List<TextQueueElement>();
	private List<TextQueueElement> done = new List<TextQueueElement>();

	private bool isFading = false;
	private Coroutine fadeCoroutine;
	private bool doSkip = false;

	public delegate void SubtitleDisplayAction(string name);
	public static event SubtitleDisplayAction OnSubtitleDisplay;

	// Note: The following functions use a bunch of delegates and callbacks and is probably coded pretty messily

	private IEnumerator FadeText(FadeDirection dir, float speed, float snap, float waitFor, Action callBack = null)
	{
		yield return new WaitForSeconds(waitFor);
		while (Mathf.Abs(text.color.a - (float)dir) > snap)
		{
			text.color = Color.Lerp(text.color, Utils.WithAlpha(text.color, (float)dir), Time.deltaTime * speed);
			yield return null;
		}
		text.color = Utils.WithAlpha(text.color, (float)dir);
		callBack?.Invoke();
	}

	public void Fade(FadeDirection dir, float speed, float snap = 0.05f, float waitFor = 0f, Action callBack = null)
	{
		if (fadeCoroutine != null)
		{
			StopCoroutine(fadeCoroutine);
			fadeCoroutine = null;
		}

		if (dir == FadeDirection.In)
		{
			fadeCoroutine = StartCoroutine(FadeText(dir, speed, (snap * (float)dir) + 0.001f, waitFor, callBack));
		}
		else
		{
			fadeCoroutine = StartCoroutine(FadeText(dir, speed * 3f, (snap * (float)dir) + 0.001f, waitFor, callBack));
		}
	}

	// Recursive function to set text and possibly fade text away while setting then fade back
	public void SetText(TextProperties p, bool doFade = false, float speed = 1f, Action callBack = null, float preTextTime = 0f)
	{
		if (doFade)
		{
			isFading = true;
			Fade(FadeDirection.Out, speed, 0.05f, 0f,
				() => SetText(p, false, speed,
				() => Fade(FadeDirection.In, speed, 0.05f, preTextTime,
				() => isFading = false
			)));
		}
		else
		{
			text.color = Utils.WithRGB(text.color, p.textColor);
			text.fontSize = p.textSize;
			text.fontStyle = p.fontStyle;
			text.text = p.text;
			text.font = p.font;
		}
		callBack?.Invoke();
	}


	public void SkipToElement(string name, List<TextQueueElement> queue)
	{
		for (int i = 0; i < queue.Count; i++)
		{
			if (queue[i].name == name)
			{
				break;
			}
			queue[i].skip = true;
		}
	}

	public void SkipElement(string name, List<TextQueueElement> queue)
	{
		queue.Find(x => x.name == name).skip = true;
	}

	private IEnumerator ExecuteQueue(List<TextQueueElement> queue)
	{
		foreach (TextQueueElement element in queue)
		{
			float time;
			doSkip = false;

			// Check skipIfDone and skipIfNotDone
			if (done.Exists(x => x.name == element.skipIfDone) || (!done.Exists(x => x.name == element.skipIfNotDone) && element.skipIfNotDone != ""))
			{
				doSkip = true;
			}


			// Fade old text out, set new properties, wait for preTextTime, then fade new text in
			SetText(element.properties, element.doFade, element.fadeSpeed, null, element.preTextTime);

			// Wait until text has finished fading in, interrupt coroutine if skip has been set
			while (isFading && !doSkip)
			{
				if (element.skip)
				{
					doSkip = true;
					break;
				}
				yield return null;
			}
			if (doSkip)
			{
				continue;
			}

			// Add element to done list and invoke event
			done.Add(element);
			OnSubtitleDisplay?.Invoke(element.name);

			// Wait for text duration
			time = Time.time;
			while (time > Time.time - element.duration && !doSkip)
			{
				if (element.skip)
				{
					doSkip = true;
					break;
				}
				yield return null;
			}
			if (doSkip)
			{
				continue;
			}

			// If element is set to fade out when done, begin fading out
			if (element.fadeOutWhenDone)
			{
				Fade(FadeDirection.Out, element.fadeSpeed);
			}
		}
	}

	private IEnumerator CheckSkipAndWait(float dur, TextQueueElement element)
	{
		float startTime = 0;
		while (!doSkip)
		{
			if (element.skip)
			{
				doSkip = true;
				break;
			}

			if (Time.time - startTime > dur)
			{
				break;
			}

			yield return null;
		}
	}

	private void Awake()
	{
		text = GetComponent<TextMeshPro>();
	}

	private void Start()
	{
		text.text = "";
		StartCoroutine(ExecuteQueue(textQueue));
	}
}
