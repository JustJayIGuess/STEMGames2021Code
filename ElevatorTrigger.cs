using UnityEngine;

public class ElevatorTrigger : MonoBehaviour
{
	public delegate void EnterElevatorAction();
	public static event EnterElevatorAction OnEnterElevator;

	public delegate void ExitElevatorAction();
	public static event ExitElevatorAction OnExitElevator;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("GroundTrigger"))
		{
			OnEnterElevator?.Invoke();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("GroundTrigger"))
		{
			OnExitElevator?.Invoke();
		}
	}
}
