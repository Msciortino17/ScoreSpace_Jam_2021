using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
	private Transform mySprite;
	[SerializeField] private int scoreGranted;
	public GameManager MyGameManager;
	[SerializeField] private Text healthText;

	public float Health;
	public float Damage;

	[Header("Movement Settings")]
	[SerializeField] private float farOrbitRange;
	[SerializeField] private float orbitRange;
	[SerializeField] private float attackRange;
	[SerializeField] private float moveSpeed;
	[SerializeField] private float spriteRotateSpeed;
	[SerializeField] private bool bounces;
	private bool attacking;
	private bool fallback;
	private bool flipped;
	private float behaviorTimer;
	[SerializeField] private float behaviorTime;

	[Header("Pefabs")]
	[SerializeField] private GameObject DeathExplosion;

	// Start is called before the first frame update
	void Start()
	{
		healthText.text = "" + (int)Health;
		mySprite = transform.Find("Sprite");
		flipped = Random.Range(0, 2) == 0;
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
			transform.Translate(toCenter.normalized * moveSpeed * Time.deltaTime);
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
		float range = attacking ? attackRange : orbitRange;
		range = fallback ? farOrbitRange : range;

		// Move to the correct range, and orbit when in range
		if (distance > range + 0.1f)
		{
			transform.Translate(toCenter.normalized * moveSpeed * Time.deltaTime);
		}
		else if (distance < range - 0.1f)
		{
			transform.Translate(toCenter.normalized * -moveSpeed * Time.deltaTime);
		}
		else
		{
			Vector3 orbitDirection = Vector2.Perpendicular(toCenter);
			if (flipped)
			{
				orbitDirection *= -1f;
			}
			transform.Translate(orbitDirection.normalized * moveSpeed * Time.deltaTime);
		}

		// Every so often, change behavior
		behaviorTimer -= Time.deltaTime;
		if (behaviorTimer <= 0f)
		{
			behaviorTimer = behaviorTime;
			int decision = Random.Range(0, 3);
			if (decision == 0)
			{
				attacking = false;
				fallback = false;
			}
			else if (decision == 1)
			{
				attacking = false;
				fallback = true;
			}
			else
			{
				attacking = true;
				fallback = false;
			}
		}
	}

	/// <summary>
	/// Simple spin effect on the main renderer
	/// </summary>
	private void UpdateSpinEffect()
	{
		mySprite.transform.Rotate(0f, 0f, spriteRotateSpeed * Time.deltaTime);
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
			healthText.text = "" + (int)Health;
			if (Health <= 0)
			{
				Kill();
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Enemy") && bounces)
		{
			flipped = !flipped;
		}
	}
}
