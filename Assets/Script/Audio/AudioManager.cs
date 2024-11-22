using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource; 
    public AudioSource sfxSource;            

    [Header("Audio Clips")]
    public AudioClip[] backgroundMusicClips;  
    public AudioClip[] sfxClips;            

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float backgroundMusicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo AudioManager không bị phá hủy khi chuyển Scene
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

    // Cài đặt âm lượng SFX
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
    }

    // Áp dụng cài đặt âm lượng cho cả nhạc nền và SFX
    public void ApplyVolumeSettings()
    {
        backgroundMusicSource.volume = backgroundMusicVolume;
        sfxSource.volume = sfxVolume;
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
}
