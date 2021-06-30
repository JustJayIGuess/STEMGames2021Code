using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnHeartbeat : MonoBehaviour
{
	public Material heartLight;

	private Coroutine heartBeatCR;

	private Material matCache;

	private readonly string em = "_EmissionColor";

	public void UpdateMatCache(Material mat)
	{
		matCache = new Material(mat);
	}

	public Color GetCacheEmission()
	{
		if (matCache != null)
		{
			return matCache.GetColor(em);
		}
		else
		{
			return Color.black;
		}
	}

	private void SetLightBrightness(float intensity)
	{
		Color c = GetCacheEmission();
		Vector4 colVect = new Vector4(c.r, c.g, c.b, c.a) * intensity;
		heartLight.SetColor(em, colVect);
	}

	private IEnumerator Flash(float speedUp, float speedDown, float minVal, float maxVal)
	{
		for (float a = minVal; a > maxVal; a -= Time.deltaTime * speedUp)
		{
			SetLightBrightness(a);
			yield return null;
		}
		if (!heartLight.GetColor(em).a.Equals(maxVal))
		{
			SetLightBrightness(maxVal);
		}


		for (float a = maxVal; a < minVal; a += Time.deltaTime * speedDown)
		{
			SetLightBrightness(a);
			yield return null;
		}
		if (!heartLight.GetColor(em).a.Equals(minVal))
		{
			SetLightBrightness(minVal);
		}
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
		UpdateMatCache(heartLight);
	}

	//private void Start()
	//{
	//	heartLight.SetColor("_EmissionColor", Color.blue);
	//}
}
