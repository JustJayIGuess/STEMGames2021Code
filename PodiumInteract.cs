using UnityEngine;
using System.Collections;

public class PodiumInteract : MonoBehaviour
{
	public GameObject intTxt;
	public Transform player;
	public Fade txtFade;
	public Transform txtContainer;
	public BodyManager.PlayerShapes exlusiveTo = BodyManager.PlayerShapes.None;

	public float displayThresh = 10f;
	public float riseThresh = 5f;
	
	public enum Range
	{
		Close,
		Far
	}

	public float scaleTo = 1f;
	public BodyManager.PlayerShapes switchTo;

	private float[] targHeights = new float[2];
	private float initHeight;

	private float riseOff = 1f;

	private IEnumerator ReturnToHeightAfter(float dur, float initRiseOff)
	{
		yield return new WaitForSeconds(dur);
		SetRiseOffset(initRiseOff);
	}

	public float[] GetHeights()
	{
		return targHeights;
	}


	public void ClickText(float bobAmount, float dur)
	{
		float temp = riseOff;
		SetRiseOffset(riseOff + bobAmount);
		StartCoroutine(ReturnToHeightAfter(dur, temp));
	}

	public bool IsPlayerInRange(Range whichRange, bool justX = true)
	{
		if (justX)
		{
			if (whichRange == Range.Close)
			{
				return Mathf.Abs(intTxt.transform.position.x - player.position.x) < riseThresh;
			}
			else
			{
				return Mathf.Abs(intTxt.transform.position.x - player.position.x) < displayThresh;
			}
		}
		else
		{
			if (whichRange == Range.Close)
			{
				return Vector3.Distance(Utils.WithOutZ(transform.position), Utils.WithOutZ(player.position)) < riseThresh;
			}
			else
			{
				return Vector3.Distance(Utils.WithOutZ(transform.position), Utils.WithOutZ(player.position)) < displayThresh;
			}
		}
	}

	public float GetXDistToPlayer()
	{
		return Vector3.Distance(Utils.WithOnlyX(intTxt.transform.position), Utils.WithOnlyX(player.position));
	}

	public float GetDistToPlayer()
	{
		return Vector3.Distance(transform.position, player.position);
	}

	public void SetRiseOffset(float offset)
	{
		//sets new riseOff and update target Heights
		riseOff = offset;
		UpdateTargHeights();
	}

	private void UpdateTargHeights()
	{
		//updates target heights based on riseOff
		targHeights[0] = initHeight;
		targHeights[1] = initHeight + riseOff;
	}

	public void SetContHeight(float newHeight)
	{
		txtContainer.position.Set(txtContainer.position.x, newHeight, txtContainer.position.z);
	}

	private void Awake()
	{
		player = FindObjectOfType<PlayerMovement>().transform;
	}

	private void Start()	//Called when game starts
	{
		initHeight = intTxt.transform.position.y;
		UpdateTargHeights();
		SetContHeight(0f);
		txtFade.SetTargetElevation(targHeights[0]);
	}

	private void LateUpdate()
	{

		//If player is in far range, make the text display
		if (IsPlayerInRange(Range.Far))
		{
			//if player is also in close range, make text rise up
			if (IsPlayerInRange(Range.Close))
			{
				txtFade.SetTargetElevation(targHeights[1]);
			}
			else    //if player is in far range but not close range, dont make text rise up
			{
				txtFade.SetTargetElevation(targHeights[0]);

			}
			//If text isn't displaying and player is in range, fade it in
			if (txtFade.GetFadeState() == Fade.fadeOut)
			{
				txtFade.FadeIn();
			}
		} else    //If text is displaying but player has moved out of range, fade text out
		{
			txtFade.FadeOut();
		}
	}
}
