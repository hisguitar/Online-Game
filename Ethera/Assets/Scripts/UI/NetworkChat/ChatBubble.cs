using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
	[SerializeField] private SpriteRenderer background;
	[SerializeField] private TMP_Text messageText;
	[SerializeField] private Vector2 bubbleSizeOffset = new(0.2f, 0.1f);
	
	private void OnEnable()
	{
		UpdateBackgroundSize();
	}
	
	public void SetText(string text)
	{
		messageText.text = text;
		UpdateBackgroundSize();
	}
	
	private void UpdateBackgroundSize()
	{
		// Force update the text mesh to ensure accurate size calculation
		messageText.ForceMeshUpdate();
		
		// Get width and height of the rendered text
		float textWidth = messageText.renderedWidth;
		float textHeight = messageText.renderedHeight;
		
		// Set size of background to match textWidth & textHeight
		background.size = new Vector2(textWidth, textHeight) + bubbleSizeOffset;
	}
}