using System;
using TMPro;
using Unity.Netcode;
using UnityEngine; 
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    #region Reference to object
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text currentHpText;
    [SerializeField] private GameObject floatingTextPrefab;
    #endregion

    #region Create color
    [HideInInspector] public Color RedFF6666 = new(1f, 0.4f, 0.4f);
    [HideInInspector] public Color RedFF0D0D = new(1f, 0.051f, 0.051f);
    [HideInInspector] public Color Green99FF66 = new(0.6f, 1.0f, 0.4f);
    [HideInInspector] public Color YellowFFFF0D = new(1f, 1f, 0.051f);
    #endregion

    #region Level system & Player status
    // Level & Exp
    public NetworkVariable<int> Exp = new();
    public int ExpToLevelUp { get; private set; } = 100;
    private readonly NetworkVariable<int> level = new(1);

    // Status
    public NetworkVariable<int> MaxHp = new();
    public NetworkVariable<int> CurrentHp = new();
    public int PlayerStr { get; private set; } = 11;
    public int PlayerVit { get; private set; } = 11;
    public float PlayerAgi { get; private set; } = 3f;

    public Action<Health> OnDie;
    private ulong playerID;
    private bool isDead;
    private readonly int exp = 75;
    private readonly int statsConverter = 10;
    private readonly float lerpSpeed = 3f;
    #endregion

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        CheckAndResetsPlayerStats();
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
        // Convert stats to use in game
        MaxHp.Value = PlayerVit * statsConverter;

        // Set to maximum value at first
        CurrentHp.Value = MaxHp.Value;
    }
    #endregion

    #region Exp & Level
    public void GainExp(int amount)
    {
        // Gain Exp
        Exp.Value += amount;

        // Show FloatingText
        if (floatingTextPrefab != null)
        {
            ShowFloatingTextClientRpc($"+{amount} Exp", YellowFFFF0D);
        }

        // Level Up
        /// Why use while instead if: To make it possible to level up multiple levels in a single move,
        /// if the player has enough Exp to skip multiple levels.
        while (Exp.Value >= ExpToLevelUp)
        {
            level.Value++;
            Exp.Value -= ExpToLevelUp;
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

    #region Check owner of bullet
    // Check owner of bullet to give exp to player who killed this player.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If rigidbody == null, the code below will not working
        Rigidbody2D otherRigidbody = collision.attachedRigidbody;
        if (otherRigidbody == null) return;

        // Check owner of bullet
        if (otherRigidbody.TryGetComponent<DealDamageOnContact>(out DealDamageOnContact bullet))
        {
            playerID = bullet.ownerClientId;
        }
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

        if (CurrentHp.Value == 0)
        {
            // Gain exp when killing other players.
            Health player = NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject.GetComponent<Health>();
            if (player != null)
            {
                player.GainExp(exp);
            }

            // Player die process
            OnDie?.Invoke(this);
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