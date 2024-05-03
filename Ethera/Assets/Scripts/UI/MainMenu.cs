using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Button clientButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 12;

    private void Update()
    {
        HandleCodeChanged();
    }

    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }

    // Check text.Length in joinCodeField
    public void HandleCodeChanged()
    {
        clientButton.interactable =
            joinCodeField.text.Length >= minNameLength &&
            joinCodeField.text.Length <= maxNameLength;
    }
}