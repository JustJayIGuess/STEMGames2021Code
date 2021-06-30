public class ButtonToDoor : Button
{
	public Trapdoor door;

	public override void OnButtonPress()
	{
		door.SetOpenState();
	}

	private void Awake()
	{
		door.SetOpenState(false);
		buttonMaterial.SetColor("_EmissionColor", buttonDepressedColor);
	}
}
