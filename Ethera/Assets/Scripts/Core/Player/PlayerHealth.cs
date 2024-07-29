using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine; 

public class PlayerHealth : NetworkBehaviour
{
	[Header("Player Data, Level, Exp")]
	public PlayerData playerData;
	public NetworkVariable<int> Level { get; private set; } = new(1);
	public NetworkVariable<int> Exp = new();
	public int ExpToLevelUp { get; private set; } = 100;

	// Player Status
	public NetworkVariable<int> MaxHp = new();
	public NetworkVariable<int> CurrentHp = new();
	public Action<PlayerHealth> OnDie;
	private ulong playerID;
	private bool isDead;
	
	[Header("Floating Text & Bubble Text")]
	[SerializeField] private GameObject floatingTextPrefab;
	[SerializeField] private GameObject bubbleText;
	[SerializeField] private float bubbleTextDuration = 5f;
	private Coroutine bubbleTextCoroutine;

	[Header("New Color")]
	public Color RedFF6666 = new(1f, 0.4f, 0.4f);
	public Color RedFF0D0D = new(1f, 0.051f, 0.051f);
	public Color Green99FF66 = new(0.6f, 1.0f, 0.4f);
	public Color YellowFFFF0D = new(1f, 1f, 0.051f);

	public override void OnNetworkSpawn()
	{
		if (!IsServer) return;
		CheckAndResetsPlayerStats();
	}
	
	private void Start()
	{
		bubbleText.SetActive(false);
	}

	#region Check & Reset player stats
	private void CheckAndResetsPlayerStats()
	{
		// Convert stats to use in game
		MaxHp.Value = playerData.playerVit * playerData.statusConverter;

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
			Level.Value++;
			Exp.Value -= ExpToLevelUp;
			ExpToLevelUp = CalculateExpToLevelUp();
			SoundManager.Instance.Play(SoundManager.SoundName.NewLevel);
			LevelUpRewards();

			// Show FloatingText
			if (floatingTextPrefab != null)
			{
				ShowFloatingTextClientRpc($"Level up to {Level.Value}!", YellowFFFF0D);
			}
		}
	}

	private int CalculateExpToLevelUp()
	{
		return 100 * Level.Value;
	}

	private void LevelUpRewards()
	{
		playerData.playerStr += 1;
		playerData.playerVit += 1;
		playerData.playerAgi += 0.1f;

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
		if (!IsServer) return;
		if (isDead) return;

		int newHealth = CurrentHp.Value + amount;
		CurrentHp.Value = Mathf.Clamp(newHealth, 0, MaxHp.Value);

		if (CurrentHp.Value == 0)
		{
			// Gain exp when killing other players.
			PlayerHealth player = NetworkManager.Singleton.ConnectedClients[playerID].PlayerObject.GetComponent<PlayerHealth>();
			if (player != null)
			{
				player.GainExp(playerData.EXPBounty);
			}

			// Player die process
			OnDie?.Invoke(this);
			isDead = true;
		}
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
		if (otherRigidbody.TryGetComponent(out DealDamageOnContact bullet))
		{
			playerID = bullet.OwnerClientId;
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
	#region Show Bubble Text
	[ServerRpc] // Send data to server
	public void ShowBubbleTextServerRpc(string text, ServerRpcParams serverRpcParams = default)
	{
		ShowBubbleTextClientRpc(text);
	}
	
	[ClientRpc] // Receive data from server
	public void ShowBubbleTextClientRpc(string text, ClientRpcParams clientRpcParams = default)
	{
		ShowBubbleText(text);
	}
	
	private void ShowBubbleText(string text)
	{		
		// If old message hasn't expired yet
		if (bubbleTextCoroutine != null)
		{
			StopCoroutine(bubbleTextCoroutine);
		}
		
		// Call SetText(text) in ChatBubble.cs then set BubbleTextPrefab active(true)
		bubbleText.GetComponent<ChatBubble>().SetText(text);
		bubbleText.SetActive(true);
		
		// Start new coroutine to deactivate bubble text
		bubbleTextCoroutine = StartCoroutine(DeactivateBubbleText(bubbleTextDuration));
	}
	
	private IEnumerator DeactivateBubbleText(float time)
	{
		yield return new WaitForSeconds(time);
		bubbleText.SetActive(false);
	}
	#endregion
}