using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
	public AudioSource MyAudioSource;

	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void Update()
	{
		if (!MyAudioSource.isPlaying)
		{
			Destroy(gameObject);
		}
	}
}
