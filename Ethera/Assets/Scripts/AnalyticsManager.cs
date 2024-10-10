using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;

public class AnalyticsManager : MonoBehaviour
{
	// count parameters in unity analytics
	private int purchasesOneHundredExpCount = 0; // 100 exp
	private int purchasesTwoHundredFiftyExpCount = 0; // 250 exp
	private int purchasesSixHundredExpCount = 0; // 600 exp
	
	private async void Start()
	{
		await UnityServices.InitializeAsync();
		AnalyticsService.Instance.StartDataCollection();
	}
	
	#region Exp purchase
	public void Buy100Exp()
	{
		purchasesOneHundredExpCount++;
		
		CustomEvent oneHundredExp = new CustomEvent("buy100Exp")
		{
			{"itemName", "100 Exp"},
			{"count", purchasesOneHundredExpCount}
		};
		
		AnalyticsService.Instance.RecordEvent(oneHundredExp);
		Debug.Log("Buy 100 Exp, Total Count: " + purchasesOneHundredExpCount);
	}
	
	public void Buy250Exp()
	{
		purchasesTwoHundredFiftyExpCount++;
		
		CustomEvent twoHundredFiftyExp = new CustomEvent("buy250Exp")
		{
			{"itemName", "250 Exp"},
			{"count", purchasesTwoHundredFiftyExpCount}
		};
		
		AnalyticsService.Instance.RecordEvent(twoHundredFiftyExp);
		Debug.Log("Buy 250 Exp, Total Count: " + purchasesTwoHundredFiftyExpCount);
	}
	
	public void Buy600Exp()
	{
		purchasesSixHundredExpCount++;
		
		CustomEvent sixHundredExp = new CustomEvent("buy600Exp")
		{
			{"itemName", "600 Exp"},
			{"count", purchasesSixHundredExpCount}
		};
		
		AnalyticsService.Instance.RecordEvent(sixHundredExp);
		Debug.Log("Buy 600 Exp, Total Count: " + purchasesSixHundredExpCount);
	}
	#endregion
}