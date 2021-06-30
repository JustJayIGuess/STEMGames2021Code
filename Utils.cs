/*
 * Coded by Jay Johnston
 * This is my utility class, where I put all of the small utility functions that dont fit anywhere else or are used in multiple classes.
 */

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
	public static Vector3 WithXVal(Vector3 vect, float newX)
	{
		return new Vector3(newX, vect.y, vect.z);
	}

	public static Vector3 WithYVal(Vector3 vect, float newY)
	{
		return new Vector3(vect.x, newY, vect.z);
	}

	public static Vector3 WithZVal(Vector3 vect, float newZ)
	{
		return new Vector3(vect.x, vect.y, newZ);
	}

	public static Vector3 WithAddX(Vector3 vect, float addX)
	{
		return new Vector3(vect.x + addX, vect.y, vect.z);
	}

	public static Vector3 WithAddY(Vector3 vect, float addY)
	{
		return new Vector3(vect.x, vect.y + addY, vect.z);
	}

	public static Vector3 WithAddZ(Vector3 vect, float addZ)
	{
		return new Vector3(vect.x, vect.y, vect.z + addZ);
	}

	public static Vector3 WithAddXY(Vector3 vect, float addX, float addY)
	{
		return new Vector3(vect.x + addX, vect.y + addY, vect.z);
	}

	public static Vector3 WithAddYZ(Vector3 vect, float addY, float addZ)
	{
		return new Vector3(vect.x, vect.y + addY, vect.z + addZ);
	}

	public static Vector3 WithAddXZ(Vector3 vect, float addX, float addZ)
	{
		return new Vector3(vect.x + addX, vect.y, vect.z + addZ);
	}

	public static Vector3 WithAddXYZ(Vector3 vect, float addX, float addY, float addZ)
	{
		return new Vector3(vect.x + addX, vect.y + addY, vect.z + addZ);
	}

	public static Vector3 FromX(float x)
	{
		return new Vector3(x, 0f, 0f);
	}

	public static Vector3 FromY(float y)
	{
		return new Vector3(0f, y, 0f);
	}

	public static Vector3 FromZ(float z)
	{
		return new Vector3(0f, 0f, z);
	}

	internal static Color WithRGB(Color original, Color rgb)
	{
		return new Color(rgb.r, rgb.g, rgb.b, original.a);
	}

	public static Vector3 WithOnlyX(Vector3 vect)
	{
		return new Vector3(vect.x, 0f, 0f);
	}

	public static Vector3 WithOnlyY(Vector3 vect)
	{
		return new Vector3(0f, vect.y, 0f);
	}

	public static Vector3 WithOnlyZ(Vector3 vect)
	{
		return new Vector3(0f, 0f, vect.z);
	}

	public static Vector3 WithOutX(Vector3 vect)
	{
		return new Vector3(0f, vect.y, vect.z);
	}

	public static Vector3 WithOutY(Vector3 vect)
	{
		return new Vector3(vect.x, 0f, vect.z);
	}

	public static Vector3 WithOutZ(Vector3 vect)
	{
		return new Vector3(vect.x, vect.y, 0f);
	}

	public static int BoolToInt(bool b)
	{
		return b ? 1 : 0;
	}

	public static float[] Vector3ToFloatArr(Vector3 vect)
	{
		float[] arr = new float[3];
		arr[0] = vect.x;
		arr[1] = vect.y;
		arr[2] = vect.z;
		return arr;
	}

	public static Vector3 FloatArrToVector3(float[] arr)
	{
		return new Vector3(arr[0], arr[1], arr[2]);
	}

	public static bool IntToBool(int i)
	{
		return i != 0;
	}

	/*Utils.NullableXor(bool? nullable, bool nonNullable) follows the following truth table:
	 *		 _______________________________________________________
	 *		|	bool? a		|		bool b		|		Output		|
	 *		|_______________|___________________|___________________|
	 *		|	null		|		false		|		true		|
	 *		|	null		|		true		|		true		|
	 *		|	false		|		false		|		false		|
	 *		|	false		|		true		|		true		|
	 *		|	true		|		false		|		true		|
	 *		|	true		|		true		|		false		|
	 *		|_______________|___________________|___________________|
	 */

	public static bool NullableXor(bool? nullable, bool nonNullable)
	{
		return (nullable == true && nonNullable == false) || (nullable == false && nonNullable == true) || nullable == null;
	}

	/*Utils.NullableOr(bool? nullable, bool nonNullable) follows the following truth table:
	 *		 _______________________________________________________
	 *		|	bool? a		|		bool b		|		Output		|
	 *		|_______________|___________________|___________________|
	 *		|	null		|		false		|		false		|
	 *		|	null		|		true		|		true		|
	 *		|	false		|		false		|		false		|
	 *		|	false		|		true		|		true		|
	 *		|	true		|		false		|		true		|
	 *		|	true		|		true		|		true		|
	 *		|_______________|___________________|___________________|
	 */

	public static bool NullableOr(bool? nullable, bool nonNullable)
	{
		if (nullable == null)
		{
			return nonNullable;
		} else
		{
			return (bool)nullable;
		}
	}

	public static Color WithAlpha(Color col, float alpha)
	{
		return new Color(col.r, col.g, col.b, alpha);
	}

	public static T GetComponentInSiblings<T>(GameObject gameObject) where T : Component
	{
		return gameObject.transform.parent.gameObject.GetComponentInChildren<T>();
	}

	public static T[] GetComponentsInSibling<T>(GameObject gameObject) where T : Component
	{
		return gameObject.transform.parent.gameObject.GetComponentsInChildren<T>();
	}

	public static float AverageColorDifference(Color col1, Color col2)
	{
		float sum = 0f;

		for (int i = 0; i < 4; i++)
		{
			sum += Mathf.Abs(col1[i] - col2[i]);
		}

		return sum / 4;
	}

	public static Vector3 GetCenterFromTopBound(float constHeight, float newHeight)
	{
		return FromY(constHeight - (newHeight / 2f));
	}

	public static List<T> Join<T> (List<T> list1, List<T> list2)
	{
		if (list1 == null)
		{
			return list2;
		}
		if (list2 == null)
		{
			return list1;
		}

		return list1.Concat(list2).ToList();
	}
}
