using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceManager : MonoBehaviour
{
    public GameObject choicePanel;
    public Button choice1Button;
    public Button choice2Button;
    public Button choice3Button; // 추가된 버튼
    public TextMeshProUGUI choice1Text;
    public TextMeshProUGUI choice2Text;
    public TextMeshProUGUI choice3Text; // 추가된 텍스트

    private System.Action<int> onChoiceSelected;

    void Start()
    {
        choice1Button.onClick.AddListener(() => OnChoiceSelected(0));
        choice2Button.onClick.AddListener(() => OnChoiceSelected(1));
        choice3Button.onClick.AddListener(() => OnChoiceSelected(2)); // 추가된 리스너
    }

    public void ShowChoices(string[] choices, System.Action<int> onChoiceSelected)
    {
        this.onChoiceSelected = onChoiceSelected;

        choice1Button.gameObject.SetActive(choices.Length > 0);
        choice2Button.gameObject.SetActive(choices.Length > 1);
        choice3Button.gameObject.SetActive(choices.Length > 2); // 세 번째 버튼 활성화

        if (choices.Length > 0) choice1Text.text = choices[0];
        if (choices.Length > 1) choice2Text.text = choices[1];
        if (choices.Length > 2) choice3Text.text = choices[2]; // 세 번째 선택지 텍스트 설정

        choicePanel.SetActive(true);
    }

    void OnChoiceSelected(int choiceIndex)
    {
        choicePanel.SetActive(false);
        onChoiceSelected?.Invoke(choiceIndex);
    }
}
