using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;
using System.Collections.Generic;

public class AnalyticsManager : MonoBehaviour
{
	[SerializeField] private NetworkChat networkChat;
	
	// count parameters in unity analytics
	private Dictionary<string, int> purchaseCounts = new Dictionary<string, int>()
	{
	{ "100 Exp", 0 },
	{ "250 Exp", 0 },
	{ "600 Exp", 0 }
	};
	
	private async void Start()
	{
		await UnityServices.InitializeAsync();
		AnalyticsService.Instance.StartDataCollection();
	}
	
	public void BuyExp(string itemName)
	{
		if (purchaseCounts.ContainsKey(itemName))
		{
			purchaseCounts[itemName]++;
			RecordPurchase(itemName, purchaseCounts[itemName]);
			Debug.Log($"Buy {itemName}, Total Count: {purchaseCounts[itemName]}");
 		}
		else
		{
			Debug.Log("This item has never been in game.");
		}
	}
	
	private void RecordPurchase(string itemName, int count)
	{
		CustomEvent purchaseEvent = new CustomEvent($"buy{itemName.Replace(" ", "")}")
		{
			{"itemName", itemName},
			{"count", count},
			{"playerName", networkChat.PlayerName}
		};
		AnalyticsService.Instance.RecordEvent(purchaseEvent);
	}
}