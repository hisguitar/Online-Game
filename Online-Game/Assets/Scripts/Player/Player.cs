using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class Player : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color redFF5858 = new(1f, 0.345f, 0.345f);
    [SerializeField] private Color greenCFFF57 = new(0.78f, 1f, 0.341f);

    [Header("Player Stats")]
    [SerializeField] private NetworkVariable<int> maxHp = new();
    [SerializeField] private NetworkVariable<int> hp = new();
    public int PlayerStr { get; private set; } = 20;
    public int PlayerVit { get; private set; } = 10;

    [Header("Reference")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text nameText;

    private bool isDead;
    private readonly int statsConvert = 10;
    private readonly float lerpSpeed = 3f;

    // OnNetworkSpawn is used when an object begins network connection.
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            maxHp.Value = PlayerVit * statsConvert;
            hp.Value = maxHp.Value;
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
        // Change color hp bar
        Color hpBarColor = Color.Lerp(redFF5858, greenCFFF57, (float)hp.Value / maxHp.Value);
        hpBar.color = hpBarColor;

        // Fill hp bar
        float targetFillAmount = (float)hp.Value / maxHp.Value;
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);

        // Update UI Text
        hpText.text = hp.Value + "/" + maxHp.Value;
    }

    #region Take Damage
    public void TakeDamage(int amount)
    {
        ModifyHealth(-amount);
    }

    public void RestoreHealth(int amount)
    {
        ModifyHealth(amount);
    }

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
}