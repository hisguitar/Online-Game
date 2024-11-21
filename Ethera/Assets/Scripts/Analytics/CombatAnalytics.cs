using UnityEngine;
using Unity.Services.Analytics;

public class CombatAnalytics : MonoBehaviour
{
	public readonly float countDownTime = 6f;
	
	private float combatStartTime;
	private float lastKillTime;
	private bool inCombat = false;

	private void Update()
	{
		// 5 seconds after killing the last monster
		if (inCombat && Time.time - lastKillTime > countDownTime)
		{
			EndCombatSession();
		}
	}
	
	// Use this function when enemy died.
	public void RecordHitOnMonster()
	{
		if (!inCombat)
		{
			inCombat = true;
			combatStartTime = Time.time;
		}
		// count down {countDown} seconds since {lastKillTime}
		lastKillTime = Time.time;
	}
	
	// This function will automatic called when countDownTime is out!
	private void EndCombatSession()
	{
		inCombat = false;
		float combatDuration = Time.time - combatStartTime;

		/// Record analytics
		/// Event name is "combatSession"
		CustomEvent combatEvent = new CustomEvent("combatSession")
		{
			{"duration", combatDuration},
			{"gTimestamp", System.DateTime.UtcNow.ToString()}
		};
		AnalyticsService.Instance.RecordEvent(combatEvent);
		Debug.Log($"Combat session ended. Duration: {combatDuration} seconds.");
	}
}