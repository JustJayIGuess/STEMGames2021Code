using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

// Main controller for game - DDOL singleton derived from Monobehaviour
public class GameController : MonoSingleton<GameController>
{
	public enum Controls
	{
		Interact,
		Up,
		Down,
		Left,
		Right
	}

	private static int[] menuScenes;
	private static int currScene = 0;
	public static int firstLevel = 2;
	public static readonly int levelCount = 6;      // @@@@@@@@@@@@@ CHANGE THIS WHEN ADDING LEVELS! @@@@@@@@@@@@@
	public static readonly string uname = "Jay";
	public KeyCache keys;
	public ForwardRendererData rendererData;
	private static bool isLoadingScene;

	public static Dictionary<Controls, List<KeyCode>> keyMaps = new Dictionary<Controls, List<KeyCode>>();

	private static void EditControl(Controls control, KeyCode[] codes)
	{
		keyMaps[control] = new List<KeyCode>();
		foreach (KeyCode key in codes)
		{
			keyMaps[control].Add(key);
		}
	}

	private void Awake()
	{
		// Set default controls
		EditControl(Controls.Interact,	new KeyCode[] { KeyCode.F, KeyCode.Mouse1					});
		EditControl(Controls.Up,		new KeyCode[] { KeyCode.W, KeyCode.Space, KeyCode.UpArrow	});
		EditControl(Controls.Down,		new KeyCode[] { KeyCode.S, KeyCode.DownArrow				});
		EditControl(Controls.Left,		new KeyCode[] { KeyCode.A, KeyCode.LeftArrow				});
		EditControl(Controls.Right,		new KeyCode[] { KeyCode.D, KeyCode.RightArrow				});

		//foreach (KeyValuePair<Controls, List<KeyCode>> pair in keyMaps)
		//{
		//	print(pair.Key);
		//	foreach (KeyCode key in pair.Value)
		//	{
		//		print("\t" + key);
		//	}
		//}

		menuScenes = new int[] { 1, SceneManager.sceneCount - 1 };  //Start menu and game over scenes
		SetXRayEnabled(false);
		LoadNextScene();
	}

	public static void SaveGame()
	{
		PlayerMovement player = FindObjectOfType<PlayerMovement>();
		if (player != null)
		{
			BodyManager body = FindObjectOfType<BodyManager>();
			Elevator elevator = FindObjectOfType<Elevator>();
			ElevatorDoor elevatorDoor = FindObjectOfType<ElevatorDoor>();
			StoreData.SavePlayerData(player, body, elevator, elevatorDoor);
		}
	}

	public static void LoadGame()
	{

		SaveData data = StoreData.Load<SaveData>(StoreData.playerDataSavePath);
		StaticCoroutine.Begin(LoadData(data));
	}

	// Coroutine to wait until level indicatoed by save data has loaded before loading other data (e.g. position)
	private static IEnumerator LoadData(SaveData data)
	{
		isLoadingScene = true;
		SceneManager.LoadSceneAsync(data.level);
		yield return new WaitUntil(() => !isLoadingScene);

		PlayerMovement player = FindObjectOfType<PlayerMovement>();
		BodyManager body = FindObjectOfType<BodyManager>();
		Elevator elevator = FindObjectOfType<Elevator>();
		if (elevator != null)
		{
			ElevatorDoor elevatorDoor = elevator.GetComponentInChildren<ElevatorDoor>();
		}
		Timer.SetTime(data.time);

		player.transform.position = Utils.FloatArrToVector3(data.pos);
		player.scaler.SetScale(data.size);
		body.EnablePlayer((BodyManager.PlayerShapes)data.playerBody);
		if (elevator != null)
		{
			print("Loading elevator state");
			elevator.LoadElevatorState(data.elevatorProgress, data.elevatorLevel, data.elevatorAddElevation, data.elevatorDoorOpenned, data.elevatorSpeed, data.elevatorMoving);
		}
	}

	public static int GetLevel()
	{
		return SceneManager.GetActiveScene().buildIndex - firstLevel;
	}

	public static int GetScene()
	{
		return SceneManager.GetActiveScene().buildIndex;
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		isLoadingScene = false;
		if (FindObjectOfType<PlayerMovement>() == null)
		{
			Cursor.visible = true;
		}
		else
		{
			Cursor.visible = false;
			AudioController.Play("Level-Transition");
			KeyCache.Instance.UnlockControls();
		}
	}

	// This is just a quick coroutine so that when i start from a scene in editor, it automatically goes back to the preload scene to get all of the DDOL components first
	private static IEnumerator LoadController(int buildIndex)
	{
		yield return SceneManager.LoadSceneAsync(0);
		yield return SceneManager.LoadSceneAsync(buildIndex);

		//Cosmetics cosmetics = FindObjectOfType<Cosmetics>();
		//cosmetics.AssignCosmeticToSlot(Cosmetics.CosmeticSlots.Head, cosmetics.hats[0]);
	}

	private static IEnumerator LoadSceneAsyncWithDelay(float delay, int scene)
	{
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
		asyncOperation.allowSceneActivation = false;
		yield return new WaitForSeconds(delay);
		asyncOperation.allowSceneActivation = true;
	}

	public static void EditorLoadScene(int buildIndex)
	{
		StaticCoroutine.Begin(LoadController(buildIndex));
	}

	public static void LoadNextScene(float delay = 0f)
	{
		currScene = SceneManager.GetActiveScene().buildIndex + 1;
		if (currScene < SceneManager.sceneCountInBuildSettings)
		{
			if (!Array.Exists(menuScenes, e => e == currScene))
			{
				KeyCache keys = KeyCache.Instance;
				keys.ResetAll();
				keys.LockControls();
			}

			StaticCoroutine.Begin(LoadSceneAsyncWithDelay(delay, currScene));
		}
	}

	public static void LoadPrevScene(float delay = 0f)
	{
		currScene = SceneManager.GetActiveScene().buildIndex - 1;
		if (currScene >= 1)	//Exclude preload
		{
			if (!Array.Exists(menuScenes, e => e == currScene))
			{
				KeyCache.Instance.ResetAll();
			}
			StaticCoroutine.Begin(LoadSceneAsyncWithDelay(delay, currScene));
			//SceneManager.LoadSceneAsync(currScene);
		}
	}

	public static void QuitGame()
	{
		Application.Quit();
		print("Quit Game!");
	}

	public static void StartGame()
	{
		SceneManager.LoadScene(2);
	}

	public static void SetXRayEnabled(bool enabled)
	{
		Instance.rendererData.rendererFeatures[0].SetActive(enabled);
	}

	private void Update()
	{
		if (Input.GetKeyDown("q"))
		{
			QuitGame();
		}
		if (Input.GetKeyDown("n"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				LoadPrevScene();
			}
			else
			{
				LoadNextScene();
			}
		}
		if (Input.GetKeyDown(";"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				LoadGame();
			}
			else
			{
				SaveGame();
			}
		}
	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}
