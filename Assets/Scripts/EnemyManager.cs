using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	private int totalEnemiesSpawned;
	private GameManager gameManager;
	private Transform spawnersParent;
	private List<Transform> enemySpawners;

	[Header("Spawn Settings")]
	[SerializeField] private float spawnRate;
	[SerializeField] private int maxEnemies;
	private float spawnTimer;
	[SerializeField] private float spawnerSpinRate;

	[Header("Prefabs")]
	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private GameObject enemyPrefab2;

	/// <summary>
	/// Let's us know how many enemies there are.
	/// Subtracting 1 because the spawners are nested under here too.
	/// </summary>
	public int EnemyCount
	{
		get
		{
			return transform.childCount - 1;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		gameManager = transform.parent.GetComponent<GameManager>();
		LoadSpawners();
	}

	/// <summary>
	/// Reads in all enemy spawner game objects configured on the child.
	/// </summary>
	private void LoadSpawners()
	{
		enemySpawners = new List<Transform>();
		spawnersParent = transform.Find("EnemySpawners");
		foreach (Transform spawner in spawnersParent)
		{
			enemySpawners.Add(spawner);
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!gameManager.IsMainMenu || gameManager.TutorialStep >= 0)
		{
			UpdateSpawnEnemies();
			SpinSpawners();
		}
	}

	/// <summary>
	/// Will churn out enemies at the configured rate.
	/// </summary>
	private void UpdateSpawnEnemies()
	{
		spawnTimer -= Time.deltaTime;
		if (spawnTimer <= 0f && EnemyCount < maxEnemies * gameManager.DifficultyModifier)
		{
			totalEnemiesSpawned++;
			int type = 1;
			if (totalEnemiesSpawned % 5 == 0)
			{
				type = 2;
			}
			SpawnEnemy(type);
			spawnTimer = spawnRate / gameManager.DifficultyModifier;
		}
	}

	/// <summary>
	/// Spins the spawners around, mixing up their positions a bit more.
	/// </summary>
	private void SpinSpawners()
	{
		spawnersParent.transform.Rotate(0f, 0f, spawnerSpinRate * Time.deltaTime);
	}

	/// <summary>
	/// Will spawn an enemy at a random spawner.
	/// </summary>
	private void SpawnEnemy(int _type)
	{
		Transform spawner = enemySpawners[Random.Range(0, enemySpawners.Count)];

		GameObject prefab = enemyPrefab;
		if (_type == 2)
		{
			prefab = enemyPrefab2;
		}

		Enemy enemy = Instantiate(prefab, transform).GetComponent<Enemy>();
		enemy.transform.position = spawner.position;
		enemy.MyGameManager = gameManager;
	}
}
