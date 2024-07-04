using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : NetworkBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentHpText;
    [SerializeField] private Image hpBar;

    private readonly float lerpSpeed = 3f;

    private void Update()
    {
        if (IsClient)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // Update Hp-Bar Color
        Color hpBarColor = Color.Lerp(playerHealth.RedFF6666, playerHealth.Green99FF66, (float)playerHealth.CurrentHp.Value / playerHealth.MaxHp.Value);
        hpBar.color = hpBarColor;
        currentHpText.color = hpBarColor;

        #region Player HUD
        // Update Hp-Bar
        float targetFillAmount = (float)playerHealth.CurrentHp.Value / playerHealth.MaxHp.Value;
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);

        // Update Hp-Text & Level-Text
        currentHpText.text = playerHealth.CurrentHp.Value + "/" + playerHealth.MaxHp.Value;
        levelText.text = "Lv." + playerHealth.Level.Value.ToString();
        #endregion
    }
}