using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Cinemachine;
using Unity.Collections;

public class Player : NetworkBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private NetworkVariable<int> maxHp = new();
    [SerializeField] private NetworkVariable<int> hp = new();
    public int PlayerStr { get; private set; } = 10;
    public int PlayerVit { get; private set; } = 10;
    public int PlayerAgi { get; private set; } = 3;

    [Header("Overhead UI Reference")]
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text currentHpText;
    [SerializeField] private GameObject floatingTextPrefab;

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    [Header("Settings")]
    [SerializeField] private int ownerPriority = 15;
    public NetworkVariable<FixedString32Bytes> PlayerName = new();
    public NetworkVariable<int> PlayerColorIndex = new();

    private bool isDead;
    private readonly int statsConvert = 10;
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

            // Need to fix
            maxHp.Value = PlayerVit * statsConvert;
            hp.Value = maxHp.Value;
        }

        if (IsOwner)
        {
            virtualCamera.Priority = ownerPriority;
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
        Color red = new(1f, 0.4f, 0.4f); //FF6666
        Color green = new(0.6f, 1.0f, 0.4f); //99FF66
        Color hpBarColor = Color.Lerp(red, green, (float)hp.Value / maxHp.Value);
        hpBar.color = hpBarColor;
        currentHpText.color = hpBarColor;

        #region OverHead & Screen UI
        // Update UI Bar
        float targetFillAmount = (float)hp.Value / maxHp.Value;
        hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, targetFillAmount, lerpSpeed * Time.deltaTime);
        hpBar.fillAmount = Mathf.Clamp01(hpBar.fillAmount);

        // Update UI Text
        currentHpText.text = hp.Value + "/" + maxHp.Value;
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

    // This method can be used for both increasing and decreasing HP
    private void ModifyHealth(int amount)
    {
        if (isDead) { return; }

        int newHealth = hp.Value + amount;
        hp.Value = Mathf.Clamp(newHealth, 0, maxHp.Value);
        if (floatingTextPrefab != null)
        {
            ShowFloatingTextClientRpc(amount.ToString());
        }

        if (hp.Value <= 0)
        {
            hp.Value = 0;
            isDead = true;
        }
    }
    [ClientRpc]
    private void ShowFloatingTextClientRpc(string text)
    {
        ShowFloatingText(text);
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