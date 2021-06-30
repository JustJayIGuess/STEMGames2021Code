using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heartbeat : MonoBehaviour
{
	public Light heartLight;

	private Coroutine heartBeatCR;

	private IEnumerator Flash(float speedUp, float speedDown, float minVal, float maxVal)
	{
		for (float a = minVal; a < maxVal; a += Time.deltaTime * speedUp)
		{
			heartLight.intensity = a;
			yield return null;
		}
		if (!heartLight.intensity.Equals(maxVal)) heartLight.intensity = maxVal;

		for (float a = maxVal; a > minVal; a -= Time.deltaTime * speedDown)
		{
			heartLight.intensity = a;
			yield return null;
		}
		if (!heartLight.intensity.Equals(minVal)) heartLight.intensity = minVal;
	}

	public void BeginHeartBeat(float speedUp, float speedDown, float minVal, float maxVal)
	{
		if (heartBeatCR != null)
		{
			StopCoroutine(heartBeatCR);
		}
		heartBeatCR = StartCoroutine(Flash(speedUp, speedDown, minVal, maxVal));
	}

	private void Start()
	{
		heartLight.intensity = 0f;
	}
}
