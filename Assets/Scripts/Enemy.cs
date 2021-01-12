using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	[SerializeField] private ParticleSystem damagedParticles;
	[SerializeField] private Color yellowColor;
	[SerializeField] private Color orangeColor;
	private SpriteRenderer mySprite;
	[SerializeField] private int scoreGranted;
	public GameManager MyGameManager;

	private float startHealth;
	public float Health;
	public float Damage;
	public Text HealthText;

	[Header("Movement Settings")]
	private float currentRange;
	[SerializeField] private float farOrbitRange;
	[SerializeField] private float orbitRange;
	[SerializeField] private float attackRange;
	private float currentMoveSpeed;
	[SerializeField] private float minMoveSpeed;
	[SerializeField] private float maxMoveSpeed;
	private float startSpriteRotateSpeed;
	[SerializeField] private float spriteRotateSpeed;
	[SerializeField] private float damagedSpriteRotateSpeed;
	private bool flipped;
	private float behaviorTimer;
	[SerializeField] private float behaviorTime;
	[SerializeField] private float attackTime;

	[Header("Pefabs")]
	[SerializeField] private GameObject DeathExplosion;

	// Start is called before the first frame update
	void Start()
	{
		mySprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
		flipped = Random.Range(0, 2) == 0;
		startHealth = Health;
		startSpriteRotateSpeed = spriteRotateSpeed;
		currentMoveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
	}

	// Update is called once per frame
	void Update()
	{
		OrbitalMovement();
		UpdateSpinEffect();
	}

	/// <summary>
	/// Simply move towards the center of the screen
	/// </summary>
	private void SimpleMovement()
	{
		Vector3 toCenter = Vector3.zero - transform.position;
		if (toCenter.magnitude > 2f)
		{
			transform.Translate(toCenter.normalized * currentMoveSpeed * Time.deltaTime);
		}
	}

	/// <summary>
	/// Move into a set range orbiting around, randomly going in and out to damage buildings.
	/// </summary>
	private void OrbitalMovement()
	{
		// Calculate some info
		Vector3 toCenter = Vector3.zero - transform.position;
		float distance = toCenter.magnitude;

		// Move to the correct range, and orbit when in range
		if (distance > currentRange + 0.1f)
		{
			transform.Translate(toCenter.normalized * currentMoveSpeed * Time.deltaTime);
		}
		else if (distance < currentRange - 0.1f)
		{
			transform.Translate(toCenter.normalized * -currentMoveSpeed * Time.deltaTime);
		}
		else
		{
			behaviorTimer -= Time.deltaTime;
			Vector3 orbitDirection = Vector2.Perpendicular(toCenter);
			if (flipped)
			{
				orbitDirection *= -1f;
			}
			transform.Translate(orbitDirection.normalized * currentMoveSpeed * Time.deltaTime);
		}

		// Every so often, change behavior
		if (behaviorTimer <= 0f)
		{
			behaviorTimer = behaviorTime;
			int decision = Random.Range(0, 3);
			if (decision == 0)
			{
				currentRange = Random.Range(orbitRange, farOrbitRange);
			}
			else if (decision == 1)
			{
				currentRange = farOrbitRange;
			}
			else
			{
				behaviorTimer = attackTime;
				currentRange = attackRange;
			}
			flipped = Random.Range(0, 2) == 0;
			currentMoveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
		}
	}

	/// <summary>
	/// Simple spin effect on the main renderer
	/// </summary>
	private void UpdateSpinEffect()
	{
		mySprite.transform.Rotate(0f, 0f, spriteRotateSpeed * Time.deltaTime * (flipped ? 1 : -1));

		if (spriteRotateSpeed > startSpriteRotateSpeed)
		{
			spriteRotateSpeed -= spriteRotateSpeed * Time.deltaTime;
			if (spriteRotateSpeed < startSpriteRotateSpeed)
			{
				spriteRotateSpeed = startSpriteRotateSpeed;
			}
		}
	}

	/// <summary>
	/// Destroys the enemy and increases score.
	/// </summary>
	private void Kill()
	{
		MyGameManager.MyCameraController.TriggerShake(4, 4, 0.35f);
		MyGameManager.IncreaseScore(scoreGranted);
		Instantiate(DeathExplosion, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	/// <summary>
	/// Blow up after colliding with a bomb.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Bomb"))
		{
			Bomb bomb = other.GetComponent<Bomb>();
			if (bomb.EarlyCheck)
			{
				bomb.MyExplodeCallback += Kill;
				bomb.Detonate();
			}
		}

		if (other.CompareTag("Explosion"))
		{
			Explosion explosion = other.GetComponent<Explosion>();
			Health -= explosion.Damage;
			HealthText.text = "" + Health;
			spriteRotateSpeed = damagedSpriteRotateSpeed;

			float ratio = Health / startHealth;
			Color newColor = Color.Lerp(yellowColor, orangeColor, ratio);
			mySprite.color = newColor;
			SpriteRenderer[] subSprites = mySprite.transform.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer sprite in subSprites)
			{
				sprite.color = newColor;
			}

			damagedParticles.Play();

			behaviorTimer = behaviorTime;
			currentRange = farOrbitRange;

			if (Health <= 0)
			{
				Kill();
			}
		}
	}
}
