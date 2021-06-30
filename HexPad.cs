using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexPad : MonoBehaviour
{
	float alpha = 1f;
	public float fadeSpeed = 1f;
	public float snapPoint = 0.01f;

	SpriteRenderer rend;

	private void Awake()
	{
		rend = GetComponent<SpriteRenderer>();
		rend.material.color = FindObjectOfType<ColorScheme>().Colors[ColorScheme.SchemeColor.LBlue];
	}

    void Update()
    {
		rend.material.color = Utils.WithAlpha(rend.material.color, alpha);

		alpha = Mathf.Lerp(alpha, 0f, Time.deltaTime * fadeSpeed);

		if (alpha < snapPoint)
			Destroy(gameObject);
	}
}
