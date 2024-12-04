using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource sfxSource;
    public AudioSource environmentMusicSource;

    [Header("Audio Clips")]
    public AudioClip[] backgroundMusicClips;
    public AudioClip[] sfxClips;
    public AudioClip[] environmentMusicClips;
    public AudioClip[] playerSFXClips; 

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float backgroundMusicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ApplyVolumeSettings(); // Áp dụng cài đặt âm lượng ngay từ đầu
    }

    // Cài đặt âm lượng nhạc nền
    public void SetBackgroundMusicVolume(float volume)
    {
        backgroundMusicVolume = volume;
        backgroundMusicSource.volume = backgroundMusicVolume;
    }

    // Cài đặt âm lượng SFX và nhạc môi trường
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
        environmentMusicSource.volume = sfxVolume;
    }

    // Áp dụng cài đặt âm lượng cho tất cả nhạc và SFX
    public void ApplyVolumeSettings()
    {
        backgroundMusicSource.volume = backgroundMusicVolume;
        sfxSource.volume = sfxVolume;
        environmentMusicSource.volume = sfxVolume;
    }

    // Phát nhạc nền cho Scene hiện tại
    public void PlayBackgroundMusic(int trackIndex)
    {
        if (backgroundMusicClips != null && trackIndex >= 0 && trackIndex < backgroundMusicClips.Length)
        {
            backgroundMusicSource.clip = backgroundMusicClips[trackIndex];
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("Background music clip index is out of range.");
        }
    }

    // Phát nhạc môi trường
    public void PlayEnvironmentMusic(int trackIndex)
    {
        if (environmentMusicClips != null && trackIndex >= 0 && trackIndex < environmentMusicClips.Length)
        {
            environmentMusicSource.clip = environmentMusicClips[trackIndex];
            environmentMusicSource.loop = true;
            environmentMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("Environment music clip index is out of range.");
        }
    }

    // Phát hiệu ứng âm thanh (SFX)
    public void PlaySFX(int clipIndex)
    {
        if (sfxClips != null && clipIndex >= 0 && clipIndex < sfxClips.Length)
        {
            sfxSource.PlayOneShot(sfxClips[clipIndex], sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFX clip index is out of range.");
        }
    }

    // Phát hiệu ứng âm thanh cho Player
    public void PlayPlayerSFX(int clipIndex)
    {
        if (playerSFXClips != null && clipIndex >= 0 && clipIndex < playerSFXClips.Length)
        {
            sfxSource.PlayOneShot(playerSFXClips[clipIndex], sfxVolume);
        }
        else
        {
            Debug.LogWarning("Player SFX clip index is out of range.");
        }
    }

    public void StopAllMusic()
    {
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }

        if (environmentMusicSource.isPlaying)
        {
            environmentMusicSource.Stop();
        }

        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }
}
