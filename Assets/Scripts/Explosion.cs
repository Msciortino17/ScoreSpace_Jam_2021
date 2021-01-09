using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
	[SerializeField] private GameObject[] explosionPrefabs;
	[SerializeField] private float lingerTime;
	private float timer;

	public float Damage;

	// Start is called before the first frame update
	void Start()
	{
		timer = lingerTime;
		GameObject explosionPrefab = explosionPrefabs[Random.Range(0, explosionPrefabs.Length)];
		Instantiate(explosionPrefab);
	}

	// Update is called once per frame
	void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			Destroy(gameObject);
		}
	}
}
