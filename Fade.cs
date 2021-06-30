using UnityEngine;
using TMPro;

public class Fade : MonoBehaviour
{

    public TextMeshPro interactText;

    public float speed = 5f;

    [Range(0f, 1f)]

    public const bool fadeOut = false;

    public const bool fadeIn = true;

	private float targetY;

	public bool GetFadeState()
    {
        return fadeState;
    }

    private Vector3 targetScaleVect = new Vector3(1f, 1f, 1f);
    private bool fadeState;

    public void FadeIn()
    {
        targetScaleVect = Vector3.one;
        fadeState = fadeIn;
    }

    public void FadeOut()
    {
        targetScaleVect = Vector3.zero;
        fadeState = fadeOut;
    }

	public void SetTargetElevation(float elevation)
	{
		targetY = elevation;
	}

	void Start()
    {
        transform.localScale = Vector3.zero;
        fadeState = fadeIn;
    }
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScaleVect, Time.deltaTime * speed);

        interactText.color = Utils.WithAlpha(Color.white, Mathf.Lerp(interactText.color.a, targetScaleVect.x, Time.deltaTime * speed));

		transform.position = Utils.WithYVal(transform.position, Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * speed * 1.5f));
    }
}
