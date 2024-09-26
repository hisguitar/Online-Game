using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class ChangeSceneOnKeyPress : MonoBehaviour
{
	[SerializeField] private string sceneName;
	[SerializeField] [Tooltip("Animator that you want to do something before changing scenes.")] private Animator animator;

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (Input.anyKeyDown)
		{
			animator.SetTrigger("ChangeScene");
		}
	}

	// Change scene method is used in 'Animation'
	private void ChangeScene()
	{
		SceneManager.LoadScene(sceneName);
	}
}