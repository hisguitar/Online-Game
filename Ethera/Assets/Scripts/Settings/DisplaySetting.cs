using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(ConfirmSetting))]
public class DisplaySetting : MonoBehaviour
{
    [Header("Confirmation Window")]
    [SerializeField][Tooltip("Any window that attached this script")] private ConfirmSetting confirmSetting;

    [Header("Display Window")]
    [SerializeField] private TMP_Dropdown displayModeDropdown;

    private FullScreenMode previousDisplayMode;
    private bool isInitialized = false;

    private void Start()
    {
        confirmSetting = GetComponent<ConfirmSetting>();

        /// Display Setting at Start()
        displayModeDropdown.ClearOptions();

        List<string> options = new() { "Fullscreen", "Windowed" };
        displayModeDropdown.AddOptions(options);

        // Set current display mode in dropdown
        switch (Screen.fullScreenMode)
        {
            case FullScreenMode.FullScreenWindow:
                displayModeDropdown.value = 0;
                break;
            case FullScreenMode.Windowed:
                displayModeDropdown.value = 1;
                break;
        }

        displayModeDropdown.RefreshShownValue();

        // Save previous display
        previousDisplayMode = Screen.fullScreenMode;
        ///
    }

    // You have to put this function into 'On Value Changed (int32)' slot in dropdown GameObject
    public void SetDisplayMode()
    {
        /// Change display
        FullScreenMode selectedMode = GetSelectedMode();
        Screen.fullScreenMode = selectedMode;
        ///

        ///  'On Value Changed' in TMP_Dropdown make this function called at Start();
        ///  So I use 'isInitialized' to prevent coroutine from accidentally running.
        if (!isInitialized)
        {
            isInitialized = true;
        }
        else
        {
            confirmSetting.Confirmation(10, "display", ConfirmDisplay, RevertDisplay);
        }
        ///
    }

    private FullScreenMode GetSelectedMode()
    {
        switch (displayModeDropdown.value)
        {
            case 0:
                return FullScreenMode.FullScreenWindow;
            case 1:
                return FullScreenMode.Windowed;
            default:
                return Screen.fullScreenMode;
        }
    }

    #region Confirm & Revert Button
    private void ConfirmDisplay()
    {
        /// Confirm display
        previousDisplayMode = Screen.fullScreenMode;
        ///
    }

    private void RevertDisplay()
    {
        /// Revert display
        Screen.fullScreenMode = previousDisplayMode;
        ///

        /// Revert dropdown list and display mode(match dropdown list with previousDisplayMode)
        switch (previousDisplayMode)
        {
            case FullScreenMode.FullScreenWindow:
                displayModeDropdown.value = 0;
                break;
            case FullScreenMode.Windowed:
                displayModeDropdown.value = 1;
                break;
        }
        displayModeDropdown.RefreshShownValue();
        ///
    }
    #endregion
}