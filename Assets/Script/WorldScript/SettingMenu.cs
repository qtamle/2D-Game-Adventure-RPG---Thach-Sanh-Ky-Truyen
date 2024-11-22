using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [SerializeField] private Slider backgroundMusicSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private GameObject[] uiCanvasesToToggle; // Mảng các Canvas/UI cần ẩn/hiện

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private int currentResolutionIndex = 0;

    private const string settingsFilePath = "Assets/saveload/settings.json";
    private bool isDropdownProcessing = false;

    private void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();
        resolutionDropdown.ClearOptions();

        var currentRefreshRateRatio = Screen.currentResolution.refreshRateRatio;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRateRatio.Equals(currentRefreshRateRatio))
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            int refreshRateHz = (int)filteredResolutions[i].refreshRateRatio.numerator / (int)filteredResolutions[i].refreshRateRatio.denominator;
            string resolutionOption = filteredResolutions[i].width + " x " + filteredResolutions[i].height + " - " + refreshRateHz + "Hz";
            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        backgroundMusicSlider.value = AudioManager.Instance.backgroundMusicVolume;
        sfxSlider.value = AudioManager.Instance.sfxVolume;

        backgroundMusicSlider.onValueChanged.AddListener(SetBackgroundMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        LoadSettings();

        ToggleUICanvases(true);
    }

    public void OpenSettings()
    {
        ToggleUICanvases(false); // Ẩn các UI không liên quan khi mở menu cài đặt
        this.gameObject.SetActive(true); // Hiển thị menu cài đặt
    }

    public void CloseSettings()
    {
        ToggleUICanvases(true); // Hiển thị lại các UI không liên quan
        this.gameObject.SetActive(false); // Ẩn menu cài đặt khi người dùng đóng nó
    }

    public void Resolution(int resolutionIndex)
    {
        if (resolutionIndex != currentResolutionIndex) 
        {
            currentResolutionIndex = resolutionIndex; 
            Resolution resolution = filteredResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    public void SetQuality(int quality)
    {
        if (quality != QualitySettings.GetQualityLevel()) 
        {
            QualitySettings.SetQualityLevel(quality);
        }
    }


    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void ApplySettings()
    {
        SettingsData settings = new SettingsData()
        {
            ResolutionIndex = resolutionDropdown.value,
            QualityIndex = qualityDropdown.value,
            FullScreen = fullscreenToggle.isOn,
            BackgroundMusicVolume = backgroundMusicSlider.value,
            SFXVolume = sfxSlider.value
        };

        SaveSettings(settings);

        Resolution(resolutionDropdown.value);
        SetQuality(qualityDropdown.value);
        SetFullScreen(fullscreenToggle.isOn);

        AudioManager.Instance.SetBackgroundMusicVolume(backgroundMusicSlider.value);
        AudioManager.Instance.SetSFXVolume(sfxSlider.value);
        AudioManager.Instance.ApplyVolumeSettings();
    }

    private void LoadSettings()
    {
        if (File.Exists(settingsFilePath))
        {
            string json = File.ReadAllText(settingsFilePath);
            SettingsData settings = JsonUtility.FromJson<SettingsData>(json);

            resolutionDropdown.value = settings.ResolutionIndex;
            resolutionDropdown.RefreshShownValue();
            Resolution(resolutionDropdown.value);

            qualityDropdown.value = settings.QualityIndex;
            qualityDropdown.RefreshShownValue();
            SetQuality(qualityDropdown.value);

            fullscreenToggle.isOn = settings.FullScreen;
            SetFullScreen(fullscreenToggle.isOn);

            backgroundMusicSlider.value = settings.BackgroundMusicVolume;
            sfxSlider.value = settings.SFXVolume;

            AudioManager.Instance.SetBackgroundMusicVolume(settings.BackgroundMusicVolume);
            AudioManager.Instance.SetSFXVolume(settings.SFXVolume);
        }
    }

    private void SaveSettings(SettingsData settings)
    {
        settings.BackgroundMusicVolume = backgroundMusicSlider.value;
        settings.SFXVolume = sfxSlider.value;

        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsFilePath, json);
    }

    private void ToggleUICanvases(bool isVisible)
    {
        foreach (GameObject canvas in uiCanvasesToToggle)
        {
            if (canvas != null)
            {
                canvas.SetActive(isVisible);
            }
        }
    }

    public void UIHide()
    {
        ToggleUICanvases(false);
    }

    public void UIShow()
    {
        ToggleUICanvases(true);
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        AudioManager.Instance.SetBackgroundMusicVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioManager.Instance.SetSFXVolume(volume);
    }

    [System.Serializable]
    public class SettingsData
    {
        public int ResolutionIndex;
        public int QualityIndex;
        public bool FullScreen;
        public float BackgroundMusicVolume;
        public float SFXVolume;
    }
}

