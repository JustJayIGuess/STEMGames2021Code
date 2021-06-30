using System.Collections;
using UnityEngine;

//[ExecuteInEditMode]
public class CameraFollow : MonoBehaviour
{
    public Transform player;
	public ScalePlayer scaler;
    public Vector3 defaultCameraOffset = new Vector3(0f, 7f, -23f);
    public Vector3 startCameraOffset = new Vector3(0f, 0f, -1000f);
	public float defaultSpeed = 0.175f;

	private float dynamicSpeed;
    private Vector3 totalOffset;
    private CCControl cc;

	public void SetOffset(Vector3 newOffset)
	{
		defaultCameraOffset = newOffset;		// Set new camera offset
		OnScale(scaler.GetCurrScale());	// Update total offset
	}

	private void OnScale(float scale)
	{
		totalOffset = defaultCameraOffset * scale;
	}

	private void OnLevelEnd(int level)
	{
		StartCoroutine(LerpOffsetTo(new Vector3(0f, scaler.playerHeight * scaler.GetCurrScale(), scaler.GetCurrScale()), 2f));    // Begin moving target closer to player head to zoom in
		dynamicSpeed = 0.09f;   // Reduce lerping speed
	}

	private IEnumerator LerpOffsetTo(Vector3 target, float acceleration, float snap = 0.05f)
	{
		float progress = 0f;
		float speed = 0f;
		Vector3 startOffset = totalOffset;

		while (Vector3.Distance(target, totalOffset) > snap)
		{
			totalOffset = Vector3.Lerp(startOffset, target, progress);

			progress += speed * Time.fixedDeltaTime;
			speed += acceleration * Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
		totalOffset = target;
	}

	private void OnEnable()
	{
		ScalePlayer.OnScale += OnScale;
		LevelEnd.OnLevelEnd += OnLevelEnd;
	}

	private void OnDisable()
	{
		ScalePlayer.OnScale -= OnScale;
		LevelEnd.OnLevelEnd -= OnLevelEnd;
	}

	private void Awake()
	{
		cc = FindObjectOfType<CCControl>();
		totalOffset = defaultCameraOffset * scaler.GetCurrScale();
		dynamicSpeed = 0f;
		transform.position = startCameraOffset;
	}

	private void Start()
	{
		StartCoroutine(LerpSpeedTo(defaultSpeed, 0.1f));
	}

	private IEnumerator LerpSpeedTo(float targetSpeed, float lerpSpeed, float snap = 0.05f)
	{
		float progress = 0;

		yield return null;

		while (Mathf.Abs(targetSpeed - dynamicSpeed) > snap)
		{
			dynamicSpeed = Mathf.Lerp(dynamicSpeed, targetSpeed, progress);

			progress += Time.fixedDeltaTime * lerpSpeed;
			yield return new WaitForFixedUpdate();
		}

		dynamicSpeed = targetSpeed;
	}

	void FixedUpdate()
    {
		transform.position = Vector3.Lerp(transform.position, totalOffset + player.position, dynamicSpeed);
		cc.SetFocalDistance(Vector3.Magnitude(player.position - transform.position));	// TODO: Make this an event to improve performance (OnUpdateFocalLength)
    }
}
