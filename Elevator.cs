using System;
using System.Collections;
using UnityEngine;

public class Elevator : MonoBehaviour
{
	public enum ElevatorState
	{
		Top,
		Bottom
	}

	private Rigidbody elevatorRb;
	private Rigidbody buttonPlateRb;
	private float yOffset;
	public float Progress { get; private set; } = 0f;
	private PlayerMovement player;
	private ElevatorDoor door1;
	private ElevatorDoor door2;
	private SpriteRenderer arrow;

	private static readonly float elevatorHeight = 17f;
	private static readonly Vector3 rayDisplacement = new Vector3(5f, elevatorHeight, 10f);

	[HideInInspector]
	public bool isMoving = false;
	[HideInInspector]
	public float level = 0f;

	public AnimationCurve heightCurve;
	public ElevatorState elevatorState = ElevatorState.Bottom;

	[HideInInspector]
	public float currAddElevation;
	[HideInInspector]
	public float currSpeed;
	

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(transform.position, 0.5f);
		Ray ray = new Ray(transform.position + Utils.WithYVal(rayDisplacement, 0.125f), Vector3.down); // TODO: make this more memory efficient
		Gizmos.color = Color.red;
		Gizmos.DrawRay(ray);
		Physics.Raycast(ray, out RaycastHit hitInfo, 1 << 12);
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(hitInfo.point, 2f);
	}

	private float GetDistanceToCeiling()
	{
		Ray ray = new Ray(transform.position + rayDisplacement, Vector3.up);
		Physics.Raycast(ray, out RaycastHit hitInfo, 1 << 12);
		return hitInfo.distance - 0.125f;
	}

	private float GetDistanceToFloor()
	{
		Ray ray = new Ray(transform.position + Utils.WithYVal(rayDisplacement, 0.125f), Vector3.down);
		Physics.Raycast(ray, out RaycastHit hitInfo, 1 << 12);
		return -hitInfo.distance + 0.125f;
	}

	// Deprecated
	public void CallElevator(float delay, float addElevation, float speed, Action callback = null)
	{
		if (!isMoving)
		{
			door1.SetDoorOpenned(false);
			if (door2 != null)
			{
				door2.SetDoorOpenned(false);
			}
			if (elevatorState == ElevatorState.Bottom)
			{
				arrow.flipY = false;
				elevatorState = ElevatorState.Top;
			}
			else
			{
				arrow.flipY = true;
				elevatorState = ElevatorState.Bottom;
			}
			StartCoroutine(ElevatorDelay(delay, addElevation, speed, callback));
			isMoving = true;
			player.DisableHover();
			KeyCache.Instance.LockControls(GameController.keyMaps[GameController.Controls.Up]);
		}
	}

	public void CallElevator(float delay, float speed, Action callback = null)
	{
		if (!isMoving)
		{
			float addElevation = elevatorState == ElevatorState.Bottom ? GetDistanceToCeiling() : GetDistanceToFloor();
			door1.SetDoorOpenned(false);
			if (door2 != null)
			{
				door2.SetDoorOpenned(false);
			}
			if (elevatorState == ElevatorState.Bottom)
			{
				arrow.flipY = false;
				elevatorState = ElevatorState.Top;
			}
			else
			{
				arrow.flipY = true;
				elevatorState = ElevatorState.Bottom;
			}
			StartCoroutine(ElevatorDelay(delay, addElevation, speed, callback));
			isMoving = true;
			player.DisableHover();
			KeyCache.Instance.LockControls(GameController.keyMaps[GameController.Controls.Up]);
		}
	}

	private IEnumerator ElevatorDelay(float delay, float elevation, float speed, Action callback = null)
	{
		float startTime = Time.time;

		while (true)
		{
			if (startTime + delay < Time.time)
			{
				break;
			}
			yield return null;
		}
		StartCoroutine(GoToElevation(elevation, speed, 0f, callback));
	}

	// (maybe) TODO: Make elevator speed automatic
	private IEnumerator GoToElevation(float addElevation, float speed, float startProgress = 0f, Action callback = null)
	{
		buttonPlateRb.isKinematic = true;

		float timeScaleFactor = Mathf.Abs(speed / addElevation);
		float curveVal;
		bool done = false;

		Progress = startProgress;

		// Data in case of save
		currAddElevation = addElevation;
		currSpeed = timeScaleFactor;

		// If elevator was loaded from save and must teleport to position
		if (Progress != 0f)
		{
			SetElevation(addElevation * heightCurve.Evaluate(Progress * timeScaleFactor), false);

			if (heightCurve.Evaluate(Progress * timeScaleFactor) == 1f)
			{
				done = true;
			}

			Progress += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		while (!done)
		{
			curveVal = heightCurve.Evaluate(Progress * timeScaleFactor);

			SetElevation(addElevation * curveVal);

			if (curveVal == 1f)
			{
				done = true;
			}

			Progress += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}

		isMoving = false;
		level += addElevation;
		yOffset = transform.position.y;
		door1.SetDoorOpenned(true);
		if (door2 != null)
		{
			door2.SetDoorOpenned(true);
		}

		player.EnableHover();
		KeyCache.Instance.UnlockControls(GameController.keyMaps[GameController.Controls.Up]);
		buttonPlateRb.isKinematic = false;

		callback?.Invoke();

		// Data reset in case of loaded save file
		currAddElevation = 0f;
		currSpeed = 1f;
	}

	public void LoadElevatorState(float _progress, float _level, float _addElevation, bool doorOpenned, float _speed, bool _isMoving)
	{
		SetElevation(_level, false);
		isMoving = _isMoving;
		level = _level;
		yOffset = transform.position.y;
		door1.SetDoorOpenned(doorOpenned, 1f, true);
		if (door2 != null)
		{
			door2.SetDoorOpenned(doorOpenned, 1f, true);
		}

		if (_isMoving)
		{
			player.DisableHover();
			StopAllCoroutines();
			StartCoroutine(GoToElevation(_addElevation, _speed, _progress));
		}
	}

	public void SetElevation(float elevatorElevation, bool interpolate = true)
	{
		if (interpolate)
		{
			elevatorRb.MovePosition(Utils.WithYVal(transform.position, yOffset + elevatorElevation));
			buttonPlateRb.MovePosition(buttonPlateRb.position + (elevatorRb.velocity * Time.fixedDeltaTime));
		}
		else
		{
			transform.position = Utils.WithYVal(transform.position, yOffset + elevatorElevation);
		}
	}

	private void Awake()
	{
		elevatorRb = GetComponent<Rigidbody>();
		SpringJoint _buttonPlate = GetComponentInChildren<SpringJoint>();
		buttonPlateRb = _buttonPlate == null ? GetComponentInChildren<ButtonPlateMovement>().GetComponent<Rigidbody>() : _buttonPlate.GetComponent<Rigidbody>();    // There's probably a much better way to do this

		yOffset = transform.position.y;
		player = FindObjectOfType<PlayerMovement>();
		door1 = GetComponentsInChildren<ElevatorDoor>()[0];
		Transform[] children = GetComponentsInChildren<Transform>();
		foreach (Transform child in children)
		{
			if (child.CompareTag("ElevatorArrow"))
			{
				arrow = child.GetComponent<SpriteRenderer>();
			}
		}

		try
		{
			door2 = GetComponentsInChildren<ElevatorDoor>()[1];
		}
		catch (IndexOutOfRangeException)
		{
			door2 = null;
		}
	}

	private void Start()
	{
		door1.SetDoorOpenned(true);
		if (door2 != null)
		{
			door2.SetDoorOpenned(true);
		}

		// debug for elevator
		//CallElevator(0f, 10f, () => CallElevator(1f, 10f));
	}
}
