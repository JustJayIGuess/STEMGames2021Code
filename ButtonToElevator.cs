/*
 * Coded by Jay Johnston
 * This controls the elevator with respect to button presses, derived from my button base class
 */ 

public class ButtonToElevator : Button
{
	private Elevator elevator;

	public override void OnButtonPress()
	{
		if (elevator.isMoving)
		{
			canPress = false;
		}
		else
		{
			canPress = true;
		}

		elevator.CallElevator(1f, 10f);
	}

	public override void ButtonAwake()
	{
		elevator = transform.parent.GetComponent<Elevator>();
		buttonMaterial.SetColor("_EmissionColor", buttonDepressedColor);
	}
}
