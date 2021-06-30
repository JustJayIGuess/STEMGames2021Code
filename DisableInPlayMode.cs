using UnityEngine;

[ExecuteAlways]
public class DisableInPlayMode : MonoBehaviour
{
#if UNITY_EDITOR
	private void Update()
	{
		if (Application.isPlaying)
		{
			gameObject.SetActive(false);
		}
	}
#endif
}
