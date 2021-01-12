using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	private int currentLayer;
	private int easyMusic;
	private int midMusic;
	private int hardMusic;
	private float transitionTimer;
	[SerializeField] private float musicTransitionTime;
	private List<AudioSource> musicLayers;

	public GameManager MyGameManager;

	// Start is called before the first frame update
	void Start()
	{
		currentLayer = 1;
		easyMusic = Random.Range(0, 2) == 0 ? 1 : 2;
		midMusic = Random.Range(0, 2) == 0 ? 3 : 4;
		hardMusic = Random.Range(0, 2) == 0 ? 5 : 6;
		musicLayers = new List<AudioSource>();
		musicLayers.AddRange(transform.GetComponentsInChildren<AudioSource>());
	}

	// Update is called once per frame
	void Update()
	{
		if (MyGameManager.GameOver)
		{
			currentLayer = 7;
		}
		else
		{
			int difficulty = MyGameManager.DifficultyModifier;
			if (difficulty == 1)
			{
				currentLayer = easyMusic;
			}
			else if (difficulty == 2)
			{
				currentLayer = midMusic;
			}
			else
			{
				currentLayer = hardMusic;
			}
		}
		RefreshLayers();
	}

	private void RefreshLayers()
	{
		for (int i = 0; i < musicLayers.Count; i++)
		{
			AudioSource music = musicLayers[i];
			bool on = i == (currentLayer - 1);
			if (on && music.volume < 1)
			{
				music.volume += musicTransitionTime * Time.unscaledDeltaTime;
			}
			if (!on && music.volume > 0)
			{
				music.volume -= musicTransitionTime * Time.unscaledDeltaTime;
			}
		}
	}
}
