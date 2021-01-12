using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetNode : MonoBehaviour
{
	[SerializeField] private GameObject targettingSoundPrefab;
	private float timeLeft;
	private float startingScale;
	public Cannon MyCannon;
	public float TimeToSelect;
	public TargetNode NextNode;
	public float ProgressOnLink;

	// Start is called before the first frame update
	void Start()
	{
		startingScale = transform.localScale.x;
		timeLeft = TimeToSelect;
	}

	// Update is called once per frame
	void Update()
	{
		if (!MyCannon.MyGameManager.Paused)
		{
			timeLeft -= Time.unscaledDeltaTime;
			if (timeLeft <= 0f)
			{
				MyCannon.FailedTarget(ProgressOnLink);
			}
		}
		//UpdateSize();
	}

	/// <summary>
	/// Keep the size of the node accurate with how much time is left.
	/// </summary>
	private void UpdateSize()
	{
		float ratio = timeLeft / TimeToSelect;
		if (ratio < 0f)
		{
			ratio = 0f;
		}
		float newScale = Mathf.Lerp(0f, startingScale, ratio);
		transform.localScale = new Vector3(newScale, newScale, newScale);
	}

	//private void OnTriggerEnter(Collider _other)
	//{
	//	CursorCollision(_other);
	//}

	private void OnTriggerStay(Collider _other)
	{
		CursorCollision(_other);
	}

	/// <summary>
	/// If no next node was configured, this is the last one, finalize it with the cannon so it spawns the bomb.
	/// If not, activate the next node for the player to target.
	/// </summary>
	private void CursorCollision(Collider _other)
	{
		if (_other.CompareTag("Cursor"))
		{
			if (NextNode == null)
			{
				MyCannon.SuccessfulTarget();
			}
			else
			{
				NextNode.gameObject.SetActive(true);
				Destroy(gameObject);
				MyCannon.SetTargettingLineProgress(MyCannon.TargettingLineProgress + MyCannon.TargettingLineProgressAmount);
			}
			SoundEffect sound = Instantiate(targettingSoundPrefab).GetComponent<SoundEffect>();
			sound.MyAudioSource.pitch = (1f - ProgressOnLink) + 1f;
		}
	}
}
