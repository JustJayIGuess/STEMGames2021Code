using UnityEngine;

//Uses a bitmask to decide which colors to affect:	
//										Bit Number:		4					3					2				1				0
//										Value:			16					8					4				2				1
//										Descripition:	Fresnel Color		Light Intensity		Light Color		Emission Color	Albedo Color
//-------------------------------------------------------------------------------------------------------------------------------------------
//		e.g. 0110 affects light color and emission color. this is inputted as its decimal value, 0110 = (int) 6

[ExecuteInEditMode]
public class ColorManager : MonoBehaviour
{
	public enum ShaderID
	{
		CubeShader = 0,
		LevelShader = 1,
		OrbShader = 2,
		FresnelDef = 3,
	}

	public Material mat;
	public SpriteRenderer sprt;
	public ColorScheme.SchemeColor schemeColor = ColorScheme.SchemeColor.Black;
	public ColorScheme.SchemeColor schemeColor2 = ColorScheme.SchemeColor.Black;
	public ColorScheme.SchemeColor fresnelColor = ColorScheme.SchemeColor.Black;
	public byte colorMask;

	[Range(0f, 5f)]
	public float emIntensity;
	[Range(0f, 5f)]
	public float lightIntensity;

	[Space(10)]
	public bool stals = false;

#if UNITY_EDITOR
	[Space(20)]
	[TextArea]
	public string comments;
#endif

	private Light[] lights;
	private ColorScheme colorScheme;

	private bool GetBit(byte b, int bit)
	{
		return 0 != ((b >> bit) & 1);
	}

	private static bool GetBit(byte b1, int[] bits)
	{
		byte tool = 0;
		foreach (int bit in bits)
		{
			tool = (byte)(tool | 1 << bit - 1);
		}

		return (tool & b1) != 0;
	}

	void OnValidate()
	{
		if (FindObjectOfType<ColorScheme>() != null)
		{
			colorScheme = FindObjectOfType<ColorScheme>();
			if (mat != null && colorScheme.hasLoaded && colorScheme.Colors.Count > 0)
			{

				mat.EnableKeyword("_EmissionColor");
				if (GetBit(colorMask, new int[] { 1, 2, 3 }))
				{
					lights = GetComponentsInChildren<Light>();
				}

				if (GetBit(colorMask, 0))
				{
					mat.SetColor("_BaseColor", colorScheme.Colors[schemeColor]);
				}

				if (GetBit(colorMask, 1))
				{
					mat.SetColor("_EmissionColor", colorScheme.Colors[schemeColor] * emIntensity);
				}

				if (GetBit(colorMask, 2))
				{
					if (stals)
					{
						foreach (Light light in lights)
						{
							Color stalColor = Color.black;

							switch (light.transform.parent.tag)
							{
								case "StalWhite":
									stalColor = colorScheme.Colors[ColorScheme.SchemeColor.White];
									break;
								case "StalGreen":
									stalColor = colorScheme.Colors[ColorScheme.SchemeColor.Green];
									break;
								case "StalRed":
									stalColor = colorScheme.Colors[ColorScheme.SchemeColor.Red];
									break;
								default:
									Debug.LogError("StalColor Error: stal \'" + light.transform.parent.name + "\' does not have valid tag!\nValid tags include: StalWhite, StalGreen, StalRed, etc.");
									break;
							}

							light.color = stalColor;
						}
					}
					else
					{
						foreach (Light light in lights)
						{
							light.color = colorScheme.Colors[schemeColor];
						}
					}
				}
				if (GetBit(colorMask, 3))
				{
					foreach (Light light in lights)
					{
						light.intensity = lightIntensity;
					}
				}
				if (GetBit(colorMask, 4))
				{
					if (mat.HasProperty("_FresnelColor"))
					{
						mat.SetColor("_FresnelColor", colorScheme.Colors[fresnelColor]);
					}
					else
					{
						Debug.LogError(mat.name + " has no property _FresnelColor");
					}
				}
			}

			if (sprt != null && colorScheme.hasLoaded)
			{
				sprt.color = colorScheme.Colors[schemeColor];
			}

		}
	}
}
