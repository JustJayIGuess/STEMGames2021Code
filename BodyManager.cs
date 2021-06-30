/*
 * Coded by Jay Johnston
 * This sets up the player's joints and rigidbody settings, to make sure those ragdolls are  c r i s p
 */ 

using System;
using System.Collections.Generic;
using UnityEngine;

public class BodyManager : MonoBehaviour
{
	public List<GameObject> playerBodies = new List<GameObject>();
	public List<ArmatureManager> armatures = new List<ArmatureManager>();

	public enum PlayerShapes {
		None,
		CubePlayer,
		SpherePlayer,
		TetrahedralPlayer
	};

	public PlayerShapes startAsShape = PlayerShapes.CubePlayer;

	[HideInInspector]
	public PlayerShapes currentShape = PlayerShapes.None;

	private void Awake()
	{
		playerBodies.Clear();
		int i = 0;
		foreach (string player in Enum.GetNames(typeof(PlayerShapes)))
		{
			GameObject _playerBody = GameObject.FindWithTag(player);

			if (_playerBody != null)
			{
				playerBodies.Add(_playerBody);

				if (_playerBody.GetComponentInChildren<ArmatureManager>() != null)
				{
					armatures.Add(_playerBody.GetComponentInChildren<ArmatureManager>());
				}
			}

			i++;
		}
	}

	public delegate void BodySwitchAction(PlayerShapes newPlayer);
	public static event BodySwitchAction OnSwitchBody;

	public void EnablePlayer(PlayerShapes newPlayer)
	{
		if (newPlayer != currentShape)
		{
			OnSwitchBody?.Invoke(newPlayer);	// Trigger OnSwitchBody event

			currentShape = newPlayer;

			//Get index of player
			int newPlayerIndex = (int)currentShape;

			// Handle some errors before they happen in case level setup is bad
			try
			{
				if (playerBodies[newPlayerIndex] == null)
				{
					Debug.LogError("BodyManager unable to switch to new body: " + newPlayer + ", as it does not exist in this scene!");
				}
			}
			catch(Exception e)
			{
				if (e is ArgumentOutOfRangeException)
				{
					Debug.LogWarning("Did you add in a \'None\' Player at the beginning?");
					throw;
				}
			}

			// Loop through players and set all except newPlayer unactive
			for (int i = 0; i < playerBodies.Count; i++)
			{
				if (i != newPlayerIndex)
				{
					playerBodies[i].SetActive(false);
				}
			}

			// Set new player active
			playerBodies[newPlayerIndex].SetActive(true);

			// If the new player has an ArmatureManager and is not null player, reset all player joints to their original localRotation
			if (newPlayerIndex <= armatures.Count && newPlayerIndex != 0)
			{
				armatures[newPlayerIndex - 1].ResetArmatures();
			}
		}
	}

	private void Start()
	{
		EnablePlayer(startAsShape);
	}

	// @@@@@@@@@@@@@@@@@@@@-REMOVE-THIS-IN-FINAL-GAME-@@@@@@@@@@@@@@@@@@@@
	private void Update()
	{
		if (Input.GetKeyDown("t"))
		{
			switch (currentShape)
			{
				case PlayerShapes.None:
					EnablePlayer(PlayerShapes.CubePlayer);
					break;
				case PlayerShapes.CubePlayer:
					EnablePlayer(PlayerShapes.SpherePlayer);
					break;
				case PlayerShapes.SpherePlayer:
					EnablePlayer(PlayerShapes.TetrahedralPlayer);
					break;
				case PlayerShapes.TetrahedralPlayer:
					EnablePlayer(PlayerShapes.CubePlayer);
					break;
				default:
					break;
			}
		}
	}

	public static ArmatureManager GetActiveRig()
	{
		return FindObjectOfType<ArmatureManager>();
	}
}
