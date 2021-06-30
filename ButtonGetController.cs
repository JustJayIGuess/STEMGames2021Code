using UnityEngine;

public class ButtonGetController : MonoBehaviour
{
	public void NextLevel()
	{
		GameController.LoadNextScene();
	}

	public void EndGame()
	{
		GameController.QuitGame();
	}

	public void RestartGame()
	{
		GameController.StartGame();
	}

	public void ClickSound()
	{
		AudioController.Play("Click");
	}
}
