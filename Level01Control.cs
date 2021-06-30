using System.Collections;
using UnityEngine;

public class Level01Control : MonoBehaviour
{
	private bool hasReachedMovementBeginning = false;

	private void OnEnable()
	{
		Subtitles.OnSubtitleDisplay += OnSubtitlesDisplay;
	}

	private void OnDisable()
	{
		Subtitles.OnSubtitleDisplay -= OnSubtitlesDisplay;
	}

	private void OnSubtitlesDisplay(string name)
	{
		if (name == "ENABLECONTROLS")
		{
			hasReachedMovementBeginning = true;
		}
	}

	private IEnumerator LevelBeginSequence()
	{
		KeyCache keys = KeyCache.Instance;
		keys.LockControls();

		yield return new WaitUntil(() => hasReachedMovementBeginning);
		yield return new WaitForSeconds(1f);

		keys.UnlockControls();
	}

	private void Start()
	{
		StartCoroutine(LevelBeginSequence());
	}

	private void Update()
	{
		if (Input.GetKeyDown("'"))
		{
			KeyCache.Instance.UnlockControls();
		}
	}
}
