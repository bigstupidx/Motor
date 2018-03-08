using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
class LoopSound : MonoBehaviour
{
	AudioSource m_AudioSource;
	void Awake()
	{
		m_AudioSource = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (!m_AudioSource.isPlaying)
		{
			m_AudioSource.Play();
		}
	}
}

