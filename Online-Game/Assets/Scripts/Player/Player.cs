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
    public int PlayerStr { get; private set; } = 9;
    public int PlayerVit { get; private set; } = 6;
    public int PlayerAgi { get; private set; } = 3;

    [Header("Overhead UI Reference")]
    [SerializeField] private NetworkObject player;
    [SerializeField] private Image hpBarOverHead;
    [SerializeField] private TMP_Text currentHpTextOverHead;
    [SerializeField] private TMP_Text nameTextOverHead;
    [SerializeField] private GameObject floatingTextPrefab;

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

            // player.OwnerClientId start with 0
            nameTextOverHead.text = "Player." + player.OwnerClientId.ToString();
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
        Color hpBarColor = Color.Lerp(redFF5858, greenCFFF57, (float)hp.Value / maxHp.Value);
        hpBarOverHead.color = hpBarColor;

        #region OverHead & Screen UI
        // Update UI Bar
        float targetFillAmount = (float)hp.Value / maxHp.Value;
        hpBarOverHead.fillAmount = Mathf.Lerp(hpBarOverHead.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        hpBarOverHead.fillAmount = Mathf.Clamp01(hpBarOverHead.fillAmount);

        // Update UI Text
        currentHpTextOverHead.text = hp.Value + "/" + maxHp.Value;
        #endregion
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
        if (floatingTextPrefab != null)
        {
            ShowFloatingText($"-{amount}");
        }

        if (hp.Value <= 0)
        {
            hp.Value = 0;
            isDead = true;
        }
    }
    #endregion

    #region Show Floating Text
    private void ShowFloatingText(string text)
    {
        GameObject go = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        go.transform.SetParent(transform);
        go.GetComponent<TMP_Text>().text = text;
    }
    #endregion
}