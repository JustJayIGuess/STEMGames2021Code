using UnityEngine;

public class Interaction : MonoBehaviour
{
	public PodiumInteract[] pods;
	public ScalePlayer scaler;
	public BodyManager bodyManager;

	[Space(10)]
	public float textClickDuration;

	public void SetScale(float scale)
	{
		foreach (PodiumInteract pod in pods)
		{
			pod.SetContHeight(scale);
		}
	}

	private int GetClosestPodIndex(PodiumInteract[] ints)
	{
		float minDist = Mathf.Infinity;
		int index = 0;
		int res = -1;	// Return bad number if something goes wrong

		foreach (PodiumInteract interactDisplay in ints)
		{
			if (interactDisplay.GetDistToPlayer() < minDist)
			{
				minDist = interactDisplay.GetDistToPlayer();
				res = index;
			}
			index++;
		}

		return res;
	}

	private void Awake()
	{
		scaler = GetComponent<ScalePlayer>();
		bodyManager = GetComponentInChildren<BodyManager>();
	}

	// Update is called once per frame
	void Update()
    {
		if (Input.GetKeyDown("f"))
		{

			bool _isInRange = false;

			foreach (PodiumInteract interact in pods)
			{
				if (interact.IsPlayerInRange(PodiumInteract.Range.Close, false))
				{
					_isInRange = true;
				}
			}

			if (_isInRange)
			{
				// Aquire dependencies
				bodyManager = GetComponentInChildren<BodyManager>();
				int index = GetClosestPodIndex(pods);

				if (pods[index].exlusiveTo == BodyManager.PlayerShapes.None || pods[index].exlusiveTo == bodyManager.currentShape)
				{
					pods[index].ClickText(-1f, textClickDuration);
					AudioController.Play("Interact-Sound");

					if (pods[index].CompareTag("Scaler"))
					{
						scaler.ScaleTo(pods[index].GetComponent<PodiumInteract>().scaleTo);
					}
					else if (pods[index].CompareTag("BodySwitch"))
					{
						bodyManager.EnablePlayer(pods[index].GetComponent<PodiumInteract>().switchTo);
					}
				}
				else
				{
					Utils.GetComponentInSiblings<LifeIndicator>(gameObject).UnAbleFlash();
				}
			}
		}
	}
}
