using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CCControl : MonoBehaviour
{
	public VolumeProfile vol;
	private ColorAdjustments filter;
	private ChromaticAberration chromAb;
	private DepthOfField dof;

	private void Awake()
	{
		vol.TryGet(out filter);
		vol.TryGet(out chromAb);
		vol.TryGet(out dof);
		filter.colorFilter.value = Color.white;
		filter.contrast.value = 26f;
		filter.saturation.value = 32f;
		chromAb.intensity.value = 0.068f;
		dof.focusDistance.value = 1f;
	}

	public void SetFocalDistance(float newDistance)
	{
		dof.focusDistance.value = newDistance;
	}

	private IEnumerator FadeColorTo(Color target, float speed, float snapPoint)
	{
		Color col = filter.colorFilter.value;

		while (Utils.AverageColorDifference(col, target) > snapPoint) {
			col = Color.Lerp(col, target, Time.unscaledDeltaTime * speed);

			filter.colorFilter.value = col;
			yield return null;
		}

		filter.colorFilter.value = target;
	}

	private IEnumerator FadeContrastTo(float target, float speed, float snapPoint)
	{
		float contrast = filter.contrast.value;

		while (Mathf.Abs(contrast - target) > snapPoint)
		{
			contrast = Mathf.Lerp(contrast, target, Time.unscaledDeltaTime * speed);

			filter.contrast.value = contrast;
			yield return null;
		}

		filter.contrast.value = target;
	}

	private IEnumerator FadeSaturationTo(float target, float speed, float snapPoint)
	{
		float intensity = filter.saturation.value;

		while (Mathf.Abs(intensity - target) > snapPoint)
		{
			intensity = Mathf.Lerp(intensity, target, Time.unscaledDeltaTime * speed);
			filter.saturation.value = intensity;
			yield return null;
		}

		filter.saturation.value = target;
	}

	private IEnumerator FadeChromAbTo(float target, float speed, float snapPoint)
	{
		float intensity = chromAb.intensity.value;

		while (Mathf.Abs(intensity - target) > snapPoint)
		{
			intensity = Mathf.Lerp(intensity, target, Time.unscaledDeltaTime * speed);
			chromAb.intensity.value = intensity;
			yield return null;
		}

		chromAb.intensity.value = target;
	}

	private IEnumerator FadeFocalDistanceTo(float target, float speed, float snapPoint)
	{
		float focalDistance = dof.focusDistance.value;

		while (Mathf.Abs(focalDistance - target) > snapPoint)
		{
			focalDistance = Mathf.Lerp(focalDistance, target, Time.unscaledDeltaTime * speed);
			dof.focusDistance.value = focalDistance;
			yield return null;
		}

		dof.focusDistance.value = target;
	}

	private IEnumerator FadeFocalLengthTo(float target, float speed, float snapPoint)
	{
		float length = dof.focalLength.value;

		while (Mathf.Abs(length - target) > snapPoint)
		{
			length = Mathf.Lerp(length, target, Time.unscaledDeltaTime * speed);
			dof.focalLength.value = length;
			yield return null;
		}

		dof.focalLength.value = target;
	}

	public void FadeTo(Color col, float contrast, float intensity, float saturation, float aperture, float[] speeds, float snapPoint = 0.01f)
	{
		StopAllCoroutines();
		StartCoroutine(FadeColorTo		(col,			speeds[0],	snapPoint));
		StartCoroutine(FadeContrastTo	(contrast,		speeds[1],	snapPoint));
		StartCoroutine(FadeChromAbTo	(intensity,		speeds[2],	snapPoint));
		StartCoroutine(FadeSaturationTo	(saturation,	speeds[3],	snapPoint));
		StartCoroutine(FadeFocalLengthTo(aperture,		speeds[4],	snapPoint));
	}
}
