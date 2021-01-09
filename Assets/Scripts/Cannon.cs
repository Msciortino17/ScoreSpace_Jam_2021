using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
	private Camera mainCamera;
	public GameManager MyGameManager;
	[SerializeField] Transform textSpawnLocation;
	[SerializeField] ParticleSystem targetLocationCursor;
	private bool shooting;
	private Vector3 targetLocation;

	public LineRenderer TargettingLine;
	public float TargettingLineProgress;
	public float TargettingLineProgressAmount;
	public string[] TargettingLineResults;

	[Header("Settings")]
	private float coolDownTime;
	private float coolDownTimer;

	[Header("Prefabs")]
	[SerializeField] private GameObject bombPrefab;
	[SerializeField] private GameObject targetNodePrefab;
	[SerializeField] private GameObject particleTextPrefab;

	// Start is called before the first frame update
	void Start()
	{
		mainCamera = Camera.main;
		MyGameManager = transform.parent.GetComponent<GameManager>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateInput();
		UpdateTimers();
	}

	/// <summary>
	/// Checks for mouse input to begin the bomb teleportation process.
	/// </summary>
	private void UpdateInput()
	{
		Vector3 mousePos = GetMousePos();
		// Place bombs
		if (Input.GetKeyDown(KeyCode.Mouse0) && coolDownTimer <= 0f && !shooting)
		{
			coolDownTimer = coolDownTime;
			shooting = true;
			targetLocation = mousePos;
			targetLocationCursor.transform.position = targetLocation;
			targetLocationCursor.Play();
			MyGameManager.SlowDownTime();
			BeginTargetting();
		}

		// Look at the mouse
		if (mousePos.magnitude > 1f)
		{
			Vector3 toMouse = (mousePos - transform.position).normalized;
			transform.rotation = XLookRotation(toMouse, Vector3.up);
		}
	}

	/// <summary>
	/// Updates the timers.
	/// </summary>
	private void UpdateTimers()
	{
		if (coolDownTimer > 0f)
		{
			coolDownTimer -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Spawns the given bomb type at the given position.
	/// </summary>
	private Bomb SpawnBomb(GameObject _bomb, Vector3 _position)
	{
		Bomb bomb = Instantiate(_bomb).GetComponent<Bomb>();
		bomb.transform.position = _position;
		bomb.gameManager = MyGameManager;
		return bomb;
	}

	/// <summary>
	/// Spawns and configures a chain of target nodes towards the target position.
	/// </summary>
	public void BeginTargetting()
	{

		List<Vector3> linePositions = new List<Vector3>();
		Vector3 toTarget = targetLocation - transform.position;
		float distance = toTarget.magnitude;
		toTarget.Normalize();
		Vector3 toTargetPerp = Vector2.Perpendicular(toTarget).normalized;

		TargetNode firstNode = null;
		TargetNode lastNode = null;
		int numNodes = 10;
		for (int i = 0; i < numNodes; i++)
		{
			// Standard node init
			TargetNode node = Instantiate(targetNodePrefab, MyGameManager.TargettingNodes).GetComponent<TargetNode>(); // should be batching this buuut...
			node.MyCannon = this;
			if (lastNode != null)
			{
				lastNode.NextNode = node;
				node.gameObject.SetActive(false);
			}
			if (firstNode == null)
			{
				firstNode = node;
			}
			lastNode = node;

			// Position along a line straight back to cannon
			float distanceRatio = 1f - ((float)i / (numNodes - 1));
			Vector3 position = new Vector3(toTarget.x * distance * distanceRatio, toTarget.y * distance * distanceRatio);

			// Add a simple sin wave
			float waveDistance = 1f;
			float angle = distanceRatio * 360f * Mathf.Deg2Rad;
			position += new Vector3(toTargetPerp.x * Mathf.Sin(angle) * waveDistance, toTargetPerp.y * Mathf.Sin(angle) * waveDistance, 0f);

			node.transform.position = position;
			node.ProgressOnLink = distanceRatio;
			node.TimeToSelect = 1f * (distanceRatio + 0.2f);
			linePositions.Add(position);
		}

		firstNode.transform.position = targetLocation;
		lastNode.transform.position = transform.position;

		TargettingLine.positionCount = numNodes;
		TargettingLine.SetPositions(linePositions.ToArray());
		TargettingLine.gameObject.SetActive(true);

		SetTargettingLineProgress(0f);
		TargettingLineProgressAmount = 1f / numNodes; 
	}

	/// <summary>
	/// Moves the bulge of the line along as the player makes progress tracing along it.
	/// This is done by making the current area wider and not transparent.
	/// </summary>
	public void SetTargettingLineProgress(float _middleTime)
	{
		float adjustedTime = _middleTime;
		if (adjustedTime < 0.2f)
		{
			adjustedTime = 0.2f;
		}
		if (adjustedTime > 0.8f)
		{
			adjustedTime = 0.8f;
		}

		Gradient gradient = TargettingLine.colorGradient;
		GradientAlphaKey[] alphaKeys = gradient.alphaKeys;
		alphaKeys[1].time = adjustedTime - 0.2f;
		alphaKeys[2].time = adjustedTime - 0.1f;
		alphaKeys[3].time = _middleTime;
		alphaKeys[4].time = adjustedTime + 0.1f;
		alphaKeys[5].time = adjustedTime + 0.2f;
		gradient.SetKeys(gradient.colorKeys, alphaKeys);
		TargettingLine.colorGradient = gradient;

		AnimationCurve widthCurve = TargettingLine.widthCurve;
		Keyframe[] keys = widthCurve.keys;
		keys[1].time = adjustedTime - 0.15f;
		keys[2].time = adjustedTime;
		keys[3].time = adjustedTime + 0.15f;
		widthCurve.keys = keys;
		TargettingLine.widthCurve = widthCurve;

		TargettingLineProgress = _middleTime;
	}

	/// <summary>
	/// Callback for if the player doesn't get the target node in time
	/// </summary>
	public void FailedTarget(float _distance)
	{
		MyGameManager.NormalizeTime();
		Bomb bomb = SpawnBomb(bombPrefab, targetLocation);
		float offSetRange = 6f * _distance;
		float x = Random.Range(-offSetRange, offSetRange);
		float y = Random.Range(-offSetRange, offSetRange);
		bomb.transform.Translate(x, y, 0f);
		shooting = false;

		foreach (Transform child in MyGameManager.TargettingNodes)
		{
			Destroy(child.gameObject);
		}

		TargettingLine.gameObject.SetActive(false);

		MyGameManager.IncreaseScore((int)((1f - _distance) * 10f));
		int resultText = 2;
		if (_distance > 0.8f)
		{
			resultText = 0;
		}
		else if (_distance > 0.5f)
		{
			resultText = 1;
		}
		ParticleText text = Instantiate(particleTextPrefab, textSpawnLocation.position, Quaternion.identity).GetComponent<ParticleText>();
		text.SetText(TargettingLineResults[resultText]);
		targetLocationCursor.Stop();
	}

	/// <summary>
	/// Callback for when the player selects the final node
	/// </summary>
	public void SuccessfulTarget()
	{
		SpawnBomb(bombPrefab, targetLocation);
		shooting = false;

		foreach (Transform child in MyGameManager.TargettingNodes)
		{
			Destroy(child.gameObject);
		}

		TargettingLine.gameObject.SetActive(false);

		MyGameManager.IncreaseScore(25);
		ParticleText text = Instantiate(particleTextPrefab, textSpawnLocation.position, Quaternion.identity).GetComponent<ParticleText>();
		text.SetText(TargettingLineResults[3]);
		targetLocationCursor.Stop();
	}

	/// <summary>
	/// The world space position of the mouse
	/// </summary>
	public Vector3 GetMousePos()
	{
		Vector2 mouse = Input.mousePosition;
		Vector3 point = mainCamera.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, 0f)); //mainCamera.nearClipPlane));
		point.z = 0f;
		return point;
	}

	/// <summary>
	/// Returns a random position within the given bounds.
	/// </summary>
	public Vector3 RandomPosition(float _xDistance, float _yDistance)
	{
		float x = Random.Range(-_xDistance, _xDistance);
		float y = Random.Range(-_yDistance, _yDistance);
		return new Vector3(x, y, 0f);
	}

	/// <summary>
	/// Thanks! https://gamedev.stackexchange.com/questions/139515/lookrotation-make-x-axis-face-the-target-instead-of-z
	/// </summary>
	Quaternion XLookRotation(Vector3 right, Vector3 up)
	{
		Quaternion rightToForward = Quaternion.Euler(0f, -90f, 0f);
		Quaternion forwardToTarget = Quaternion.LookRotation(right, up);

		return forwardToTarget * rightToForward;
	}
}
