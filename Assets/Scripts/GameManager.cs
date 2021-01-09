using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	private int score;
	private bool GameOver;
	private float m_ElapsedGameTime;
	private float m_ElapsedRealTime;


	[Header("Slowdown Settings")]
	[SerializeField] private float slowTimeScale;
	[SerializeField] private float normalizeTransitionLength;
	private float normalizeTransitionTimer;
	public bool TimeIsSlow;

	[Header("Managers and Parent objects")]
	public EnemyManager MyEnemyManager;
	public Transform TargettingNodes;
	public Transform BuildingsParent;
	public CameraController MyCameraController;

	[Header("UI Stuff")]
	public Text ScoreText;
	public Text FinalScoreText;
	public GameObject GameOverScreen;

	public int BuildingCount
	{
		get
		{
			return BuildingsParent.childCount;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		GameOver = false;
		TargettingNodes = transform.Find("TargettingNodes");
		BuildingsParent = transform.Find("Buildings");
	}

	// Update is called once per frame
	void Update()
	{
		UpdateTimers();
		UpdateSlowDown();
		UpdateGameOver();
	}

	/// <summary>
	/// Updates the timers.
	/// </summary>
	private void UpdateTimers()
	{
		m_ElapsedGameTime += Time.deltaTime;
		m_ElapsedRealTime += Time.unscaledDeltaTime;
	}

	/// <summary>
	/// Mainly for handling the transition from slow to normal time.
	/// </summary>
	private void UpdateSlowDown()
	{
		if (!TimeIsSlow && normalizeTransitionTimer > 0f)
		{
			normalizeTransitionTimer -= Time.unscaledDeltaTime;
			float ratio = normalizeTransitionTimer / normalizeTransitionLength;
			if (ratio < 0f)
			{
				ratio = 0f;
			}
			Time.timeScale = Mathf.Lerp(1f, slowTimeScale, ratio);
		}
	}

	/// <summary>
	/// Checks if all buildings have been destroyed, and pulls up the game over screen if true.
	/// </summary>
	private void UpdateGameOver()
	{
		if (BuildingCount <= 0)
		{
			Time.timeScale = 0f;
			GameOver = true;
			GameOverScreen.SetActive(true);
			FinalScoreText.text = "Score: " + score;
		}
	}

	/// <summary>
	/// Immediately slows down time.
	/// </summary>
	public void SlowDownTime()
	{
		if (!TimeIsSlow)
		{
			Time.timeScale = slowTimeScale;
			TimeIsSlow = true;
		}
	}

	/// <summary>
	/// Begins the process of gradually bringing time back.
	/// </summary>
	public void NormalizeTime()
	{
		if (TimeIsSlow)
		{
			normalizeTransitionTimer = normalizeTransitionLength;
			TimeIsSlow = false;
		}
	}

	public void IncreaseScore(int _amount)
	{
		score += _amount;
		ScoreText.text = "" + score;
	}

	/// <summary>
	/// Restarts the scene.
	/// </summary>
	public void Restart()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
