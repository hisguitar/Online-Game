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
		NewLevel
	}

	// For playing sound
	public void Play(SoundName soundName)
	{
		var sound = GetSound(soundName);
		if (sound.audioSource == null)
		{
			sound.audioSource = gameObject.AddComponent<AudioSource>();
			sound.audioSource.clip = sound.clip;
			sound.audioSource.volume = sound.volume;
			sound.audioSource.loop = sound.loop;
		}
		sound.audioSource.Play();
	}
	
	public void Stop()
	{
		foreach (var sound in sounds)
		{
			if (sound.audioSource != null && sound.audioSource.isPlaying)
			{
				sound.audioSource.Stop();
			}
		}
	}
	
	private Sound GetSound(SoundName soundName)
	{
		return Array.Find(sounds, s => s.soundName == soundName);
	}
}