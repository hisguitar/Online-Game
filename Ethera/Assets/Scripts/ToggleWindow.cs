using UnityEngine;

public class ToggleWindow : MonoBehaviour
{
    public GameObject window;
    public KeyCode shortcut; // Key to open/close window

    private void Update()
    {
        // Shortcut to open/close window
        if (Input.GetKeyDown(shortcut))
        {
            OnOpenOrClose();
        }
    }

    #region window control
    // Add this script to any window you need to open/close, Can be use to button.
    public void OnOpenOrClose()
    {
        if (window.activeSelf == false)
        {
            LeanTween.scale(window, new Vector3(1, 1, 1), 0.25f).setOnStart(Toggle);
        }
        else if (window.activeSelf == true)
        {
            LeanTween.scale(window, new Vector3(0, 0, 0), 0.25f).setOnComplete(Toggle);
        }
    }

    // This method below already used in OnOpenOrClose method
    // Toggle window active/inactive
    private void Toggle()
    {
        if (window != null)
        {
            window.SetActive(!window.activeSelf);
        }
    }
    #endregion
}