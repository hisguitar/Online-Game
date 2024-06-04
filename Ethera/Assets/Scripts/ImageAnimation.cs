using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageAnimation : MonoBehaviour
{
    public Image image;
    public Sprite[] animationSprite;
    public float animationSpeed;

    private int indexSprite;
    private Coroutine coroutineAnimation;
    bool IsDone;

    private void Start()
    {
        PlayAnimation();
    }

    // This is function to play/stop animation
    public void PlayAnimation()
    {
        IsDone = false;
        StartCoroutine(Animation());
    }

    public void StopAnimation()
    {
        IsDone = true;
        StopCoroutine(Animation());
    }

    // This below is how animation works
    private IEnumerator Animation()
    {
        yield return new WaitForSeconds(animationSpeed);
        if (indexSprite >= animationSprite.Length)
        {
            indexSprite = 0;
        }
        image.sprite = animationSprite[indexSprite];
        indexSprite += 1;
        if (IsDone == false)
        {
            coroutineAnimation = StartCoroutine(Animation());
        }
    }
}