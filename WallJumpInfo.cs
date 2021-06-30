using System;

[Serializable]
public class WallJumpInfo : UnityEngine.MonoBehaviour
{
	[Flags]
	public enum WallJumpAllowableSides
	{
		None = 0,
		Left = 1 << 0,
		Right = 1 << 1,
		Everything = ~0
	}

	public WallJumpAllowableSides allowWallJumpOnSides;

	public bool CheckIfAllowable(WallCollisions.WallColliders colliderType)
	{
		if (allowWallJumpOnSides.HasFlag(colliderType == WallCollisions.WallColliders.Left ? WallJumpAllowableSides.Right : WallJumpAllowableSides.Left))
		{
			return true;
		}
		return false;
	}
}
