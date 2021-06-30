/*
 * Coded by Jay Johnston
 * This controls the door behaviour on the elevator
 */ 

using System.Collections;
using UnityEngine;

public class ElevatorDoor : MonoBehaviour
{
	public enum States
	{
		Open,
		Closed
	}
	public AnimationCurve curve;

	[HideInInspector]
	public States CurrState { get; private set; } = States.Closed;

	private Collider doorCollider;
	private MeshRenderer doorRenderer;
	private float progress = 1f;

	private IEnumerator SetState(States state, float speed)
	{
		float sign = -1f;

		if (state == States.Closed)
		{
			doorCollider.enabled = true;
			doorRenderer.enabled = true;
			sign = 1f;
		}

		CurrState = state;
		float nextProgVal = progress + Time.deltaTime * speed * sign;

		while (nextProgVal > 0f && nextProgVal < 1f)
		{
			transform.localScale = Utils.WithZVal(transform.localScale, curve.Evaluate(progress));

			progress = nextProgVal;
			nextProgVal = progress + Time.deltaTime * speed * sign;
			yield return null;
		}
		if (state == States.Open)
		{
			doorCollider.enabled = false;
			doorRenderer.enabled = false;
		}
		else
		{
			transform.localScale = Utils.WithZVal(transform.localScale, 1f);
		}
	}

	public void SetDoorOpenned(bool? openned = null, float speed = 1.5f, bool instant = false)
	{
		if (!instant)
		{
			StopAllCoroutines();
			if (openned == null)
			{
				print("Oh no.");
				StartCoroutine(SetState(CurrState == States.Open ? States.Closed : States.Open, speed));
				return;
			}
			bool nonNullOpenned = openned == true;
			if (nonNullOpenned && CurrState != States.Open)
			{
				StartCoroutine(SetState(States.Open, speed));
			}
			else if (!nonNullOpenned && CurrState != States.Closed)
			{
				StartCoroutine(SetState(States.Closed, speed));
			}
		}
		else
		{
			StopAllCoroutines();
			bool nonNullOpenned = openned == true;
			if (nonNullOpenned)
			{
				transform.localScale = Utils.WithZVal(transform.localScale, 0f);
				progress = 0.0001f;
				doorCollider.enabled = false;
				doorRenderer.enabled = false;
				CurrState = States.Open;
			}
			else
			{
				transform.localScale = Utils.WithZVal(transform.localScale, 1f);
				progress = 0.9999f;
				doorCollider.enabled = true;
				doorRenderer.enabled = true;
				CurrState = States.Closed;
			}
		}
	}

	private void Awake()
	{
		doorCollider = GetComponent<Collider>();
		doorRenderer = GetComponent<MeshRenderer>();
	}
}
