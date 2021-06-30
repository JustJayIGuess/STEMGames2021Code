using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StalManager : MonoBehaviour
{
	Light[] lights;
	public bool randomizeLights;
	public float lightRange;

	public float lightIntensity;

	public LightmapBakeType bakeType;
	public float shadowRadius;

	public bool forceAllVertex = false;

	private string[] colors = new string[] { "StalRed", "StalWhite", "StalGreen" };


#if UNITY_EDITOR
	private void OnValidate()
    {
		lights = GetComponentsInChildren<Light>();
		List<MeshRenderer> stalRenderers = new List<MeshRenderer>();

		foreach (Light light in lights)
		{
			stalRenderers.AddRange(light.transform.parent.GetComponentsInChildren<MeshRenderer>());
			foreach (MeshRenderer meshRenderer in stalRenderers)
			{
				meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				meshRenderer.staticShadowCaster = false;
			}
			stalRenderers.Clear();
			if (randomizeLights)
			{
				light.transform.parent.tag = colors[UnityEngine.Random.Range(0, 3)];
			}
			try
			{
				light.intensity = lightIntensity * (light.transform.parent.CompareTag(colors[0]) ? 1.5f : 1f);
				light.shadows = LightShadows.Soft;
				light.range = lightRange;
				light.shadowRadius = shadowRadius;
				light.lightmapBakeType = bakeType;

				if (forceAllVertex)
				{
					light.renderMode = LightRenderMode.ForceVertex;
				}
			}
			catch (Exception e)
			{
				Logger.WriteDebug(e.ToString());
				Debug.LogWarning(e.ToString());
			}
		}
	}
#endif
}
