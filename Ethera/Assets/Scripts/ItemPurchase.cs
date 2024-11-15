using Unity.Netcode;
using UnityEngine.UI;

public class ItemPurchase : NetworkBehaviour
{
	public Image notificationsBackground;
	public Image notificationsBanner;
	private ulong playerID;
	
	private void Start()
	{
		playerID = this.OwnerClientId;
	}
	
	#region Item for sale
	public void BuyExp(int expAmout)
	{
		if (!IsServer) return;
		
		if (NetworkManager.Singleton.ConnectedClients.TryGetValue(playerID, out var client) &&
			client.PlayerObject.TryGetComponent<PlayerHealth>(out var player))
		{
			player.GainExp(expAmout);
			
			// Open 'Item Purchase Successful!' banner
			notificationsBackground.gameObject.SetActive(true);
			StartCoroutine(ExtensionMethods.ExpandBannerHeight(notificationsBanner, 0, 60, 0.4f));
		}
	}
	#endregion
}