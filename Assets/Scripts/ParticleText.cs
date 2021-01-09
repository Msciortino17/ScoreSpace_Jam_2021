using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticleText : MonoBehaviour
{
	[SerializeField] private Text myText;
	private float timer;

	// Start is called before the first frame update
	void Start()
	{
		timer = 1f;
	}

	// Update is called once per frame
	void Update()
	{
		timer -= Time.unscaledDeltaTime;
		if (timer <= 0f)
		{
			Destroy(gameObject);
		}

		transform.Translate(0f, 0.25f * Time.unscaledDeltaTime, 0f);
	}

	public void SetText(string _text)
	{
		myText.text = _text;
	}
}
