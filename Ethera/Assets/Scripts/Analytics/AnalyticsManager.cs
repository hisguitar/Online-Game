// Need to be singleton class, because i need to call this anywhere.
public class AnalyticsManager : SingletonPersistent<AnalyticsManager>
{
	public ExpPurchaseAnalytics expPurchaseAnalytics;
	public CombatAnalytics combatAnalytics;
	//public IdleAnalytics idleAnalytics;
	//public KillStreakAnalytics killStreakAnalytics;

	public void RecordExpPurchase(string itemName) => expPurchaseAnalytics.RecordExpPurchase(itemName);
	public void RecordCombatHit() => combatAnalytics.RecordHitOnMonster();
	//public void RecordIdleEnd() => idleAnalytics.EndIdleSession();
	//public void RecordPlayerKill() => killStreakAnalytics.RecordKill();
}