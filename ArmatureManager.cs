using UnityEngine;

[ExecuteInEditMode]
public class ArmatureManager : MonoBehaviour
{
	public PhysicMaterial mat;
	private Quaternion[] jointRotDefaults;
	private Vector3[,] anchors;
	private CharacterJoint[] joints;

	public void UpdateArmatures()
	{
		for (int i = 0; i < joints.Length; i++)
		{
			joints[i].anchor = anchors[i, 0];
			joints[i].connectedAnchor = anchors[i, 1];
		}
	}

	public void ResetArmatures()
	{
		// If default armatures have been set, set all armatures to default values
		if (jointRotDefaults != null)
		{
			for (int i = 0; i < jointRotDefaults.Length; i++)
			{
				joints[i].transform.localRotation = jointRotDefaults[i];
				joints[i].anchor = anchors[i, 0];
				joints[i].connectedAnchor = anchors[i, 1];
			}
		}
	}

	private void OnEnable()
	{
		if (joints == null)
		{
			Collider[] clds = GetComponentsInChildren<Collider>();
			foreach (Collider cld in clds)
			{
				cld.material = mat;
			}

			joints = GetComponentsInChildren<CharacterJoint>();
			foreach (CharacterJoint joint in joints)
			{
				joint.enablePreprocessing = false;
				joint.connectedMassScale = 1f;
				joint.enableProjection = true;
				joint.enableCollision = true;

				SoftJointLimitSpring spring = joint.swingLimitSpring;

				spring.spring = 0f;
				spring.damper = 0f;
				joint.swingLimitSpring = spring;
				joint.autoConfigureConnectedAnchor = false;
			}

			jointRotDefaults = new Quaternion[joints.Length];
			anchors = new Vector3[joints.Length, 2];
			for (int i = 0; i < joints.Length; i++)
			{
				anchors[i, 0] = joints[i].anchor;
				anchors[i, 1] = joints[i].connectedAnchor;

				jointRotDefaults[i] = joints[i].transform.localRotation;
			}


			Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody rb in rbs)
			{
				if (rb.collisionDetectionMode != CollisionDetectionMode.ContinuousSpeculative)
				{
					rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
				}

				rb.drag = 5f;
				rb.angularDrag = 5f;
				rb.solverIterations = 15;
				rb.interpolation = RigidbodyInterpolation.None;
				rb.maxDepenetrationVelocity = 7f;
			}
		}
	}
}
