using System.Collections;
using TMPro;
using UnityEngine;

public class TownChecker : MonoBehaviour
{
	public string townName;
	public TMP_Text textToFade;
	public float duration = 2f;
	
	private void Start()
	{
		SoundManager.Instance.Play(SoundManager.SoundName.WhiteMorning);
		
		StartCoroutine(FadeOutText());
	}
	
	IEnumerator FadeOutText()
    {
        Color originalColor = textToFade.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            textToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Setting alpha to be 0 to make sure fade successfully
        textToFade.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // after fade successfully let set active to be false
        textToFade.gameObject.SetActive(false);
    }
		
	// private void OnTriggerEnter2D(Collider2D other)
	// {
	// 	if (other.CompareTag("Player"))
	// 	{
	// 		if (gameObject.name == townName)
	// 		{
	// 			Debug.Log($"Player entered to {townName}");
	// 			SoundManager.Instance.Play(SoundManager.SoundName.WhiteMorning);
	// 		}
	// 	}
	// }
}