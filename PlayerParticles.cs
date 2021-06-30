using System;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
	public enum PlayerParticleType
	{
		WallJump,
		GroundParticles
	}
	public PlayerParticleType particleType;
	public ColorScheme.SchemeColor startColor;
	public ColorScheme.SchemeColor trailColor;

	private ParticleSystem particles;
	private ParticleSystem.MainModule particleProperties;
	private ParticleSystemRenderer particleRenderer;
	private PlayerMovement player;
	private readonly Vector3? offsetVect = new Vector3(0f, 0.1f, 0f);

	private void Awake()
	{
		particles = GetComponent<ParticleSystem>();
		particleRenderer = GetComponent<ParticleSystemRenderer>();
		particleProperties = particles.main;

		player = FindObjectOfType<PlayerMovement>();
	}

	//private void OnValidate()
	//{
	//	particles = GetComponent<ParticleSystem>();
	//	particleRenderer = GetComponent<ParticleSystemRenderer>();
	//	particleProperties = particles.main;

	//	player = FindObjectOfType<PlayerMovement>();
	//	ColorScheme colorScheme = FindObjectOfType<ColorScheme>();
	//	particleProperties.startColor = colorScheme.Colors[startColor];
	//	particleRenderer.trailMaterial.SetColor("_EmissionColor", colorScheme.Colors[trailColor]);
	//}

	private void Start()
    {
		particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);

		ColorScheme colorScheme = FindObjectOfType<ColorScheme>();
		particleProperties.startColor = colorScheme.Colors[startColor];
		particleRenderer.trailMaterial.SetColor("_EmissionColor", colorScheme.Colors[trailColor]);
	}

	private void OnEnable()
	{
		ScalePlayer.OnScale += OnScale;
	}

	private void OnDisable()
	{
		ScalePlayer.OnScale -= OnScale;
	}

	private void OnScale(float scale)
	{
		transform.localScale = scale * Vector3.one;
	}

	public void EmitBatch(int size)
	{
		particles.Emit(size);
	}

	public void StartEmittingAt(float rate)
	{
		if (particleType == PlayerParticleType.GroundParticles)
		{
			transform.position = player.GetPlatPos(Vector3.down, offsetVect);
		}

		StopEmitting();

		ParticleSystem.ShapeModule shape = particles.shape;
		shape.rotation = transform.InverseTransformDirection(shape.rotation);

		ParticleSystem.EmissionModule emission = particles.emission;
		emission.rateOverTime = rate;
		emission.enabled = true;
		particles.Play();
	}

	public void StopEmitting()
	{
		particles.Stop(false, ParticleSystemStopBehavior.StopEmitting);
	}
}
