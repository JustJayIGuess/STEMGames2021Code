using UnityEngine;

public class LevelEnd : MonoBehaviour
{
	public delegate void LevelEndAction(int level);
	public static LevelEndAction OnLevelEnd;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			OnLevelEnd?.Invoke(GameController.GetLevel());	// TODO: Make sure this is only called once in a level
			GameController.LoadNextScene(2f);
		}
	}
}
