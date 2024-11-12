using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private int currentResolutionIndex = 0;

    private const string settingsFilePath = "Assets/saveload/settings.json";

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

        LoadSettings();
    }

    public void Resolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void ApplySettings()
    {
        SettingsData settings = new SettingsData()
        {
            ResolutionIndex = resolutionDropdown.value,
            QualityIndex = qualityDropdown.value,
            FullScreen = fullscreenToggle.isOn
        };

        SaveSettings(settings);

        Resolution(resolutionDropdown.value);
        SetQuality(qualityDropdown.value);
        SetFullScreen(fullscreenToggle.isOn);
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
        }
    }

    private void SaveSettings(SettingsData settings)
    {
        string json = JsonUtility.ToJson(settings, true);
        File.WriteAllText(settingsFilePath, json);
    }

    [System.Serializable]
    public class SettingsData
    {
        public int ResolutionIndex;
        public int QualityIndex;
        public bool FullScreen;
    }
}
