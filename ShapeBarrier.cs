using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeBarrier : MonoBehaviour
{
	public BodyManager.PlayerShapes allowPlayer = BodyManager.PlayerShapes.CubePlayer;
	public BodyManager body;
	public Collider collider1;
	public Collider parentPlayerCollider;
	public Collider lifeIndCollider;
	public Material indMat;
	public LifeIndicator lifeInd;
	public float indIntensity = 1f;

	private Collider[] playerColliders;
	private bool isInTrigger = false;
	private ColorScheme colorScheme;

	private IEnumerator ResetToAfter(Color col, float after)
	{
		yield return new WaitForSeconds(after);
		indMat.SetColor("_EmissionColor", col);
	}

	private void OnEnable()
	{
		BodyManager.OnSwitchBody += UpdateTrigger;
	}

	private void OnDisable()
	{
		BodyManager.OnSwitchBody -= UpdateTrigger;
	}

	private void Awake()
	{
		colorScheme = FindObjectOfType<ColorScheme>();
		lifeInd = FindObjectOfType<LifeIndicator>();
		lifeIndCollider = lifeInd.gameObject.GetComponent<Collider>();
		collider1 = Utils.GetComponentInSiblings<BoxCollider>(gameObject);
		parentPlayerCollider = FindObjectOfType<PlayerMovement>().gameObject.GetComponent<Collider>();
		playerColliders = parentPlayerCollider.GetComponentsInChildren<Collider>();
		body = FindObjectOfType<BodyManager>();
	}

	private void Start()
	{
		indMat.SetColor("_EmissionColor", indIntensity * colorScheme.Colors[ColorScheme.SchemeColor.LBlue]);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isInTrigger = true;
			bool _allow = body.currentShape == allowPlayer;

			if (_allow)
			{
				indMat.SetColor("_EmissionColor", indIntensity * colorScheme.Colors[ColorScheme.SchemeColor.Green]);
			}
			else
			{
				lifeInd.UnAbleFlash();
				indMat.SetColor("_EmissionColor", indIntensity * colorScheme.Colors[ColorScheme.SchemeColor.Red]);
			}

			Physics.IgnoreCollision(collider1, parentPlayerCollider, _allow);
			Physics.IgnoreCollision(collider1, lifeIndCollider, _allow);
			foreach (Collider c in playerColliders)
			{
				Physics.IgnoreCollision(collider1, c, _allow);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			isInTrigger = false;
			StopAllCoroutines();
			StartCoroutine(ResetToAfter(indIntensity * colorScheme.Colors[ColorScheme.SchemeColor.LBlue], 0.5f));
		}
	}

	public void UpdateTrigger(BodyManager.PlayerShapes newPlayer)
	{
		if (isInTrigger)
		{
			bool _allow = newPlayer == allowPlayer;

			if (_allow)
			{
				indMat.SetColor("_EmissionColor", indIntensity * colorScheme.Colors[ColorScheme.SchemeColor.Green]);
			}
			else
			{
				indMat.SetColor("_EmissionColor", indIntensity * colorScheme.Colors[ColorScheme.SchemeColor.Red]);
			}

			Physics.IgnoreCollision(collider1, parentPlayerCollider, _allow);
			Physics.IgnoreCollision(collider1, lifeIndCollider, _allow);
			foreach (Collider c in playerColliders)
			{
				Physics.IgnoreCollision(collider1, c, _allow);
			}
		}
		else
		{
			StopAllCoroutines();
			StartCoroutine(ResetToAfter(indIntensity * colorScheme.Colors[ColorScheme.SchemeColor.LBlue], 0.5f));
		}
	}
}
