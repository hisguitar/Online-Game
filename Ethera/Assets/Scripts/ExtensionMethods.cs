using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class ExtensionMethods
{
	public static IEnumerator ExpandBannerHeight(Image bannerImage, float startHeight, float endHeight, float duration)
	{
		float elapsed = 0f;
		RectTransform rt = bannerImage.GetComponent<RectTransform>();
		
		while (elapsed < duration)
		{
			elapsed += Time.deltaTime;
			float newHeight = Mathf.Lerp(startHeight, endHeight, elapsed / duration);
			rt.sizeDelta = new Vector2(rt.sizeDelta.x, newHeight);
			yield return null;
		}
		
		// Ensure final height is set
		rt.sizeDelta = new Vector2(rt.sizeDelta.x, endHeight);
	}
}