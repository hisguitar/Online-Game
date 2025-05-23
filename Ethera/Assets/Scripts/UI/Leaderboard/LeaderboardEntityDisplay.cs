using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class LeaderboardEntityDisplay : MonoBehaviour
{
	[SerializeField] private TMP_Text rankText;
	[SerializeField] private TMP_Text nameText;
	[SerializeField] private TMP_Text expText;
	[SerializeField] private Color myColor;

	private FixedString32Bytes playerName;

	public ulong ClientId { get; private set; }
	public int Exp { get; private set; }

	public void Initialise(ulong clientId, FixedString32Bytes playerName, int exp)
	{
		ClientId = clientId;
		this.playerName = playerName;
		UpdateExp(exp);

		if(clientId == NetworkManager.Singleton.LocalClientId)
		{
			rankText.color = myColor;
			nameText.color = myColor;
			expText.color = myColor;
		}
	}

	public void UpdateExp(int exp)
	{
		Exp = exp;
		UpdateText();
	}

	public void UpdateText()
	{
		rankText.text = $"{transform.GetSiblingIndex()+1}";
		nameText.text = $"{playerName}";
		expText.text = $"{Exp}";
	}
}