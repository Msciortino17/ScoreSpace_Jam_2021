using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
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
		}
	}

	/// <summary>
	/// When an enemy is overlapping the building, apply damage
	/// </summary>
	private void OnCollisionStay(Collision collision)
	{
		if (collision.gameObject.CompareTag("Enemy"))
		{
			Enemy enemy = collision.gameObject.GetComponent<Enemy>();
			Health -= enemy.Damage * Time.deltaTime;
			healthText.text = "" + (int)Health;
		}
	}
}
