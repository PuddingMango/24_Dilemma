/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image를 사용하기 위해 필요

public class SceneController : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer characterRenderer;
    public Image characterNameImage; // 이름표 이미지 객체 추가
    public List<Sprite> backgrounds;
    public List<Sprite> characters;
    public GameObject blackBoxPanel; // 검은색 상자를 위한 패널

    private Dictionary<string, Sprite> backgroundDict;
    private Dictionary<string, Sprite> characterDict;
    private Dictionary<string, Vector3> characterPositions;

    void Start()
    {
        // Initialize the dictionaries
        backgroundDict = new Dictionary<string, Sprite>();
        characterDict = new Dictionary<string, Sprite>();
        characterPositions = new Dictionary<string, Vector3>();

        // Populate the background dictionary
        foreach (Sprite bg in backgrounds)
        {
            if (bg != null) // Ensure the sprite is not null
            {
                backgroundDict[bg.name] = bg;
            }
        }

        // Populate the character dictionary
        foreach (Sprite character in characters)
        {
            if (character != null) // Ensure the sprite is not null
            {
                characterDict[character.name] = character;
            }
        }

        // Define character positions
        characterPositions["unknown"] = new Vector3(0, -1, 0);
        characterPositions["jungun"] = new Vector3(-4, -1, 0);  // Left side
        characterPositions["yanggun"] = new Vector3(4, -1, 0);  // Right side
    }

    public void ChangeBackground(string backgroundName)
    {
        if (backgroundDict.ContainsKey(backgroundName))
        {
            backgroundRenderer.sprite = backgroundDict[backgroundName];
            Debug.Log("Changed background to: " + backgroundName);
        }
        else
        {
            Debug.LogWarning("Background not found: " + backgroundName);
        }
    }

    public void ChangeCharacter(string characterName)
    {
        if (characterDict.ContainsKey(characterName))
        {
            characterRenderer.sprite = characterDict[characterName];
            if (characterPositions.ContainsKey(characterName))
            {
                characterRenderer.transform.localPosition = characterPositions[characterName];
            }
            else
            {
                characterRenderer.transform.localPosition = Vector3.zero;  // Default position if not specified
            }
            characterRenderer.transform.localScale = new Vector3(0.3f, 0.3f, 0.6f); // Adjust scale as needed
            Debug.Log("Changed character to: " + characterName);
        }
        else
        {
            characterRenderer.sprite = null;
            Debug.LogWarning("Character not found: " + characterName);
        }

        // Update character name image
        UpdateCharacterNameImage(characterName);
    }

    void UpdateCharacterNameImage(string characterName)
    {
        string path = "CharacterNameImages/" + characterName;
        Sprite nameSprite = Resources.Load<Sprite>(path);
        if (nameSprite != null)
        {
            characterNameImage.sprite = nameSprite;
            characterNameImage.gameObject.SetActive(true);
        }
        else
        {
            characterNameImage.gameObject.SetActive(false);
        }
    }

    public void ShowBlackBox(bool show)
    {
        blackBoxPanel.SetActive(show);
    }
}
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image를 사용하기 위해 필요

public class SceneController : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public SpriteRenderer characterRenderer;
    public Image characterNameImage; // 이름표 이미지 객체 추가
    public Image evidenceImage; // 증거물 이미지를 표시할 UI 객체
    public List<Sprite> backgrounds;
    public List<Sprite> characters;
    public List<Sprite> evidenceItems; // 증거물 이미지 리스트 추가

    private Dictionary<string, Sprite> backgroundDict;
    private Dictionary<string, Sprite> characterDict;
    private Dictionary<string, Sprite> evidenceDict; // 증거물 딕셔너리 추가
    private Dictionary<string, Vector3> characterPositions;

    void Start()
    {
        // Initialize the dictionaries
        backgroundDict = new Dictionary<string, Sprite>();
        characterDict = new Dictionary<string, Sprite>();
        evidenceDict = new Dictionary<string, Sprite>();
        characterPositions = new Dictionary<string, Vector3>();

        // Populate the background dictionary
        foreach (Sprite bg in backgrounds)
        {
            if (bg != null) // Ensure the sprite is not null
            {
                backgroundDict[bg.name] = bg;
            }
        }

        // Populate the character dictionary
        foreach (Sprite character in characters)
        {
            if (character != null) // Ensure the sprite is not null
            {
                characterDict[character.name] = character;
            }
        }

        // Populate the evidence dictionary
        foreach (Sprite item in evidenceItems)
        {
            if (item != null) // Ensure the sprite is not null
            {
                evidenceDict[item.name] = item;
            }
        }

        // Define character positions
        characterPositions["unknown"] = new Vector3(0, -1, 0);
        characterPositions["jungun"] = new Vector3(-4, -1, 0);  // Left side
        characterPositions["yanggun"] = new Vector3(4, -1, 0);  // Right side
        characterPositions["professorYoon"] = new Vector3(0, -1, 0);  
    }

    public void ChangeBackground(string backgroundName)
    {
        if (backgroundDict.ContainsKey(backgroundName))
        {
            backgroundRenderer.sprite = backgroundDict[backgroundName];
            Debug.Log("Changed background to: " + backgroundName);
        }
        else
        {
            Debug.LogWarning("Background not found: " + backgroundName);
        }
    }

    public void ChangeCharacter(string characterName)
    {
        if (characterDict.ContainsKey(characterName))
        {
            characterRenderer.sprite = characterDict[characterName];
            if (characterPositions.ContainsKey(characterName))
            {
                characterRenderer.transform.localPosition = characterPositions[characterName];
            }
            else
            {
                characterRenderer.transform.localPosition = Vector3.zero;  // Default position if not specified
            }
            characterRenderer.transform.localScale = new Vector3(0.3f, 0.3f, 0.6f); // Adjust scale as needed
            Debug.Log("Changed character to: " + characterName);
        }
        else
        {
            characterRenderer.sprite = null;
            Debug.LogWarning("Character not found: " + characterName);
        }

        // Update character name image
        UpdateCharacterNameImage(characterName);
    }

    void UpdateCharacterNameImage(string characterName)
    {
        string path = "CharacterNameImages/" + characterName;
        Sprite nameSprite = Resources.Load<Sprite>(path);
        if (nameSprite != null)
        {
            characterNameImage.sprite = nameSprite;
            characterNameImage.gameObject.SetActive(true);
        }
        else
        {
            characterNameImage.gameObject.SetActive(false);
        }
    }

    public void ShowEvidence(string evidenceName)
    {
        if (evidenceDict.ContainsKey(evidenceName))
        {
            evidenceImage.sprite = evidenceDict[evidenceName];
            evidenceImage.gameObject.SetActive(true);
            Debug.Log("Showing evidence: " + evidenceName);
        }
        else
        {
            evidenceImage.gameObject.SetActive(false);
            Debug.LogWarning("Evidence not found: " + evidenceName);
        }
    }

    public void HideEvidence()
    {
        evidenceImage.gameObject.SetActive(false);
    }
}
