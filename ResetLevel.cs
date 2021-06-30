using UnityEngine;

[ExecuteInEditMode]
public class ResetLevel : MonoBehaviour
{
#if UNITY_EDITOR
	public bool resetWallJumpInfo = false;
	public WallJumpInfo.WallJumpAllowableSides defaultWallJumpAllowance = WallJumpInfo.WallJumpAllowableSides.Everything;

	public Material assignMaterial;
	public PhysicMaterial assignPhysicsMaterial;

	// Changing tags and layers in OnValidate throws an annoying warning and can cause weird stuff, so I've added it to this delegate which is called after everything has been updated in the inspector, so its basically the same as OnValidate by itself
	private void OnValidate()
	{
		UnityEditor.EditorApplication.delayCall += OnValidateSafe;
	}

	private void OnValidateSafe()
	{
		if (this == null) return; // Avoid calling if object is null because otherwise weird stuff happens

		gameObject.tag = "Platform";
		gameObject.layer = 12;

		if (!Application.isPlaying)
		{
			foreach (MeshRenderer r in (Renderer[])GetComponentsInChildren<MeshRenderer>())
			{
				r.material = new Material(assignMaterial);
				r.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
				r.gameObject.tag = "Platform";
				r.gameObject.layer = 12;

				if (resetWallJumpInfo)
				{
					WallJumpInfo info = r.gameObject.GetComponent<WallJumpInfo>();
					if (info == null)
					{
						info = r.gameObject.AddComponent<WallJumpInfo>();
					}
					info.allowWallJumpOnSides = defaultWallJumpAllowance;
				}

				MeshCollider collider = r.gameObject.GetComponent<MeshCollider>();

				if (collider == null)
				{
					collider = r.gameObject.AddComponent<MeshCollider>();
				}
				collider.material = assignPhysicsMaterial;
				collider.convex = true;
			}
		}
    }
#endif
	
}
