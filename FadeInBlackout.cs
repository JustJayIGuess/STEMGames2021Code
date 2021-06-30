using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInBlackout : MonoBehaviour
{
	// Sadly can't use MaterialPropertyBlock here, because UI CanvasRenderer doesn't inherit from Renderer class (which is kind of annoying)
	private Material blackoutMaterial;
	private void Awake()
	{
		blackoutMaterial = GetComponent<Image>().material;
	}

	private void Start()
	{
		blackoutMaterial.color = Color.black;
		StartCoroutine(LerpBlackoutMaterialAlpha(0f, 0.85f, true));
	}

	private void OnEnable()
	{
		LevelEnd.OnLevelEnd += OnLevelEnd;
	}

	private void OnDisable()
	{
		LevelEnd.OnLevelEnd -= OnLevelEnd;
	}

	private void OnLevelEnd(int level)
	{
		StartCoroutine(LerpBlackoutMaterialAlpha(1f, 0.85f, false));
	}

	private IEnumerator LerpBlackoutMaterialAlpha(float targetAlpha, float speed, bool setPlayerEnabledAfterFade, float snap = 0.05f)
	{
		Color blackoutColor = blackoutMaterial.color;
		float blackoutStartAlpha = blackoutColor.a;

		float progress = 0f;

		while (Mathf.Abs(blackoutColor.a - targetAlpha) > snap)
		{
			blackoutColor.a = Mathf.Lerp(blackoutStartAlpha, targetAlpha, progress);
			blackoutMaterial.color = blackoutColor;

			progress += Time.fixedDeltaTime * speed;

			yield return new WaitForFixedUpdate();
		}

		blackoutColor.a = targetAlpha;
		blackoutMaterial.color = blackoutColor;

		FindObjectOfType<PlayerMovement>().gameObject.SetActive(setPlayerEnabledAfterFade);

		yield return null;
	}
}
