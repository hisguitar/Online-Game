using System;
using TMPro;
using Unity.Netcode;
using UnityEngine; 
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text currentHpText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject floatingTextPrefab;

    // Exp and Level
    [SerializeField] private NetworkVariable<int> level = new(1);
    public int Exp { get; private set; } = 0;
    public int ExpToLevelUp { get; private set; } = 100;
    
    // Status
    public NetworkVariable<int> MaxHp = new();
    public NetworkVariable<int> CurrentHp = new();
    public int PlayerStr { get; private set; } = 11;
    public int PlayerVit { get; private set; } = 11;
    public float PlayerAgi { get; private set; } = 3f;

    // New Color
    [HideInInspector] public Color RedFF6666 = new(1f, 0.4f, 0.4f);
    [HideInInspector] public Color RedFF0D0D = new(1f, 0.051f, 0.051f);
    [HideInInspector] public Color Green99FF66 = new(0.6f, 1.0f, 0.4f);
    [HideInInspector] public Color YellowFFFF0D = new(1f, 1f, 0.051f);

    private bool isDead;
    private readonly int statsConverter = 10;
    private readonly float lerpSpeed = 3f;
    
    public Action<Health> Ondie;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            CheckAndResetsPlayerStats();
        }
    }

    private void Update()
    {
        if (IsClient)
        {
            UIUpdate();
        }
    }

    private void UIUpdate()
    {
        // Update HpBar Color
        Color hpBarColor = Color.Lerp(RedFF6666, Green99FF66, (float)CurrentHp.Value / MaxHp.Value);
        hpBar.color = hpBarColor;
        currentHpText.color = hpBarColor;

        #region OverHead & Screen UI
        // Update UI Bar
        float targetFillAmount = (float)CurrentHp.Value / MaxHp.Value;
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);

        // Update UI Text
        currentHpText.text = CurrentHp.Value + "/" + MaxHp.Value;
        levelText.text = "Lv." + level.Value.ToString();
        #endregion
    }

    #region Check & Reset player stats
    private void CheckAndResetsPlayerStats()
    {
        MaxHp.Value = PlayerVit * statsConverter;
        CurrentHp.Value = MaxHp.Value;
    }
    #endregion

    #region Exp & Level
    public void GainExp(int amount)
    {
        // Gain Exp
        Exp += amount;

        // Show FloatingText
        if (floatingTextPrefab != null)
        {
            ShowFloatingTextClientRpc($"+{amount} Exp", YellowFFFF0D);
        }

        // Level Up
        /// Why use while instead if: To make it possible to level up multiple levels in a single move,
        /// if the player has enough Exp to skip multiple levels.
        while (Exp >= ExpToLevelUp)
        {
            level.Value++;
            Exp -= ExpToLevelUp;
            ExpToLevelUp = CalculateExpToLevelUp();
            LevelUpRewards();

            // Show FloatingText
            if (floatingTextPrefab != null)
            {
                ShowFloatingTextClientRpc($"Level up to {level.Value}!", YellowFFFF0D);
            }
        }
    }

    private int CalculateExpToLevelUp()
    {
        return 100 * level.Value;
    }

    private void LevelUpRewards()
    {
        PlayerStr += 1;
        PlayerVit += 1;
        PlayerAgi += 0.1f;

        CheckAndResetsPlayerStats();
    }
    #endregion

    #region Take Damage
    public void TakeDamage(int amount)
    {
        ModifyHealth(-amount);

        // Show FloatingText
        if (floatingTextPrefab != null)
        {
            ShowFloatingTextClientRpc("-" + amount.ToString(), RedFF0D0D);
        }
    }

    public void RestoreHealth(int amount)
    {
        ModifyHealth(amount);

        // Show FloatingText
    }

    // This method can be used for both increasing and decreasing HP
    private void ModifyHealth(int amount)
    {
        if (isDead) { return; }

        int newHealth = CurrentHp.Value + amount;
        CurrentHp.Value = Mathf.Clamp(newHealth, 0, MaxHp.Value);

        if (CurrentHp.Value <= 0)
        {
            CurrentHp.Value = 0;
            isDead = true;
        }
    }
    #endregion

    #region Show Floating Text
    [ClientRpc]
    private void ShowFloatingTextClientRpc(string text, Color textColor)
    {
        ShowFloatingText(text, textColor);
    }

    private void ShowFloatingText(string text, Color textColor)
    {
        GameObject go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.GetComponent<TMP_Text>().color = textColor;
        go.GetComponent<TMP_Text>().text = text;
    }
    #endregion
}