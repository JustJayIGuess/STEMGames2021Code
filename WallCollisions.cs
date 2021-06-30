using UnityEngine;

public class WallCollisions : MonoBehaviour
{
	public PlayerMovement player;
	public ScalePlayer scaler;
	public enum WallColliders
	{
		Left,
		Right
	}

	public WallColliders wallColliderType;

	private void Awake()
	{
		if (CompareTag("LWallTrigger"))
		{
			wallColliderType = WallColliders.Left;
		}
		else if (CompareTag("RWallTrigger"))
		{
			wallColliderType = WallColliders.Right;
		}
		else
		{
			throw new System.Exception("Wall Collider was not tagged with LWallTrigger or RWallTrigger!");
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Platform"))
		{
			WallJumpInfo wallJumpInfo = other.GetComponent<WallJumpInfo>();
			if (wallJumpInfo != null)
			{
				if (other.GetComponent<WallJumpInfo>().CheckIfAllowable(wallColliderType))
				{
					player.SetIsOnWall(tag);
				}
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		player.SetIsOnWall();
	}

	// TODO: make wall colliders scale better with scaler via subscription ScalePlayer.OnScale += func
}
