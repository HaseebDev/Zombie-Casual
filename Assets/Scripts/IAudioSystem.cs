using System;

public interface IAudioSystem
{
	void Mute();

	void PlayBgMusic(BGM_ENUM musicType);

	void PlaySFX(SFX_ENUM soundType);

	void Unmute();
}
