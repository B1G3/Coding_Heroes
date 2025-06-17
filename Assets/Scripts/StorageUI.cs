using UnityEngine;
using TMPro;

public class StorageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI storageText;

    public void Initialize(StorageModule storageModule)
    {
        UpdateText(storageModule.GetStoredAmount(), storageModule.GetCapacity());
        storageModule.OnStorageChanged += UpdateText;
    }

    private void UpdateText(int current, int max)
    {
        storageText.text = $"{current} / {max}";
    }
}