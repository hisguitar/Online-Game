using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Cinemachine;
using Unity.Collections;
using System;

public class Player : NetworkBehaviour
{
    [Header("Settings")]
    // Character Creation
    public NetworkVariable<FixedString32Bytes> PlayerName = new();
    public NetworkVariable<int> PlayerColorIndex = new();

    // Exp and Level
    [SerializeField] private NetworkVariable<int> level = new(1);
    public int Exp { get; private set; } = 0;
    public int ExpToLevelUp { get; private set; } = 100;

    // Player Stats
    [SerializeField] private NetworkVariable<int> maxHp = new();
    [SerializeField] private NetworkVariable<int> hp = new();
    public int PlayerStr { get; private set; } = 11;
    public int PlayerVit { get; private set; } = 11;
    public float PlayerAgi { get; private set; } = 3f;

    [Header("Reference")]
    [SerializeField] private int ownerPriority = 15;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text currentHpText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject floatingTextPrefab;

    public static event Action<Player> OnPlayerSpawned;
    public static event Action<Player> OnPlayerDespawned;

    // New Color
    public Color RedFF6666 { get; private set; } = new(1f, 0.4f, 0.4f);
    public Color RedFF0D0D { get; private set; } = new(1f, 0.051f, 0.051f);
    public Color Green99FF66 { get; private set; } = new(0.6f, 1.0f, 0.4f);
    public Color YellowFFFF0D { get; private set; } = new(1f, 1f, 0.051f);

    private bool isDead;
    private readonly int statsConverter = 10;
    private readonly float lerpSpeed = 3f;

    // OnNetworkSpawn is used when an object begins network connection.
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            UserData userData =
                HostSingleton.Instance.GameManager.NetworkServer.GetUserDataByClientId(OwnerClientId);

            PlayerName.Value = userData.userName;
            PlayerColorIndex.Value = userData.userColorIndex;

            CheckAndResetsPlayerStats();
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
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
        Color hpBarColor = Color.Lerp(RedFF6666, Green99FF66, (float)hp.Value / maxHp.Value);
        hpBar.color = hpBarColor;
        currentHpText.color = hpBarColor;

        #region OverHead & Screen UI
        // Update UI Bar
        float targetFillAmount = (float)hp.Value / maxHp.Value;
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);

        // Update UI Text
        currentHpText.text = hp.Value + "/" + maxHp.Value;
        levelText.text = "Lv." + level.Value.ToString();
        #endregion
    }

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

    #region Check & Reset player stats
    private void CheckAndResetsPlayerStats()
    {
        maxHp.Value = PlayerVit * statsConverter;
        hp.Value = maxHp.Value;
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

        int newHealth = hp.Value + amount;
        hp.Value = Mathf.Clamp(newHealth, 0, maxHp.Value);

        if (hp.Value <= 0)
        {
            hp.Value = 0;
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