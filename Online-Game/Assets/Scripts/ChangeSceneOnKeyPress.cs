using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnKeyPress : MonoBehaviour
{
    public string sceneName; // Name of the scene to load
    public Animator animator; // Animator that you want to control by this script

    private void Update()
    {
        // Check if any key is pressed
        if (Input.anyKeyDown)
        {
            // Play animation
            animator.SetTrigger("ChangeScene");
        }
    }

    // Change scene method is used in 'Press any key to start' animation
    private void ChangeScene()
    {
        // Change scene
        SceneManager.LoadScene(sceneName);
    }
}