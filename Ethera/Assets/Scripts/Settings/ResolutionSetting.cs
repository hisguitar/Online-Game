using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ConfirmSetting))]
public class ResolutionSetting : MonoBehaviour
{
    [Header("Confirmation Window")]
    [SerializeField][Tooltip("Any window that attached this script")] private ConfirmSetting confirmSetting;

    [Header("Resolution Window")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private Resolution[] resolutions;
    private int previousResolutionIndex;
    private bool isInitialized = false;

    private void Start()
    {
        confirmSetting = GetComponent<ConfirmSetting>();

        // Initialize resolutions array with available screen resolutions
        resolutions = Screen.resolutions;

        // Clear existing options in dropdown
        resolutionDropdown.ClearOptions();

        List<string> options = new();
        int currentResolutionIndex = 0;

        // Filter resolutions to include only those that match the aspect ratio of the current screen
        List<Resolution> filteredResolutions = new List<Resolution>();
        float currentAspectRatio = (float)Screen.width / Screen.height;

        foreach (Resolution resolution in resolutions)
        {
            float aspectRadio = (float)resolution.width / resolution.height;
            if (Mathf.Approximately(aspectRadio, currentAspectRatio))
            {
                filteredResolutions.Add(resolution);
            }
        }

        // Populate dropdown with resolution options
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string option = filteredResolutions[i].width + " x " + filteredResolutions[i].height;
            options.Add(option);

            // Check if the resolution matches the game screen resolution
            if (filteredResolutions[i].width == Screen.width &&
                filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Add resolution options to the dropdown and set the current resolution
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Save the previous resolution index
        previousResolutionIndex = currentResolutionIndex;

        // Mark as initialized after setting up the initial state
        isInitialized = true;
    }

    // This function should be added to the 'On Value Changed (int32)' slot in the dropdown GameObject
    public void SetResolution()
    {
        // Prevent running the resolution change during initialization
        if (!isInitialized)
        {
            return;
        }

        /// Change resolution
        Resolution resolution = resolutions[resolutionDropdown.value];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        confirmSetting.Confirmation(10, "resolution", ConfirmResolution, RevertResolution);
    }

    #region Confirm & Revert Button
    private void ConfirmResolution()
    {
        // Confirmed resolution index
        previousResolutionIndex = resolutionDropdown.value;
    }

    private void RevertResolution()
    {
        // Revert to the previous resolution
        Resolution previousResolution = resolutions[previousResolutionIndex];
        Screen.SetResolution(previousResolution.width, previousResolution.height, Screen.fullScreen);

        // Revert the dropdown list to the previous value
        resolutionDropdown.value = previousResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
    #endregion
}