using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmSetting : MonoBehaviour
{
    [Header("Confirmation Window")]
    [SerializeField] private GameObject _confirmPanel;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField][Tooltip("Just put button to this blank, no need to attach function to button's OnClick()")] private Button confirmButton;
    [SerializeField][Tooltip("Just put button to this blank, no need to attach function to button's OnClick()")] private Button revertButton;

    private Coroutine confirmationCoroutine;
    private System.Action confirmAction;
    private System.Action revertAction;

    private void Start()
    {
        _confirmPanel.SetActive(false);
    }

    // OnEnable() will be called when the game object SetActive(true) / enabled = true
    private void OnEnable()
    {
        //Register button
        confirmButton.onClick.AddListener(Confirm);
        revertButton.onClick.AddListener(Revert);
    }

    // OnDisable() will be called when the game object SetActive(false) or enabled = false
    private void OnDisable()
    {
        confirmButton.onClick.RemoveListener(Confirm);
        revertButton.onClick.RemoveListener(Revert);
    }

    public void Confirmation(int countdownTime, string whatIsSetting, System.Action confirmAction, System.Action revertAction)
    {
        this.confirmAction = confirmAction;
        this.revertAction = revertAction;

        /// Open window & Start coroutine
        _confirmPanel.SetActive(true);

        if (confirmationCoroutine != null) StopCoroutine(confirmationCoroutine);
        confirmationCoroutine = StartCoroutine(WaitForConfirmation(countdownTime, whatIsSetting));
        ///
    }

    private IEnumerator WaitForConfirmation(int countdownTime, string whatIsSetting)
    {
        int seconds = countdownTime;
        while (seconds > 0)
        {
            countdownText.text = $"Would you like to keep these {whatIsSetting} settings?\nReverting to previous settings in <color=red>{seconds}</color> seconds.";
            seconds--;
            yield return new WaitForSeconds(1);
        }
        Revert();
    }

    #region Confirm & Revert Button
    public void Confirm()
    {
        confirmAction?.Invoke();
        StopConfirmation();
    }

    public void Revert()
    {
        revertAction?.Invoke();
        StopConfirmation();
    }

    private void StopConfirmation()
    {
        /// Stop coroutine & Close window
        if (confirmationCoroutine != null) StopCoroutine(confirmationCoroutine);

        _confirmPanel.SetActive(false);
        ///
    }
    #endregion
}