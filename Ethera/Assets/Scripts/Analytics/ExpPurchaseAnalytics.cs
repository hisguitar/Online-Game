using UnityEngine;
using Unity.Services.Analytics;
using System.Collections.Generic;

public class ExpPurchaseAnalytics : MonoBehaviour
{
	private Dictionary<string, int> purchaseCounts;

	private void Awake()
	{
		purchaseCounts = new Dictionary<string, int>()
		{
			/// Spaces will be removed in RecordPurchaseEvent function
			/// Event name is..
			/// buy100Exp
			/// buy250Exp
			/// buy600Exp
			{ "100 Exp", 0 },
			{ "250 Exp", 0 },
			{ "600 Exp", 0 }
		};
	}

	public void RecordExpPurchase(string itemName)
	{
		if (purchaseCounts.ContainsKey(itemName))
		{
			purchaseCounts[itemName]++;
			RecordPurchaseEvent(itemName, purchaseCounts[itemName]);
			Debug.Log($"Buy {itemName}, Total Count: {purchaseCounts[itemName]}");
		}
		else
		{
			Debug.Log("This item has never been in game.");
		}
	}

	private void RecordPurchaseEvent(string itemName, int count)
	{
        /// About event name
        /// new CustomEvent($"buy{itemName.Replace(" ", "")}") = is event name
        /// for example, "buy100 Exp" it will get a string name from the Awake function.
        /// then remove space by command ".Replace(" ", "")"
        /// so, it will be "buy100Exp"
		CustomEvent purchaseEvent = new CustomEvent($"buy{itemName.Replace(" ", "")}")
		{
			{ "itemName", itemName },
			{ "count", count }
		};
		AnalyticsService.Instance.RecordEvent(purchaseEvent);
	}
}