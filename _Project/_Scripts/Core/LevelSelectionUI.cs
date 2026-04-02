using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectionUI : MonoBehaviour
{
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private int totalLevels = 10;

    void Start()
    {
        GenerateButtons();
    }

    private void GenerateButtons()
    {
        for (int i = 0; i < totalLevels; i++)
        {
            int levelIndex = i;
            bool unlocked = ProgressManager.IsLevelUnlocked(levelIndex);
            bool completed = ProgressManager.IsLevelCompleted(levelIndex);

            GameObject btnObj = Instantiate(levelButtonPrefab, buttonContainer);
            Button btn = btnObj.GetComponent<Button>();
            TextMeshProUGUI text = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null) text.text = $"Уровень {levelIndex + 1}";

            btn.interactable = unlocked;
            if (completed)
            {
                // Добавить звёздочку или галочку
                Image star = btnObj.transform.Find("Star")?.GetComponent<Image>();
                if (star != null) star.gameObject.SetActive(true);
            }

            btn.onClick.AddListener(() => LevelSelection.LoadLevel(levelIndex));
        }
    }
}