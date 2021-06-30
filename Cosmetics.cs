using System;
using System.Collections.Generic;
using UnityEngine;

public class Cosmetics : MonoBehaviour
{
	public enum CosmeticSlots
	{
		Head,
		Glasses,
		Shoes
	}

	public List<Mesh> hats;

	public List<Mesh> shoes;

	public List<Mesh> glasses;

	public void AssignCosmeticToSlot(CosmeticSlots slot, Mesh newCosmetic)
	{
		MeshFilter targetSlot;
		try
		{
			targetSlot = GameObject.FindGameObjectWithTag(slot.ToString() + "Slot").GetComponent<MeshFilter>();	//Not the best way im sure, but it is not called very often so should be fine
		} catch (Exception e)
		{
			Debug.LogWarning($"Failed to get meshfilter of slot: {slot.ToString()}!");
			throw e;
		}
		targetSlot.mesh = newCosmetic;
	}
}
