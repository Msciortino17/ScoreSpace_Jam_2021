using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public struct HighScore
{
	public string name;
	public int score;
}

public class GameManager : MonoBehaviour
{
	private int score;
	public bool Paused;
	private float m_ElapsedGameTime;
	private float m_ElapsedRealTime;
	public List<HighScore> HighScores;
	public int TutorialStep;

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
	public Text FinalScoreTextQuit;
	public GameObject GameOverScreen;
	public GameObject PausedScreen;
	public GameObject QuitScreen;
	public bool IsMainMenu;
	public InputField NameInputFieldGameOver;
	public InputField NameInputFieldQuit;

	public int BuildingCount
	{
		get
		{
			return BuildingsParent.childCount;
		}
	}

	public int DifficultyModifier
	{
		get
		{
			if (m_ElapsedGameTime > 90f)
			{
				return 3;
			}
			else if (m_ElapsedGameTime > 30f)
			{
				return 2;
			}
			return 1;
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		TargettingNodes = transform.Find("TargettingNodes");
		BuildingsParent = transform.Find("Buildings");
		LoadHighScores();
	}

	/// <summary>
	/// Read in current high scores from player prefs
	/// </summary>
	public void LoadHighScores()
	{
		HighScores = new List<HighScore>();
		for (int i = 0; i < 10; i++)
		{
			HighScore score = new HighScore();
			score.name = PlayerPrefs.GetString("HighScoreName_" + i, "None");
			score.score = PlayerPrefs.GetInt("HighScoreValue_" + i, 0);
			HighScores.Add(score);
		}
	}

	/// <summary>
	/// Save current high scores to player prefs
	/// </summary>
	public void SaveHighScores()
	{
		for (int i = 0; i < 10; i++)
		{
			HighScore score = HighScores[i];
			PlayerPrefs.SetString("HighScoreName_" + i, string.IsNullOrEmpty(score.name) ? "Unnamed" : score.name);
			PlayerPrefs.SetInt("HighScoreValue_" + i, score.score);
		}
	}

	/// <summary>
	/// See if the given high score can take a place
	/// </summary>
	public void InsertHighScore(HighScore _score)
	{
		for (int i = 0; i < 10; i++)
		{
			HighScore otherScore = HighScores[i];
			if (_score.score > otherScore.score)
			{
				HighScores.Insert(i, _score);
				HighScores.RemoveAt(10);
				return;
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		if (!IsMainMenu)
		{
			UpdateTimers();
			UpdateGameOver();
			UpdatePaused();
		}
		UpdateSlowDown();
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
	/// Checks for input to pause/unpause the game.
	/// </summary>
	private void UpdatePaused()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause();
		}
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

	public void ResetScore()
	{
		score = 0;
		ScoreText.text = "" + score;
	}

	public void PlayAgain()
	{
		HighScore finalScore = new HighScore();
		finalScore.name = NameInputFieldGameOver.text;
		finalScore.score = score;
		InsertHighScore(finalScore);
		SaveHighScores();
		Restart();
	}

	/// <summary>
	/// Restarts the scene.
	/// </summary>
	public void Restart()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	/// <summary>
	/// Toggles pause.
	/// </summary>
	public void TogglePause()
	{
		if (Paused)
		{
			Time.timeScale = TimeIsSlow ? slowTimeScale : 1f;
			Paused = false;
			PausedScreen.SetActive(false);
		}
		else
		{
			Time.timeScale = 0f;
			Paused = true;
			PausedScreen.SetActive(true);
		}
	}

	public void VerifyQuit()
	{
		FinalScoreTextQuit.text = "Score: " + score;
		QuitScreen.SetActive(true);
		PausedScreen.SetActive(false);
	}

	public void UndoQuit()
	{
		QuitScreen.SetActive(false);
		PausedScreen.SetActive(true);
	}

	public void ReturnToMenu()
	{
		Time.timeScale = 1f;
		HighScore finalScore = new HighScore();
		finalScore.name = NameInputFieldQuit.text;
		finalScore.score = score;
		InsertHighScore(finalScore);
		SaveHighScores();
		SceneManager.LoadScene("MainMenu");
	}
}
