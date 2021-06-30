using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
	Coroutine cameraShake;

	private IEnumerator ShakeCamera(float intensity, float dur, float fadeSpeed)
	{
		float passed = 0f;
		Vector3 startPos = transform.localPosition;

		while (passed < dur)
		{
			float x = Random.Range(-1f, 1f) * intensity;
			float y = Random.Range(-1f, 1f) * intensity;

			transform.localPosition = new Vector3(x, y, startPos.z);

			intensity = Mathf.Lerp(intensity, 0f, Time.deltaTime * fadeSpeed);
			passed += Time.deltaTime;

			yield return null;
		}

		transform.localPosition = startPos;
	}

	public void BeginCameraShake(float intensity, float dur, float fadeSpeed)
	{
		if (cameraShake != null)
			StopCoroutine(cameraShake);
		cameraShake = StartCoroutine(ShakeCamera(intensity, dur, fadeSpeed));
	}
}
