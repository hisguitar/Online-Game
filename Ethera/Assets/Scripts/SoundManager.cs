using System;
using UnityEngine;

public class SoundManager : SingletonPersistent<SoundManager>
{
	[SerializeField] private Sound[] sounds;

	// List of sounds
	public enum SoundName
	{
		FastMagicGameSpell,
		ShatterShotExplosion,
		NewLevel,
		WhiteMorning
	}

	// For setting sound
	public void Play(SoundName soundName)
	{
		var sound = GetSound(soundName);
		if (sound.audioSource == null)
		{
			sound.audioSource = gameObject.AddComponent<AudioSource>();
			sound.audioSource.clip = sound.clip;
			sound.audioSource.volume = sound.volume;
			sound.audioSource.loop = sound.loop;
			sound.audioSource.outputAudioMixerGroup = sound.audioMixerGroup;
		}
		sound.audioSource.Play();
	}

	private Sound GetSound(SoundName soundName)
	{
		return Array.Find(sounds, s => s.soundName == soundName);
	}

	public void Stop(SoundName soundName)
	{
		var sound = GetSound(soundName);
		if (sound.audioSource != null)
		{
			sound.audioSource.Stop();
		}
	}
	
	public void StopAll()
	{
		foreach (var sound in sounds)
		{
			if (sound.audioSource != null)
			{
				sound.audioSource.Stop();
			}
		}
	}
}