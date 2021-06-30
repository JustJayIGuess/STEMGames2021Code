using UnityEngine;

public class GoToSubOnInteract : MonoBehaviour
{
	PodiumInteract pod;
	Subtitles subs;
	public string goTo;
	//public bool 

	private void Awake()
	{
		pod = GetComponent<PodiumInteract>();
		subs = FindObjectOfType<Subtitles>();
	}

	private void OnEnable()
	{
		KeyCache.OnKeyDown += OnKeyDown;
	}

	private void OnDisable()
	{
		KeyCache.OnKeyDown -= OnKeyDown;
	}

	private void OnKeyDown(KeyCode key)
	{
		if (GameController.keyMaps[GameController.Controls.Interact].Contains(key))
		{
			if (pod.IsPlayerInRange(PodiumInteract.Range.Close))
			{
				subs.SkipToElement(goTo, subs.textQueue);
			}
		}
	}
}
