using System.Collections;
using UnityEngine;

public class WallInd : MonoBehaviour
{
	public LifeIndicator lifeInd;
	public PlayerMovement player;
	private ColorScheme colorScheme;

	private float fader;

	private IEnumerator FadeColor(float speed, Color col, float intensity)
	{
		fader = 0f;
		while (fader < 1f)
		{
			fader += Time.deltaTime * speed;

			lifeInd.SetColor(col, intensity, true, fader);
			yield return null;
		}
		yield break;
	}

	private void Awake()
	{
		colorScheme = FindObjectOfType<ColorScheme>();
	}

	public void UpdateState()
	{
		if (player.CanWallJump())
		{
			StopAllCoroutines();
			StartCoroutine(FadeColor(3f, colorScheme.Colors[ColorScheme.SchemeColor.LBlue], 1.4f));
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(FadeColor(3f, lifeInd.mainColor, 1f));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		UpdateState();
	}

	private void OnTriggerExit(Collider other)
	{
		UpdateState();
	}
}
