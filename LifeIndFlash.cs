using System.Collections;
using UnityEngine;

public class LifeIndFlash : MonoBehaviour
{
	public Material lifeLightMat;

	private Coroutine heartBeatCR;

	private Material matCache;

	private readonly string em = "_EmissionColor";

	private bool isRunning = false;

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

	public void SetLightBrightness(float intensity, Color col, float t = 1f)
	{
		Color c = GetCacheEmission();
		Vector4 colVect = new Vector4(col.r, col.g, col.b, col.a) * intensity;
		lifeLightMat.SetColor(em, Vector4.Lerp(c, colVect, t));
		lifeLightMat.SetColor("_Color", Vector4.Lerp(c, colVect, t));
	}

	private IEnumerator Flash(float speed1, float speed2, Color col, int iters = 1, float swing = 0f, float val1 = 0f, float val2 = 1f)
	{
		while (iters > 0)
		{
			float t = 0f;
			while (t < 1f)
			{
				SetLightBrightness(Mathf.Lerp(val1, val2, t), col, t);
				t += Time.deltaTime * speed1;
				yield return null;
			}
			while (t > 0f)
			{
				SetLightBrightness(Mathf.Lerp(val1, val2, t), col, t);
				t -= Time.deltaTime * speed2;
				yield return null;
			}

			iters--;
			speed1 += swing;
			speed2 += swing;
		}
		heartBeatCR = null;
		isRunning = false;
	}
	

	public void BeginHeartBeat(float speed1, float speed2, Color col, int iters = 1, float swing = 0f, float val1 = 0f, float val2 = 1f)
	{
		if (!isRunning)
		{
			if (heartBeatCR != null)
			{
				StopCoroutine(heartBeatCR);
			}

			heartBeatCR = null;
			heartBeatCR = StartCoroutine(Flash(speed1, speed2, col, iters, swing, val1, val2));
		}
	}

	public void BeginImportantHeartBeat(float speed1, float speed2, Color col, int iters = 1, float swing = 0f, float val1 = 0f, float val2 = 1f)
	{
		if (!isRunning)
		{
			if (heartBeatCR != null)
			{
				StopCoroutine(heartBeatCR);
			}

			heartBeatCR = null;
			isRunning = true;
			heartBeatCR = StartCoroutine(Flash(speed1, speed2, col, iters, swing, val1, val2));
		}
	}
}
