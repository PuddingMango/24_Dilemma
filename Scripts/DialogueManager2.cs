using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class DialogueManager2 : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    public SceneController sceneController;
    public ChoiceManager choiceManager;

    public string dialogueFileName = "dialogues2.txt"; // Scene2에서 사용할 txt 파일 이름

    private Dictionary<string, DialogueSection2> dialogues = new Dictionary<string, DialogueSection2>();
    private string currentLabel = "start";
    private int currentDialogueIndex = 0;

    private string currentCharacter = "";  // 현재 캐릭터를 추적

    void Start()
    {
        LoadDialogues();
        StartCoroutine(StartDialogue()); // 텍스트 진행을 자동으로 시작
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && choiceManager.choicePanel.activeSelf == false)
        {
            ShowNextDialogue();
        }
    }

    void LoadDialogues()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, dialogueFileName);
        filePath = filePath.Replace("\\", "/");  // 경로 수정
        Debug.Log("Attempting to load file from: " + filePath);

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            ParseDialogueLines(lines);
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }
    }

    void ParseDialogueLines(string[] lines)
    {
        string currentLabel = "";
        DialogueSection2 currentSection = null;

        foreach (string line in lines)
        {
            Debug.Log("Processing line: " + line);

            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("LABEL="))
            {
                if (currentSection != null && !string.IsNullOrEmpty(currentLabel))
                {
                    dialogues[currentLabel] = currentSection;
                    Debug.Log("Added section: " + currentLabel);
                }

                currentLabel = line.Replace("LABEL=", "").Trim();
                currentSection = new DialogueSection2();
                Debug.Log("New section: " + currentLabel);
            }
            else if (line.StartsWith("BACKGROUND="))
            {
                if (currentSection != null)
                {
                    string background = line.Replace("BACKGROUND=", "").Trim();
                    currentSection.Dialogues.Add(new Dialogue2 { Background = background });
                    Debug.Log("Set background: " + background);
                }
            }
            else if (line.StartsWith("CHARACTER="))
            {
                if (currentSection != null)
                {
                    string character = line.Replace("CHARACTER=", "").Trim();
                    currentSection.Dialogues.Add(new Dialogue2 { Character = character });
                    Debug.Log("Set character: " + character);
                }
            }
            else if (line.StartsWith("DIALOGUE="))
            {
                if (currentSection != null)
                {
                    string dialogueText = line.Replace("DIALOGUE=", "").Trim();
                    if (currentSection.Dialogues.Count > 0 && string.IsNullOrEmpty(currentSection.Dialogues[currentSection.Dialogues.Count - 1].Text))
                    {
                        currentSection.Dialogues[currentSection.Dialogues.Count - 1].Text = dialogueText;
                    }
                    else
                    {
                        currentSection.Dialogues.Add(new Dialogue2 { Text = dialogueText });
                    }
                    Debug.Log("Added dialogue: " + dialogueText);
                }
            }
            else if (line.StartsWith("CHOICE="))
            {
                if (currentSection != null)
                {
                    string[] choiceParts = line.Replace("CHOICE=", "").Split('|');
                    currentSection.Choices.Add(new Choice2
                    {
                        Text = choiceParts[0].Trim(),
                        TargetLabel = choiceParts[1].Trim()
                    });
                    Debug.Log("Added choice: " + choiceParts[0].Trim() + " -> " + choiceParts[1].Trim());
                }
            }
            else if (line.StartsWith("ITEM="))
            {
                if (currentSection != null)
                {
                    string item = line.Replace("ITEM=", "").Trim();
                    currentSection.Dialogues.Add(new Dialogue2 { Item = item });
                    Debug.Log("Set item: " + item);
                }
            }
        }

        if (currentSection != null && !string.IsNullOrEmpty(currentLabel))
        {
            dialogues[currentLabel] = currentSection;
            Debug.Log("Added final section: " + currentLabel);
        }
    }

    IEnumerator StartDialogue()
    {
        yield return new WaitForEndOfFrame(); // 씬 로드 후 한 프레임 대기
        ShowNextDialogue(); // 텍스트 진행을 시작합니다.
    }

    void ShowNextDialogue()
    {
        if (dialogues.ContainsKey(currentLabel))
        {
            DialogueSection2 currentSection = dialogues[currentLabel];

            if (currentDialogueIndex == 0)
            {
                sceneController.HideEvidence(); // 새로운 라벨 시작 시 증거물 숨기기
            }

            if (currentDialogueIndex < currentSection.Dialogues.Count)
            {
                Dialogue2 currentDialogue = currentSection.Dialogues[currentDialogueIndex];

                if (!string.IsNullOrEmpty(currentDialogue.Background))
                {
                    sceneController.ChangeBackground(currentDialogue.Background);
                }

                if (!string.IsNullOrEmpty(currentDialogue.Character))
                {
                    currentCharacter = currentDialogue.Character;
                    sceneController.ChangeCharacter(currentDialogue.Character);
                }
                else if (!string.IsNullOrEmpty(currentCharacter))
                {
                    sceneController.ChangeCharacter(currentCharacter);  // 현재 캐릭터 유지
                }

                if (!string.IsNullOrEmpty(currentDialogue.Item))
                {
                    sceneController.ShowEvidence(currentDialogue.Item);
                }

                dialogueText.text = currentDialogue.Text;

                currentDialogueIndex++;
            }
            else if (currentSection.Choices.Count > 0)
            {
                List<string> choicesText = new List<string>();
                List<string> choiceTargets = new List<string>();

                foreach (Choice2 choice in currentSection.Choices)
                {
                    choicesText.Add(choice.Text);
                    choiceTargets.Add(choice.TargetLabel);
                }

                choiceManager.ShowChoices(choicesText.ToArray(), index => OnChoiceMade(choiceTargets[index]));
            }
            else
            {
                dialogueBox.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Label not found: " + currentLabel);
        }
    }

    void OnChoiceMade(string choiceTarget)
    {
        currentLabel = choiceTarget;
        currentDialogueIndex = 0;
        ShowNextDialogue();
    }
}

[System.Serializable]
public class DialogueSection2
{
    public List<Dialogue2> Dialogues = new List<Dialogue2>();
    public List<Choice2> Choices = new List<Choice2>();
}

[System.Serializable]
public class Dialogue2
{
    public string Character;
    public string Background;
    public string Text;
    public string Item; // 증거물 항목 추가
}

[System.Serializable]
public class Choice2
{
    public string Text;
    public string TargetLabel;
}
