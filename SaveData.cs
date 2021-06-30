using System;

[Serializable]
public class SaveData
{
	public readonly int level;
	public readonly float[] pos;
	public readonly int playerBody;
	public readonly float size;
	public readonly float time;
	public readonly float elevatorProgress;
	public readonly float elevatorLevel;
	public readonly float elevatorAddElevation;
	public readonly bool elevatorDoorOpenned;
	public readonly float elevatorSpeed;
	public readonly bool elevatorMoving;
	public readonly int doorState;

	public SaveData(PlayerMovement player, BodyManager body, Elevator elevator = null, ElevatorDoor elevatorDoor = null)
	{
		level = GameController.GetScene();
		pos = Utils.Vector3ToFloatArr(player.transform.position);
		playerBody = (int)body.currentShape;
		size = player.scaler.GetCurrScale();
		time = Timer.GetTime();
		if (elevator != null)
		{
			elevatorProgress = elevator.Progress;
			elevatorLevel = elevator.level;
			elevatorAddElevation = elevator.currAddElevation;
			elevatorDoorOpenned = !Utils.IntToBool((int)elevatorDoor.CurrState);
			elevatorSpeed = elevator.currSpeed;
			elevatorMoving = elevator.isMoving;
		}
	}
}
