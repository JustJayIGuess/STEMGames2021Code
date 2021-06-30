using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlayerFollow : MonoBehaviour
{
	public Transform player;
	public ScalePlayer scaler;
	public Vector3 offset;
	public float speed = 1f;

	[SerializeField]
	private Rigidbody rb;
	private readonly Collider[] colliders = new Collider[2];

	private void Start()
	{
		rb = GetComponent<Rigidbody>();

		colliders[0] = player.GetComponent<Collider>();
		colliders[1] = GetComponent<Collider>();

		Physics.IgnoreCollision(colliders[0], colliders[1]);
	}

	private void FixedUpdate()
	{
		rb.MovePosition(Vector3.Lerp(transform.position, player.position + offset * scaler.GetCurrScale(), Time.deltaTime * speed));
	}
}
