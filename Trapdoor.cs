using System.Collections;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{

	private float currAngle = 0f;
	private float targetAngle = 0f;
	private Vector3 pivotPoint;

	private void ChangeAngle(bool? changeTo)
	{
		if (changeTo == null)
		{
			if (targetAngle == 90f)
			{
				targetAngle = 0f;
			}
			else
			{
				targetAngle = 90f;
			}
		}
		else if (changeTo == true)
		{
			targetAngle = 90f;
		}
		else
		{
			targetAngle = 0f;
		}
	}

	private IEnumerator SwingTo(bool? open, float speed = 1f, float lerpSnap = 0.1f)
	{
		ChangeAngle(open);

		while (currAngle != targetAngle)
		{
			if (Mathf.Abs(targetAngle - currAngle) < lerpSnap)
			{
				currAngle = targetAngle;
			}

			currAngle = Mathf.LerpAngle(currAngle, targetAngle, Time.deltaTime * speed);
			transform.localRotation = Quaternion.Euler(Utils.WithZVal(transform.localRotation.eulerAngles, currAngle));

			yield return null;
		}
	}

	public void SetOpenState(bool? open = null)
	{
		StopAllCoroutines();
		StartCoroutine(SwingTo(open));
	}
}
