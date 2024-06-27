using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

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

        // Sort resolutions from highest to lowest
        Array.Sort(resolutions, (r1, r2) =>
        {
            if (r1.width != r2.width)
            {
                return r2.width.CompareTo(r1.width); // Sort by width descending
            }
            else
            {
                return r2.height.CompareTo(r1.height); // If widths are equal, sort by height descending
            }
        });

        // Clear existing options in dropdown
        resolutionDropdown.ClearOptions();

        List<string> options = new();
        int currentResolutionIndex = 0;

        // Calculate current aspect ratio
        float currentAspectRatio = (float)Screen.width / Screen.height;

        // Use a HashSet to track unique resolutions based on width and height
        HashSet<string> uniqueResolutions = new HashSet<string>();

        // Populate dropdown with resolution options
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Calculate aspect ratio of the resolution
            float aspectRatio = (float)resolutions[i].width / resolutions[i].height;

            // Check if the aspect ratios are approximately equal
            if (Mathf.Approximately(aspectRatio, currentAspectRatio))
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;

                // Add the resolution to the options only if it's not already added
                if (uniqueResolutions.Add(option))
                {
                    options.Add(option);

                    // Check if the resolution matches the game screen resolution
                    if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                    {
                        currentResolutionIndex = options.Count - 1;
                    }
                }
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

        // Get the selected resolution index
        int selectedResolutionIndex = resolutionDropdown.value;

        // Get the actual resolution from the uniqueResolutions dictionary
        string selectedOption = resolutionDropdown.options[selectedResolutionIndex].text;
        int resolutionIndex = Array.FindIndex(resolutions, r => $"{r.width} x {r.height}" == selectedOption);

        // Change resolution
        Resolution resolution = resolutions[resolutionIndex];
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