using UnityEngine;

public class LifeIndicator : MonoBehaviour
{
	[Space(15)]
	[Header("Objects")]
	public Material fairyLight;
	public ScalePlayer scaler;
	public LifeIndFlash flasher;
	public Light lifeLight;
	public Color mainColor;

	private Color fairyColor;
	private float origScale = 1f;

	private void OnEnable()
	{
		ScalePlayer.OnScale += UpdateLifeInd;
	}

	private void OnDisable()
	{
		ScalePlayer.OnScale -= UpdateLifeInd;
	}


	public void UnAbleFlash()
	{
		AudioController.Play("Unable-Flash-Sound");
		flasher.BeginImportantHeartBeat(7f, 7f, Color.red);
	}

	private void UpdateLifeInd(float scale)
	{
		transform.localScale = Vector3.one * origScale * scale;
	}

	private void UpdateEmission(float intensity = 1f, bool doLight = false, float t = 1f)
	{
		flasher.SetLightBrightness(intensity, fairyColor, t);
		flasher.UpdateMatCache(flasher.lifeLightMat);

		if (doLight)
		{
			lifeLight.color = fairyColor;
			lifeLight.intensity = intensity;
		}
	}

	public void Beat(float speed, Color col)
	{
		flasher.BeginHeartBeat(speed, speed, col);
	}

	public void SetColor(Color col, float intensity = 1f, bool doLight = false, float t = 1f)
	{
		fairyColor = col;
		UpdateEmission(intensity, doLight, t);
	}

	public void ResetColor()
	{
		fairyColor = mainColor;
		UpdateEmission(1f, true);
	}

	private void Start()
	{
		flasher.UpdateMatCache(flasher.lifeLightMat);
		origScale = transform.localScale.x;
		fairyColor = mainColor;
		UpdateEmission(1f, true);
	}
}
