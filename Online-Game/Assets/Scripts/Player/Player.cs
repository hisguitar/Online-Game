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

    [Header("Overhead UI Reference")]
    [SerializeField] private Image hpBarOverHead;
    [SerializeField] private TMP_Text currentHpTextOverHead;
    [SerializeField] private TMP_Text nameTextOverHead;

    [Header("UI Reference")]
    [SerializeField] private Image hpBar; // The problem is hpBar becomes owned by the last player join
    [SerializeField] private TMP_Text currentHpText; // The problem is hpBar becomes owned by the last player join

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
            TryReference();
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
        hpBar.color = hpBarColor;

        #region OverHead & Screen UI
        // Update UI Bar
        float targetFillAmount = (float)hp.Value / maxHp.Value;
        hpBarOverHead.fillAmount = Mathf.Lerp(hpBarOverHead.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        hpBarOverHead.fillAmount = Mathf.Clamp01(hpBarOverHead.fillAmount);
        hpBar.fillAmount = hpBarOverHead.fillAmount;

        // Update UI Text
        currentHpTextOverHead.text = hp.Value + "/" + maxHp.Value;
        currentHpText.text = currentHpTextOverHead.text;
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

        if (hp.Value <= 0)
        {
            hp.Value = 0;
            isDead = true;
        }
    }
    #endregion

    #region Try to reference game object
    private void TryReference()
    {
        // Find the GameObject named "HpBar" under the current GameObject
        GameObject hpBarGameObject = GameObject.Find("HpBar");
        GameObject currentHpTextGameObject = GameObject.Find("CurrentHpText");

        // Check if GameObject is found.
        if (hpBarGameObject && currentHpTextGameObject != null)
        {
            // Get the Image component from the found GameObject
            hpBar = hpBarGameObject.GetComponent<Image>();
            currentHpText = currentHpTextGameObject.GetComponent<TMP_Text>();
        }
        else
        {
            Debug.LogWarning("The GameObject was not found.");
        }
    }
    #endregion
}