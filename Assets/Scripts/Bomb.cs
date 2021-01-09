using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	public bool Teleporting;
	private ParticleSystem myParticleSystem;
	private SpriteRenderer mySpriteRenderer;

	public GameManager gameManager;
	public delegate void ExplodeCallback();
	public ExplodeCallback MyExplodeCallback;

	[SerializeField] private float explodeTime;
	private float timer;

	public float Damage;

	[Header("Prefabs")]
	[SerializeField] private GameObject explosionPrefab;

	/// <summary>
	/// Checks if the timer has made less than a tenth of a second progress.
	/// Used in enemies to see if the bomb spawned within.
	/// </summary>
	public bool EarlyCheck
	{
		get
		{
			return timer <= 0.1f;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		myParticleSystem = GetComponent<ParticleSystem>();
		mySpriteRenderer = GetComponent<SpriteRenderer>();
		timer = 0f;
		Teleporting = true;
	}

	// Update is called once per frame
	void Update()
	{
		if (Teleporting)
		{
			if (!myParticleSystem.isPlaying)
			{
				Teleporting = false;
				mySpriteRenderer.enabled = true;
				gameManager.NormalizeTime();
			}
		}
		else
		{
			timer += Time.deltaTime;
			if (timer > explodeTime)
			{
				Explode();
			}
		}
	}

	/// <summary>
	/// Blows up the bomb, spawning the explosion.
	/// </summary>
	public void Explode()
	{
		Destroy(gameObject);
		Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity).GetComponent<Explosion>();
		explosion.Damage = Damage;
		MyExplodeCallback?.Invoke();
	}

	/// <summary>
	/// Maxes out timer so it'll explode right away.
	/// Main use is that if this is called while teleporting, the teleport effect will finish, then it will explode.
	/// </summary>
	public void Detonate()
	{
		timer = explodeTime;
	}
}
