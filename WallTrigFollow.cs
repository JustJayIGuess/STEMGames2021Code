using UnityEngine;
[ExecuteInEditMode]
public class WallTrigFollow : MonoBehaviour
{
	public RectTransform player;
	public ScalePlayer scaler;
	public Vector3 offset;

	private void OnEnable()
	{
		ScalePlayer.OnScale += OnScale;
	}

	private void OnDisable()
	{
		ScalePlayer.OnScale -= OnScale;
	}

	private void OnScale(float scale)
	{
		transform.localScale = Vector3.one * scale;
	}

	// Update is called once per frame
	void LateUpdate()
    {
		transform.position = Utils.WithXVal(player.position + (offset * scaler.GetCurrScale()), player.position.x + offset.x);//Utils.WithYVal(player.position + offset, player.position.y + (offset.y * scaler.GetCurrScale()));
    }
}
