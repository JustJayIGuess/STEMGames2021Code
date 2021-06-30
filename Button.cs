using System.Collections;
using UnityEngine;

public abstract class Button : MonoBehaviour
{
	public bool canPress = true;

	// Deprecated
	[HideInInspector]
	public Material buttonMaterial;

	[HideInInspector]
	public Renderer buttonRenderer;

	[ColorUsage(false, true)]
	public Color buttonDepressedColor;
	[ColorUsage(false, true)]
	public Color buttonPressedColor;
	[ColorUsage(false, true)]
	public Color buttonUnableColor;

	private MaterialPropertyBlock buttonMaterialPropertyBlock;

	private IEnumerator LightButton(bool on, float speed, float snap = 0.01f)
	{
		yield return null;
		buttonRenderer.GetPropertyBlock(buttonMaterialPropertyBlock);

		Color currColor = buttonMaterialPropertyBlock.GetColor("_EmissionColor");
		Color targetColor = on ? (canPress ? buttonPressedColor : buttonUnableColor) : buttonDepressedColor;

		while (Utils.AverageColorDifference(currColor, targetColor) > snap)
		{
			currColor = Color.Lerp(currColor, targetColor, Time.deltaTime * speed);
			buttonMaterialPropertyBlock.SetColor("_EmissionColor", currColor);

			buttonRenderer.SetPropertyBlock(buttonMaterialPropertyBlock);
			yield return null;
		}

		buttonMaterialPropertyBlock.SetColor("_EmissionColor", targetColor);
	
		buttonRenderer.SetPropertyBlock(buttonMaterialPropertyBlock);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("ButtonPlate"))
		{
			StopAllCoroutines();
			StartCoroutine(LightButton(true, 5f));
			OnButtonPress();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("ButtonPlate"))
		{
			StopAllCoroutines();
			StartCoroutine(LightButton(false, 3f));
		}
	}

	private void Awake()
	{
		buttonMaterialPropertyBlock = new MaterialPropertyBlock();
		buttonRenderer = GetComponentInChildren<Renderer>();
		buttonMaterial = buttonRenderer.material;

		ButtonAwake();
	}

	public virtual void ButtonAwake()
	{

	}

	public virtual void OnButtonPress()
	{
		
	}
}
