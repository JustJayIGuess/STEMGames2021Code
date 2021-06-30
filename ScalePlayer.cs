using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScalePlayer : MonoBehaviour
{
	private Vector3 currScale = new Vector3(1f, 1f, 1f);
	private Vector3 targetScale = new Vector3(1f, 1f, 1f);

	public readonly float playerHeight = 9.496286f;
	public readonly float playerColliderHeight = 9.811684f;
	private readonly float[] bounds = { 0.25f, 4f };
	private readonly float gravCache = -60f;
	private readonly float scaleAmount = 2f;
	private float prevScale;

	public LifeIndicator lifeInd;
	public ArmatureManager armature;
	public TextMeshPro sizeInd;
	public Interaction interactor;
	public Transform[] wallCheck;

	private void OnDestroy()
	{
		Physics.gravity = Utils.FromY(gravCache);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.1f);
	}
	

	public delegate void ScaleAction(float scale);
	public static event ScaleAction OnScale;


	private void UpdateCurrScale()
	{
		Physics.gravity = Utils.FromY(gravCache * currScale.x);
		currScale = targetScale;
		sizeInd.text = currScale.x.ToString() + "x";
		interactor.SetScale(currScale.x);
	}

	public void SetScale(float scale)
	{
		Vector3 scaleVector = scale * Vector3.one;
		transform.localScale = scaleVector;
		targetScale = scaleVector;
		UpdateCurrScale();
		OnScale?.Invoke(targetScale.x);
	}

	public void ScaleTo(float scale)
	{
		UpdateCurrScale();
		Vector3 targetScaleTemp = Vector3.one * scale;

		Ray ray = new Ray(transform.position, Vector3.up);
		if (!Physics.Raycast(ray, playerHeight * targetScaleTemp.x, 1 << 12))
		{
			targetScale = targetScaleTemp;

			OnScale?.Invoke(targetScale.x);
		}
		else
		{
			lifeInd.UnAbleFlash();
		}

		UpdateCurrScale();
	}

	public void ScaleUp(int scaleCount = 1)
	{
		UpdateCurrScale();
		if (GetCurrScale() < bounds[1])
		{
			Vector3 targetScaleTemp = new Vector3();
			while (scaleCount > 0)
			{
				targetScaleTemp = currScale * scaleAmount;
				scaleCount--;
			}

			Ray ray = new Ray(transform.position, Vector3.up);
			if (!Physics.Raycast(ray, playerHeight * targetScaleTemp.x, 1 << 12))
			{
				targetScale = targetScaleTemp;

				OnScale?.Invoke(targetScale.x);
			}
			else
			{
				lifeInd.UnAbleFlash();
			}

			UpdateCurrScale();
		}
		else
		{
			lifeInd.UnAbleFlash();
		}
	}

	public void ScaleDown(int scaleCount = 1)
	{
		UpdateCurrScale();
		if (GetCurrScale() > bounds[0])
		{

			while (scaleCount > 0)
			{
				targetScale = currScale / scaleAmount;
				scaleCount--;
			}
			UpdateCurrScale();

			OnScale?.Invoke(targetScale.x);
		}
		else
		{
			lifeInd.UnAbleFlash();
		}
	}

	public float GetCurrScale()
	{
		UpdateCurrScale();
		return currScale.x;
	}

	private void LerpScale(float speed)
	{
		transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
		armature = BodyManager.GetActiveRig();
		if (armature != null)
		{
			armature.UpdateArmatures(); //Update armatures so that they correctly adapt to change in size
		}
	}

	private void Awake()
	{
		lifeInd = Utils.GetComponentInSiblings<LifeIndicator>(gameObject);
		interactor = GetComponent<Interaction>();
	}

	// Update is called once per frame
	void Update()
    {
		LerpScale(10f);             //Lerp the scale at a speed of 10x
		if (prevScale != targetScale.x)
		{
			prevScale = targetScale.x;  //update prevScale
		}

		if (Input.GetKeyDown(","))
		{
			ScaleDown();
		}
		if (Input.GetKeyUp("."))
		{
			ScaleUp();
		}
	}
}