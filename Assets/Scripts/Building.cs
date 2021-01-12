using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
	[SerializeField] private GameManager gameManager;
	[SerializeField] private ParticleSystem damageParticles;
	[SerializeField] private AudioSource damageSound;
	[SerializeField] private Text healthText;

	public float Health;

	// Start is called before the first frame update
	void Start()
	{
		healthText.text = "" + (int)Health;
	}

	// Update is called once per frame
	void Update()
	{
		if (Health <= 0f)
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Explosions will damage buildings.
	/// </summary>
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Explosion"))
		{
			Explosion explosion = other.GetComponent<Explosion>();
			Health -= explosion.Damage;
			healthText.text = "" + (int)Health;

			if (!damageSound.isPlaying)
			{
				damageSound.Play();
			}
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Enemy"))
		{
			Enemy enemy = other.gameObject.GetComponent<Enemy>();
			Health -= enemy.Damage * Time.deltaTime;
			healthText.text = "" + (int)Health;

			if (!damageSound.isPlaying)
			{
				damageSound.Play();
			}

			if (!damageParticles.isPlaying)
			{
				damageParticles.Play();
			}

			gameManager.MyCameraController.TriggerShake(0.75f, 0.75f, 0.1f);
		}
	}
}
