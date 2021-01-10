using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	private GameManager gameManager;

	private const string effectVolumeKey = "EffectsVolume";
	private const string musicVolumeKey = "MusicVolume";

	[Header("Audio")]
	public Slider MusicSlider;
	public Slider EffectSlider;
	public AudioMixer MasterMixer;
	public GameObject Feedback1Prefab;
	public GameObject Feedback2Prefab;

	[Header("UI Stuff")]
	[SerializeField] private GameObject mainMenu;
	[SerializeField] private GameObject howToPlayMenu;
	[SerializeField] private GameObject highScoreMenu;
	[SerializeField] private GameObject optionsMenu;
	[SerializeField] private GameObject creditsMenu;
	[SerializeField] private Text highScoreText;

	[Header("Tutorial Stuff")]
	public List<GameObject> TutorialSteps = new List<GameObject>();
	public GameObject Buildings;
	public GameObject Enemies;
	public GameObject ScoreText;

	// Start is called before the first frame update
	void Start()
	{
		InitAudio();

		gameManager = GetComponent<GameManager>();
		gameManager.LoadHighScores();
		string highScoreInfo = "";
		for (int i = 0; i < 10; i++)
		{
			HighScore score = gameManager.HighScores[i];
			highScoreInfo += score.name + " - " + score.score + "\n";
		}
		highScoreText.text = highScoreInfo;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void TutorialNextStep()
	{
		gameManager.ResetScore();
		gameManager.TutorialStep++;
		if (gameManager.TutorialStep >= 7)
		{
			ToggleHowToPlay();
			return;
		}

		for (int i = 0; i < TutorialSteps.Count; i++)
		{
			TutorialSteps[i].SetActive(i == gameManager.TutorialStep);
		}

		Buildings.SetActive(gameManager.TutorialStep >= 3);
		Enemies.SetActive(gameManager.TutorialStep >= 4);
		ScoreText.SetActive(gameManager.TutorialStep >= 6);
	}

	public void Play()
	{
		Instantiate(Feedback1Prefab);
		SceneManager.LoadScene("Gameplay");
	}

	public void ToggleHowToPlay()
	{
		gameManager.TutorialStep = -1;
		TutorialNextStep();

		howToPlayMenu.SetActive(!howToPlayMenu.activeInHierarchy);
		mainMenu.SetActive(!mainMenu.activeInHierarchy);
		bool returning = mainMenu.activeInHierarchy;
		Instantiate(returning ? Feedback2Prefab : Feedback1Prefab);
	}

	public void ToggleHighScore()
	{
		highScoreMenu.SetActive(!highScoreMenu.activeInHierarchy);
		mainMenu.SetActive(!mainMenu.activeInHierarchy);
		bool returning = mainMenu.activeInHierarchy;
		Instantiate(returning ? Feedback2Prefab : Feedback1Prefab);
	}

	public void ToggleOptions()
	{
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
		mainMenu.SetActive(!mainMenu.activeInHierarchy);
		bool returning = mainMenu.activeInHierarchy;
		Instantiate(returning ? Feedback2Prefab : Feedback1Prefab);
	}

	public void ToggleCredits()
	{
		creditsMenu.SetActive(!creditsMenu.activeInHierarchy);
		optionsMenu.SetActive(!optionsMenu.activeInHierarchy);
		bool returning = creditsMenu.activeInHierarchy;
		Instantiate(returning ? Feedback2Prefab : Feedback1Prefab);
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void InitAudio()
	{
		float effectVolume = -10f;
		if (PlayerPrefs.HasKey(effectVolumeKey))
		{
			effectVolume = PlayerPrefs.GetFloat(effectVolumeKey);
		}
		MasterMixer.SetFloat("EffectsVolume", effectVolume);

		float musicVolume = -10f;
		if (PlayerPrefs.HasKey(musicVolumeKey))
		{
			musicVolume = PlayerPrefs.GetFloat(musicVolumeKey);
		}
		MasterMixer.SetFloat("MusicVolume", musicVolume);

		EffectSlider.value = effectVolume;
		MusicSlider.value = musicVolume;
	}

	public void SetEffectVolume(float volume)
	{
		if (volume < -39f)
		{
			volume = -80f;
		}
		MasterMixer.SetFloat("EffectsVolume", volume);
		PlayerPrefs.SetFloat(effectVolumeKey, volume);
	}

	public void SetMusicVolume(float volume)
	{
		if (volume < -39f)
		{
			volume = -80f;
		}
		MasterMixer.SetFloat("MusicVolume", volume);
		PlayerPrefs.SetFloat(musicVolumeKey, volume);
	}
}
