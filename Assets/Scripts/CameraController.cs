using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private float shakeTimer = 0f;

	private CinemachineVirtualCamera myVirtualCamera;
	private CinemachineBasicMultiChannelPerlin myCameraShake;

	/// <summary>
	/// Standard startup
	/// </summary>
	void Awake()
	{
		myVirtualCamera = GetComponent<CinemachineVirtualCamera>();
		myCameraShake = myVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
	}

	/// <summary>
	/// Standard update
	/// </summary>
	void Update()
	{
		UpdateCameraShake();
	}

	/// <summary>
	/// Triggers a camera shake based on the given parameters.
	/// </summary>
	public void TriggerShake(float amp, float freq, float duration)
	{
		myCameraShake.m_AmplitudeGain = amp;
		myCameraShake.m_FrequencyGain = freq;
		shakeTimer = duration;
	}

	/// <summary>
	/// Update the timer and logic for camera shaking.
	/// </summary>
	private void UpdateCameraShake()
	{
		if (shakeTimer > 0f)
		{
			shakeTimer -= Time.unscaledDeltaTime;
			if (shakeTimer <= 0f)
			{
				myCameraShake.m_AmplitudeGain = 0f;
				myCameraShake.m_FrequencyGain = 0f;
			}
		}
	}
}
