using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox;
    public SceneController sceneController;
    public ChoiceManager choiceManager;

    private Dictionary<string, DialogueSection> dialogues = new Dictionary<string, DialogueSection>();
    private string currentLabel = "start";
    private int currentDialogueIndex = 0;

    private bool isJungunDialogueCompleted = false;
    private bool isYanggunDialogueCompleted = false;
    private string currentCharacter = "";  // 현재 캐릭터를 추적

    void Start()
    {
        LoadDialogues();
        ShowNextDialogue();
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
        string filePath = Path.Combine(Application.streamingAssetsPath, "dialogues.txt");
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
        DialogueSection currentSection = null;

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
                currentSection = new DialogueSection();
                Debug.Log("New section: " + currentLabel);
            }
            else if (line.StartsWith("BACKGROUND="))
            {
                if (currentSection != null)
                {
                    string background = line.Replace("BACKGROUND=", "").Trim();
                    currentSection.Dialogues.Add(new Dialogue { Background = background });
                    Debug.Log("Set background: " + background);
                }
            }
            else if (line.StartsWith("CHARACTER="))
            {
                if (currentSection != null)
                {
                    string character = line.Replace("CHARACTER=", "").Trim();
                    currentSection.Dialogues.Add(new Dialogue { Character = character });
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
                        currentSection.Dialogues.Add(new Dialogue { Text = dialogueText });
                    }
                    Debug.Log("Added dialogue: " + dialogueText);
                }
            }
            else if (line.StartsWith("CHOICE="))
            {
                if (currentSection != null)
                {
                    string[] choiceParts = line.Replace("CHOICE=", "").Split('|');
                    currentSection.Choices.Add(new Choice
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
                    currentSection.Dialogues.Add(new Dialogue { Item = item });
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

    void ShowNextDialogue()
    {
        if (dialogues.ContainsKey(currentLabel))
        {
            DialogueSection currentSection = dialogues[currentLabel];

            if (currentDialogueIndex == 0)
            {
                sceneController.HideEvidence(); // 새로운 라벨 시작 시 증거물 숨기기
            }

            if (currentDialogueIndex < currentSection.Dialogues.Count)
            {
                Dialogue currentDialogue = currentSection.Dialogues[currentDialogueIndex];

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

                if (currentLabel == "askJungun" && currentDialogueIndex == currentSection.Dialogues.Count - 1)
                {
                    isJungunDialogueCompleted = true;
                }

                if (currentLabel == "askYanggun" && currentDialogueIndex == currentSection.Dialogues.Count - 1)
                {
                    isYanggunDialogueCompleted = true;
                }

                currentDialogueIndex++;
            }
            else if (currentSection.Choices.Count > 0)
            {
                List<string> choicesText = new List<string>();
                List<string> choiceTargets = new List<string>();

                foreach (Choice choice in currentSection.Choices)
                {
                    if (choice.TargetLabel == "checkScene3")
                    {
                        if (isJungunDialogueCompleted && isYanggunDialogueCompleted)
                        {
                            choicesText.Add(choice.Text);
                            choiceTargets.Add(choice.TargetLabel);
                        }
                    }
                    else if (choice.TargetLabel == "askJungun" || choice.TargetLabel == "askYanggun")
                    {
                        if (!isJungunDialogueCompleted && choice.TargetLabel == "askJungun")
                        {
                            choicesText.Add(choice.Text);
                            choiceTargets.Add(choice.TargetLabel);
                        }
                        if (!isYanggunDialogueCompleted && choice.TargetLabel == "askYanggun")
                        {
                            choicesText.Add(choice.Text);
                            choiceTargets.Add(choice.TargetLabel);
                        }
                    }
                    else
                    {
                        choicesText.Add(choice.Text);
                        choiceTargets.Add(choice.TargetLabel);
                    }
                }

                choiceManager.ShowChoices(choicesText.ToArray(), index => OnChoiceMade(choiceTargets[index]));
            }
            else
            {
                dialogueBox.SetActive(false);
                // 모든 대화와 선택지가 끝났을 때 Scene2로 전환
                SceneManager.LoadScene("Scene2");
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
public class DialogueSection
{
    public List<Dialogue> Dialogues = new List<Dialogue>();
    public List<Choice> Choices = new List<Choice>();
}

[System.Serializable]
public class Dialogue
{
    public string Character;
    public string Background;
    public string Text;
    public string Item; // 증거물 항목 추가
}

[System.Serializable]
public class Choice
{
    public string Text;
    public string TargetLabel;
}
