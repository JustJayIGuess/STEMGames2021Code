using UnityEngine;

public class StartFromPreload : MonoBehaviour
{
	private void Awake()
	{

#if UNITY_EDITOR

		GameObject app = GameObject.Find("__app");
		if (app == null)
		{
			GameController.EditorLoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
		}

#endif

	}
}