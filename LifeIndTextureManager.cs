using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeIndTextureManager : MonoBehaviour
{
	public enum LifeIndTextures
	{
		LeftWall,
		RightWall,
		Floor
	}

	public Material material;
	public Texture2D leftWallTexture;
	public Texture2D rightWallTexture;
	public Texture2D floorTexture;
    
	public void SwitchTexture(LifeIndTextures texture)
	{
		switch (texture)
		{
			case LifeIndTextures.LeftWall:
				material.SetTexture("_EmissionMap", leftWallTexture);
				break;
			case LifeIndTextures.RightWall:
				material.SetTexture("_EmissionMap", rightWallTexture);
				break;
			case LifeIndTextures.Floor:
				material.SetTexture("_EmissionMap", floorTexture);
				break;
			default:
				material.SetTexture("_EmissionMap", floorTexture);
				break;
		}
	}
}
