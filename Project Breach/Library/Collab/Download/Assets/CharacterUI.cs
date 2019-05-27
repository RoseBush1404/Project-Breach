using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour {

    [SerializeField] Button[] buttons;
    [SerializeField] Image characterProfile;

    PlayerController playerController;
    [SerializeField] Character selectedCharcter;

	void Start ()
    {
        playerController = FindObjectOfType<PlayerController>();
        ResetUI();
	}

    public void ResetUI()
    {
        characterProfile.color = Color.white;

        foreach (Button button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void DisplayCharacterUI(Character character)
    {
        ResetUI();
        selectedCharcter = character;
        UpdateCharacterProfile(character);
        UpdateCharacterAbilites(character);
    }

    private void UpdateCharacterProfile(Character character)
    {
        // TODO change to image, not color
        Color32 characterColor = character.GetCharacterColor();
        characterProfile.color = characterColor;
    }

    private void UpdateCharacterAbilites(Character character)
    {
        Abilities abilityComponent = character.GetComponent<Abilities>();
        int numOfAbilitys = abilityComponent.GetNumberOfAbilities();

        for (int i = 0; i < numOfAbilitys; i++)
        {
            buttons[i].gameObject.SetActive(true);
            Color abilityColor = abilityComponent.GetAbilityAtIndex(i).GetAbilityColor();
            buttons[i].GetComponent<Image>().color = abilityColor;
        }
    }

    public void Ability0Pressed()
    {
        selectedCharcter.AbilityPressed(0);
    }

    public void Ability1Pressed()
    {
        selectedCharcter.AbilityPressed(1);
    }

    public void Ability2Pressed()
    {
        selectedCharcter.AbilityPressed(2);
    }
}
