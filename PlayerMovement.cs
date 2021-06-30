using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

//[System.Serializable]
public class PlayerMovement : MonoBehaviour
{
	//-------------------Enums-------------------------
	public enum Direction
	{
		None,
		Left,
		Right,
		Both
	}

	//---------------Private-Variables-----------------
    private int sign = 1;		//used to decide which way player is facing

    private float moveForce = 84f;          //force applied to the player when moving

	private static readonly float intensity = 400f;	//intensity of the lifeIndicator square
    private float dragX;					//speed at which player slows down when movement keys released
    private float lastJump;                 //time since last jump (used to fix inconsistencies in jump height)
#pragma warning disable IDE0044 // Add readonly modifier
	private float groundTrailDensity = 0.5f;
#pragma warning restore IDE0044 // Add readonly modifier

	private Vector3 cappedVector;			//used later
    private Vector3 targetAngle;			//angle that player will lerp to
    private Vector3 currentAngle;			//current angle of player
    private Vector3 locVel;					//used later
	private Vector3 playerLegsVector;		//used for spherecasting calculations
	private Vector3 wallJumpDir;			//direction player should wall jump

	private Rigidbody tempRb;               //temporary rigidbody to apply maths only to certain forces but not others

	private readonly Color[] lifeColors = {	//colors that lifeIndicator can be
        new Color(255 / intensity, 50  / intensity, 50 / intensity),
        new Color(200 / intensity, 255 / intensity, 50 / intensity),
        new Color(50  / intensity, 255 / intensity, 50 / intensity)
        };

	//			Variables that store whether key has been pressed in last Update(), used in FixedUpdate()
	private bool leftEnabled;
	private bool rightEnabled;
	private bool onWall  = false;
	private bool onLWall = false;

	private bool isWallJumpRunning;
	private bool didWallJumpOnFixedFrame = false;
	private bool doSpawnFloorHex = true;	// Variable to store if script should spawn a hexagon at player feet on trigger enter

	private bool? lWallJumpCache = false;   //create nullable bool to store which wall player was last on
	private bool isGrounded = false;

	//			all dependencies of script - these are the objects it reads from and modifies
	[HideInInspector]
    public Rigidbody rb;
	[HideInInspector]
	public LifeIndicator lifeIndicator;
	[HideInInspector]
	public ScalePlayer scaler;
	[HideInInspector]
	public GameObject arrow;
	[HideInInspector]
	public RectTransform arrowTrans;
	[HideInInspector]
	public CameraFollow cameraFollower;
	[HideInInspector]
	public CameraShaker camShake;
	[HideInInspector]
	public LifeIndTextureManager lifeIndTexture;
	[HideInInspector]
	public HexPadManager hexPads;
	[HideInInspector]
	public CCControl cc;
	[HideInInspector]
	public WallInd wallInd;
	[HideInInspector]
	public Dictionary<PlayerParticles.PlayerParticleType, PlayerParticles> particles = new Dictionary<PlayerParticles.PlayerParticleType, PlayerParticles>();
	[HideInInspector]
	public KeyCache keys;                  // Used for lenient keyboard interaction
	[HideInInspector]
	public CapsuleCollider playerCollider;
	[HideInInspector]
	public ColorScheme colorScheme;

	//----------------------Public-Variables------------------

	//			stores information on how to jump
	[Header("Jumping")]
    [Range(10, 1000)]
    public float jumpForce = 1000f;
	public float wallJumpForce = 50f;
	public float defaultHoverTransitionTime = 5f;
	[HideInInspector]
	public float hoverTransitionTime = 5f;
	[Range(0f, 1f)]
    public float jumpRecharge = 1f;
	public bool doSpawnHexes = true;
	public float swing = 0.01f;

	//			stores information on how to move
	[Space(20)]
    [Header("Movement")]
    [Range(8, 20)]
    public float movementSens = 84f;
	public float arrowSpinSpeed = 200f;
	[Space(10)]
	[Header("Controls")]
	[Range(0f, 1f)]
	public float wallJumpLeniency;
	[Range(0f, 0.5f)]
	public float jumpLeniency;

	[Space(10)]

    [Range(1, 5)]
    public float airMovementLoss = 2f;

    [Space(10)]
    public float maxSpeed = 8f;

	//			stores information on how to turn
    [Space(20)]
    [Header("Turning")]
    public float lerpSpeed = 10f;

    [Range(0, 90)]
    public float turnAngle;

	//			radii and offsets of spheres for Physics.CheckSphere() checks
    [Space(20)]
	[Header("Sensing")]
    [Range(0.01f, 1f)]
    public float groundCheckRad;	// Radius of trigger that fires when player interacts with ground
	public float wallCheckRad;	// Radius of triggers that fire when player interacts with wall
	public float playerLegsRad = 1.05f; // Radius of capsulecast that determines if player is grounded

    [Space(10)]
	public Vector3 wallCheckOff;
	[SerializeField]
	private float wallJumpCollapseHeight = 4f;

	//-----------Member-Functions-----------

	internal void DisableHover()
	{
		//hoverEnabled = false;
		//playerCollider.center = Utils.GetCenterFromTopBound(scaler.playerColliderHeight, scaler.playerColliderHeight);
		//playerCollider.height = scaler.playerColliderHeight;
		hoverTransitionTime = Mathf.Infinity;
	}

	internal void EnableHover()
	{
		//hoverEnabled = true;
		//playerCollider.center = Utils.GetCenterFromTopBound(scaler.playerColliderHeight, 5f);
		//playerCollider.height = 5f;
		hoverTransitionTime = defaultHoverTransitionTime;
	}

	public Vector3 GetPlatPos(Vector3 dir, Vector3? nOffsetVect = null, float offset = -0.01f)
	{
		Vector3 offsetVect;

		if (nOffsetVect == null)
		{
			offsetVect = Vector3.zero;
		}
		else
		{
			offsetVect = (Vector3)nOffsetVect;
		}

		// Normalise dir vector
		dir.Normalize();

		// Create vector pointing to somewhere between feet and head of player
		Vector3 rayPos = Utils.WithAddY(transform.position, scaler.playerHeight * scaler.GetCurrScale());

		// Create and cast a ray pointing in dir looking for layer 12
		Ray ray = new Ray(rayPos, dir);
		Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, 1 << 12);

		// Return position of raycast hit
		return hitInfo.point + (dir * offset) + offsetVect;
	}

	private void SetLWallCache(bool? _isLWall = null)
	{
		lWallJumpCache = _isLWall;
	}

	public bool IsGrounded()
	{
		return isGrounded;
	}

	public void DisableMovement(Direction dir)
	{
		if (dir == Direction.Left || dir == Direction.Both)
		{
			leftEnabled = false;
		}
		if (dir == Direction.Right || dir == Direction.Both)
		{
			rightEnabled = false;
		}
	}

	public void EnableMovement(Direction dir)
	{
		if (dir == Direction.Left || dir == Direction.Both)
		{
			leftEnabled = true;
		}
		if (dir == Direction.Right || dir == Direction.Both)
		{
			rightEnabled = true;
		}
	}

	// Coroutine to wall-jump
	private IEnumerator WallJump(bool isOnLWall)
	{
		isWallJumpRunning = true;
		int jumpSign;

		yield return new WaitForEndOfFrame();   //wait for the end of the frame to start

		float elapsedTime = 0f;

		if (particles[PlayerParticles.PlayerParticleType.WallJump] != null)
		{
			particles[PlayerParticles.PlayerParticleType.WallJump].StartEmittingAt(35f);
		}
		cc.FadeTo(colorScheme.Colors[ColorScheme.SchemeColor.DBlue] * 1.2f, 100f, 1f, 100f, 300f, new float[] { 1f, 1f, 2f, 3f, 2f});
		arrowTrans.rotation = new Quaternion(0f, 0f, 0f, 0f);
		arrow.SetActive(true);					//display arrow of direction
		cameraFollower.SetOffset(Utils.WithAddZ(cameraFollower.defaultCameraOffset, 10f));  //move the camera closer (if the value here is changed, change value for zoom out as well)
		SetLWallCache(isOnLWall);

		if (isOnLWall)	//if player is jumping off left wall
		{
			lifeIndTexture.SwitchTexture(LifeIndTextureManager.LifeIndTextures.LeftWall);
			jumpSign = -1;

			//make player face direction of jump
			targetAngle = new Vector3(0f, -turnAngle, 0f);

			//while the rotation is within the bounds...
			while (arrowTrans.rotation.eulerAngles.z > 270f + (Time.deltaTime * arrowSpinSpeed * (elapsedTime + swing)) || arrowTrans.rotation.eulerAngles.z == 0f)
			{
				//rotate arrow
				arrowTrans.Rotate(Vector3.forward, -Time.deltaTime * arrowSpinSpeed * (elapsedTime + swing));
				//if the space key is released, break out of loop
				if (!keys.WasKeyPressed(GameController.keyMaps[GameController.Controls.Up], 0f))
				{
					break;
				}

				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
		}
		else    //if player is jumping off right wall
		{
			lifeIndTexture.SwitchTexture(LifeIndTextureManager.LifeIndTextures.RightWall);
			jumpSign = 1;
			targetAngle = new Vector3(0f, turnAngle, 0f);

			while (arrowTrans.rotation.eulerAngles.z < 90f - (Time.deltaTime * arrowSpinSpeed * (elapsedTime + swing)))
			{
				arrowTrans.Rotate(Vector3.forward, Time.deltaTime * arrowSpinSpeed * (elapsedTime + swing));

				if (!keys.WasKeyPressed(GameController.keyMaps[GameController.Controls.Up], 0f))
				{
					break;
				}

				elapsedTime += Time.unscaledDeltaTime;

				yield return null;
			}
		}

		//when player releases space button...

		//// Set Collider's height to head height
		//// @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ TODO FIX THIS @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
		//playerCollider.height = wallJumpCollapseHeight;
		//playerCollider.center = Utils.GetCenterFromTopBound(scaler.playerColliderHeight * scaler.GetCurrScale(), wallJumpCollapseHeight);
		//print(wallJumpCollapseHeight);

		// Play Sound-effect
		AudioController.Play("Wall-Jump-Sound");

		//StopAllCoroutines Emitting Particles
		if (particles[PlayerParticles.PlayerParticleType.WallJump] != null)
		{
			particles[PlayerParticles.PlayerParticleType.WallJump].StopEmitting();
		}

		//Shake Camera
		camShake.BeginCameraShake(0.5f * Mathf.Pow(scaler.GetCurrScale(), 2f), 0.5f, 10f);

		//remove drag
		dragX = 0f;

		//Update wallJumpInd
		wallInd.UpdateState();

		//convert angle of arrow to direction of jump, then store direction to then be applied at end of FixedUpdate()
		float endAngle = arrowTrans.eulerAngles.z;
		wallJumpDir = new Vector3(-Mathf.Sin(Mathf.Deg2Rad * endAngle), Mathf.Cos(Mathf.Deg2Rad * endAngle), 0f);

		//tell rest of code that wall jump has occured on frame
		didWallJumpOnFixedFrame = true;

		//return settings to default after releasing space button
		SlowMotion(false);
		arrow.SetActive(false);
		cc.FadeTo(colorScheme.Colors[ColorScheme.SchemeColor.White], 26f, 0.068f, 32f, 55f, new float[] { 8f, 8f, 8f, 8f, 8f });
		cameraFollower.SetOffset(Utils.WithAddZ(cameraFollower.defaultCameraOffset, -10f));  //move the camera closer (if the value here is changed, change value for zoom out as well)

		yield return new WaitForSeconds(0.1f);
		targetAngle = new Vector3(0f, jumpSign * turnAngle, 0f);

		isWallJumpRunning = false;

		// Check if player has launched self into ground
	}

	//Function to enter/exit slowmo
	private void SlowMotion(bool doSlow)
	{
		if (doSlow)
		{
			Time.timeScale = 0.1f;	//set time scale to 10%
		} else
		{
			Time.timeScale = 1f;	//set time scale to 100%
		}
		Time.fixedDeltaTime = Time.timeScale * 0.02f;	//adjust fixedDeltaTime to time scale to keep framerate consistent
	}

	//Function to normalise a Vector1/float to either -1 or 1
	private float Normalise(float x)
    {
        if (x >= 0)
        {
            return 1;
        } else
        {
            return -1;
        }
    }

	//Function for other scripts to call to say that the player is on a wall or not
	public void SetIsOnWall(string wall = null)
	{
		if (!string.IsNullOrEmpty(wall))	//if an argument was passed in...
		{
			onWall = true;                  //tell rest of code that player is on wall

			if (doSpawnHexes)
			{
				if (wall == "LWallTrigger")     //tell rest of code what wall player is on
				{
					onLWall = true;
					hexPads.SpawnHex(GetPlatPos(Vector3.left, Vector3.down * scaler.GetCurrScale() * scaler.playerHeight * 0.09f), HexPadManager.Orientation.right);
				}
				else
				{
					hexPads.SpawnHex(GetPlatPos(Vector3.right, Vector3.down * scaler.GetCurrScale() * scaler.playerHeight * 0.09f), HexPadManager.Orientation.left);
				}
			}
			else
			{
				if (wall == "LWallTrigger")
				{
					onLWall = true;
				}
			}
		} else
		{
			onWall = false;		//If no argument was passed, tell rest of code that player is not on wall
			onLWall = false;
		}
	}

	// Checks if current wall state is different to previous wall state via nullable XOR (see truth table in Utils.cs)
	public bool CanWallJump()
	{
		return Utils.NullableXor(lWallJumpCache, onLWall) && onWall && (scaler.GetCurrScale() <= 1f);
	}

	Vector3 GetHeadPos()
	{
		return Utils.WithAddY(transform.position, scaler.playerHeight * scaler.GetCurrScale());
	}


	//--------Main-Code--------

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = Color.green;
			Physics.SphereCast(GetHeadPos(), playerLegsRad, Vector3.down, out RaycastHit hitInfo, scaler.playerHeight * scaler.GetCurrScale(), 1 << 12);
			Gizmos.DrawSphere(hitInfo.point + playerLegsVector, playerLegsRad);
			Gizmos.color = Color.white;
			Gizmos.DrawSphere(GetHeadPos() - playerLegsVector, playerLegsRad);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.parent.CompareTag("ElevatorPlatform"))
		{
			transform.parent.SetParent(other.transform);
		}
	}

	private void OnTriggerExit(Collider other)	//Executed when this or any child components exits a collision
	{
		if (other.gameObject.layer == 12 && !Physics.GetIgnoreCollision(playerCollider, other))
		{
			doSpawnFloorHex = true;
		}
		if (other.transform.parent.CompareTag("ElevatorPlatform"))
		{
			transform.parent.SetParent(null);
		}
	}

	private void Awake()    //Called when game starts
	{
		// Get dependencies

		PlayerParticles[] _playerParticles = Utils.GetComponentsInSibling<PlayerParticles>(gameObject);

		particles.Clear();
		foreach (PlayerParticles.PlayerParticleType type in Enum.GetValues(typeof(PlayerParticles.PlayerParticleType)))
		{
			particles.Add(type, Array.Find(_playerParticles, e => e.particleType == type));
		}

		colorScheme = FindObjectOfType<ColorScheme>();

		keys = FindObjectOfType<KeyCache>();

		cc = FindObjectOfType<CCControl>();

		playerCollider = GetComponent<CapsuleCollider>();

		scaler = GetComponent<ScalePlayer>();

		rb = GetComponent<Rigidbody>();

		lifeIndicator = Utils.GetComponentInSiblings<LifeIndicator>(gameObject);
		lifeIndTexture = GetComponent<LifeIndTextureManager>();

		cameraFollower = FindObjectOfType<CameraFollow>();
		camShake = FindObjectOfType<CameraShaker>();

		lifeIndTexture = Utils.GetComponentInSiblings<LifeIndTextureManager>(gameObject);

		hexPads = Utils.GetComponentInSiblings<HexPadManager>(gameObject);

		wallInd = Utils.GetComponentInSiblings<WallInd>(gameObject);

		arrow = GameObject.FindGameObjectWithTag("Arrow");
		arrowTrans = GameObject.FindGameObjectWithTag("ArrowTrans").GetComponent<RectTransform>();

		// Initialise members
		currentAngle = transform.eulerAngles;	//Set current angle
		arrow.SetActive(false);                 //Hide arrow of direction

		playerCollider.height = wallJumpCollapseHeight;
		playerCollider.center = Utils.GetCenterFromTopBound(scaler.playerColliderHeight, wallJumpCollapseHeight);

		leftEnabled = true;
		rightEnabled = true;

		hoverTransitionTime = defaultHoverTransitionTime;

		SlowMotion(false);

		playerLegsVector = Utils.FromY(playerLegsRad);
	}

	//-----------Physics-Code-------------

	void FixedUpdate()	//Called at fixed framerate of physX engine (default 50FPS)
    {
		tempRb = rb;    //Store rigidbody in temp variable to perform calculations on, seperate to other stuff

		//default to facing forwards
		if (transform.rotation != Quaternion.Euler(0f, 0f, 0f) && moveForce == movementSens)
		{
			targetAngle = Vector3.zero;
		}

		//Wall jump
		if (keys.WasKeyDown(GameController.keyMaps[GameController.Controls.Up], wallJumpLeniency) && keys.WasKeyPressed(GameController.keyMaps[GameController.Controls.Up], 0f) && onWall && !isWallJumpRunning)    //If player is on wall and wall key (default space) is pressed...
		{
			if (CanWallJump())
			{
				SlowMotion(true);
				StartCoroutine(WallJump(onLWall));
			} else
			{
				lifeIndicator.UnAbleFlash();
			}
		}

		//Jump Code
        if (!onWall && Time.fixedTime > lastJump + jumpRecharge && keys.WasKeyPressed(GameController.keyMaps[GameController.Controls.Up], jumpLeniency))	//If player isn't on wall and hasn't jumped within jumpRecharge seconds and space key pressed...
        {
			if (isGrounded)	//If player is touching platform with feet...
            {
				tempRb.velocity = new Vector3(tempRb.velocity.x, 0, tempRb.velocity.z);	//remove y velocity
				tempRb.AddForce(tempRb.velocity.x, jumpForce * scaler.GetCurrScale(), 0f, ForceMode.Force);     //add jumping force
				AudioController.Play("Jump-Sound");

				lastJump = Time.fixedTime;                                              //set last jump to current time
			}
        }

		//Sideways Movement Code
        if (keys.WasKeyPressed(Utils.Join(GameController.keyMaps[GameController.Controls.Left], GameController.keyMaps[GameController.Controls.Right])) && (leftEnabled || rightEnabled))	//If any direction keys are pressed...
        {
			sign = 0;
            //Check if player has pressed keys
            if (keys.WasKeyPressed(GameController.keyMaps[GameController.Controls.Left]) && leftEnabled) {
                targetAngle = new Vector3(0f,  turnAngle, 0f);	//set angle of player based on direction
				sign = -1;										//Set direction of movement (used soon)
            }
            if (keys.WasKeyPressed(GameController.keyMaps[GameController.Controls.Right]) && rightEnabled) {
                targetAngle = new Vector3(0f, -turnAngle, 0f);	//Same as before but flipped
				sign =  1;
            }
			if ((sign == 1) ? rightEnabled : leftEnabled) {

				if (isGrounded)
				{
					tempRb.AddForce(Vector3.right.x * sign * Mathf.Pow(scaler.GetCurrScale(), 0.33f) * moveForce * Time.deltaTime, 0f, 0f, ForceMode.Impulse); //add impulse in direction of key press by moveForce
				}
				else    //If player is in air...
				{
					tempRb.AddForce(Vector3.right * 700 * sign * Mathf.Pow(scaler.GetCurrScale(), 0.33f) * Time.deltaTime); //add small force in direciton of key press
				}
			}

        }

		//Sideways Velocity Limiting Code
		if (Mathf.Abs(tempRb.velocity.x) > maxSpeed * scaler.GetCurrScale())	//If player is moving over speed limit
        {
            cappedVector.Set(Normalise(tempRb.velocity.x) * maxSpeed * scaler.GetCurrScale(), tempRb.velocity.y, tempRb.velocity.z);	//Set capped vector to same as old vector but with normalised x-speed
			tempRb.velocity = cappedVector;	//Set velocity to capped vector
        }

        //Simulate drag but only on the x axis
        locVel = transform.InverseTransformDirection(tempRb.velocity);	//Create opposing velocity to current velocity
        locVel.x *= 1.0f - dragX;										//Reduce opposing velocity by 1 - dragX
		tempRb.velocity = transform.TransformDirection(locVel);			//Add drag to velocity

        //Lerp angle of player along y axis
        currentAngle = new Vector3(
             Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime * lerpSpeed),	//Lerps between current angle an target angle by deltaTime and lerpSpeed
             Mathf.LerpAngle(currentAngle.y, targetAngle.y, Time.deltaTime * lerpSpeed),
             Mathf.LerpAngle(currentAngle.z, targetAngle.z, Time.deltaTime * lerpSpeed));
		tempRb.MoveRotation(Quaternion.Euler(currentAngle));    //Set rotation to Lerped angle



		rb = tempRb;    //Set rigidbody to temp rigidbody we've been using

		//All following code is not velocity capped or adjusted by drag
		if (didWallJumpOnFixedFrame)	//if player wall jumped this frame...
		{
			// Remove Pre-existing velocity
			tempRb.velocity = Vector3.zero;

			rb.AddForce(wallJumpDir * wallJumpForce * scaler.GetCurrScale(), ForceMode.Impulse);	//Apply force in direction calculated in WallJump()
			didWallJumpOnFixedFrame = false;								//Wait for nect time player wall jumps to do again
		}

		// Hover physics + ground checking (so glad this is finally working)
		if (Physics.SphereCast(GetHeadPos() - playerLegsVector * scaler.GetCurrScale(), playerLegsRad * scaler.GetCurrScale(), Vector3.down, out RaycastHit hitInfo, (scaler.playerColliderHeight * scaler.GetCurrScale()) - (playerLegsRad * scaler.GetCurrScale() * 2), 1 << 12)) // If player is on ground...
		{
			// Dont use gravity while hovering over ground
			if (rb.useGravity)
			{
				rb.useGravity = false;
			}

			// Cancel rb's velocity instantly by adding opposing velocity change (rb.velocity.scale(Vector3.right) takes a while to update)
			rb.AddForce(0f, -rb.velocity.y, 0f, ForceMode.VelocityChange);

			// Smoothly move from current elevation to target elevation calculated by raycast
			rb.MovePosition(Vector3.Lerp(transform.position, Utils.WithAddY(hitInfo.point, -0.01f), Time.deltaTime * hoverTransitionTime));

			// Activate ground-impact burst
			if (particles[PlayerParticles.PlayerParticleType.GroundParticles] != null)
			{
				particles[PlayerParticles.PlayerParticleType.GroundParticles].StartEmittingAt(Mathf.Abs(rb.velocity.x) * groundTrailDensity * (1f / scaler.GetCurrScale()));//EmitBatch(5);
			}
			// If player hasn't already been grounded, do so by...
			if (!isGrounded)
			{
				isGrounded = true;			// Setting grounded state to true
				dragX = 0.2f;				// Make x-drag high
				moveForce = movementSens;	// Give player more traction (makes controls more sensitive)
				lifeIndTexture.SwitchTexture(LifeIndTextureManager.LifeIndTextures.Floor);	// Set Indicator texture to indicate that player is on floor
				SetLWallCache();			// Reset wall cache state so that player can wall jump on left or right walls
				wallInd.UpdateState();      // Update wall indicators

				// Possibly spawn hex pads on floor
				if (doSpawnFloorHex && doSpawnHexes)
				{
					hexPads.SpawnHex(GetPlatPos(Vector3.down), HexPadManager.Orientation.up);
					doSpawnFloorHex = false;
				}
				playerCollider.center = Utils.GetCenterFromTopBound(scaler.playerColliderHeight, 5f);
				playerCollider.height = 5f;
			}
		}
		else
		{
			// If in air, use gravity
			if (!rb.useGravity)
			{
				rb.useGravity = true;
			}

			// If player grounded state hasn't been updated...
			if (isGrounded)
			{
				isGrounded = false;	// Set grounded state to false
				dragX = 0.01f;		// Reduce x-drag
				moveForce = movementSens / airMovementLoss; // Reduce player traction (make controls less sensitive)
				// Stop emitting ground particles
				if (particles[PlayerParticles.PlayerParticleType.GroundParticles] != null)
				{
					particles[PlayerParticles.PlayerParticleType.GroundParticles].StopEmitting();
				}
				
				playerCollider.center = Utils.GetCenterFromTopBound(scaler.playerColliderHeight, 2.5f);
				playerCollider.height = 2.5f;
			}
		}
	}
}
