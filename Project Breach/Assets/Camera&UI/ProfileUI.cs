using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Placeable;
using Breach.Placeable.Characters;
using Breach.Placeable.Component;
using Breach.Controler;
using Breach.UI.ToolTip;

namespace Breach.UI
{
    public class ProfileUI : MonoBehaviour
    { // TODO change to image, not color

        [SerializeField] Button[] buttons;
        [SerializeField] Image characterProfile;
        [SerializeField] Sprite defaultProfileImage;

        GameObject selectedGO;
        GameObject toolTip;

        public void Init()
        {
            ResetUI();
        }

        public void ResetUI()
        {
            selectedGO = null;
            characterProfile.sprite = defaultProfileImage;
            RemoveToolTip();

            foreach (Button button in buttons)
            {
                button.gameObject.SetActive(false);
            }
        }

        public void DisplayProfileUI(GameObject selectable)
        {
            ResetUI();
            selectedGO = selectable;
            UpdateProfileSlot(selectable);
            UpdateAbilitesSlots(selectable);
        }

        private void UpdateProfileSlot(GameObject selected)
        {
            IProfileUI profile = selected.GetComponent<IProfileUI>();
            if (profile != null)
            {
                Sprite ProfileImage = profile.GetProfileImage();
                characterProfile.sprite = ProfileImage;
            }
        }

        private void UpdateAbilitesSlots(GameObject selected)
        {
            ICharacter character = selected.GetComponent<ICharacter>();
            if (character != null)
            {
                ITeam team = selected.GetComponent<ITeam>();
                if(team.GetTeamType() == TeamType.Player)
                {
                    Abilities abilityComponent = selected.GetComponent<Abilities>();
                    if (abilityComponent != null)
                    {
                        int numOfAbilitys = abilityComponent.GetNumberOfAbilities();

                        for (int i = 0; i < numOfAbilitys; i++)
                        {
                            AbilityConfig abilityConfig = abilityComponent.GetAbilityAtIndex(i);
                            abilityConfig.SetUser(selectedGO);
                            buttons[i].gameObject.SetActive(true);
                            Sprite abilityImage = abilityConfig.GetAbilityColor();
                            int currentCoolDown = abilityConfig.GetCurrentCoolDown();
                            buttons[i].GetComponent<Image>().sprite = abilityImage;
                            SetCoolDownIconOnButton(i, currentCoolDown);
                        }
                    }
                }
            }
        }

        void SetCoolDownIconOnButton(int ButtonIndex, int currentCoolDown)
        {
            Text text = buttons[ButtonIndex].GetComponentInChildren<Text>();
            if(currentCoolDown != 0)
            {
                text.enabled = true;
                text.text = currentCoolDown.ToString();
            }
            else
            {
                text.enabled = false;
            }
        }

        public void Ability0Pressed()
        {
            if (PlayerController.Instance.IsTaskInAction() == false)
            {
                ICharacter character = selectedGO.GetComponent<ICharacter>();
                if (character != null)
                {
                    character.AbilityPressed(0);
                }
            }
        }

        public void Ability0Hoverd()
        {
            DisplayToolTip(0);
        }

        public void Ability1Pressed()
        {
            if (PlayerController.Instance.IsTaskInAction() == false)
            {
                ICharacter character = selectedGO.GetComponent<ICharacter>();
                if (character != null)
                {
                    character.AbilityPressed(1);
                }
            }
        }

        public void Ability1Hoverd()
        {
            DisplayToolTip(1);
        }



        public void Ability2Pressed()
        {
            if (PlayerController.Instance.IsTaskInAction() == false)
            {
                ICharacter character = selectedGO.GetComponent<ICharacter>();
                if (character != null)
                {
                    character.AbilityPressed(2);
                }
            }
        }

        public void Ability2Hoverd()
        {
            DisplayToolTip(2);
        }

        private void DisplayToolTip(int abilityNumber)
        {
            Abilities abilities = selectedGO.GetComponent<Abilities>();
            if (abilities != null)
            {
                AbilityConfig abilityConfig = abilities.GetAbilityAtIndex(abilityNumber);
                GameObject toolTipGO = abilityConfig.GetAbilityToolTip();
                toolTip = Instantiate(toolTipGO, gameObject.transform.position, Quaternion.identity, gameObject.transform);
                toolTip.GetComponent<AbilityToolTip>().Init(abilityConfig);
            }
        }


        public void AbilityDehoverd()
        {
            RemoveToolTip();
        }

        private void RemoveToolTip()
        {
            if (toolTip != null)
            {
                Destroy(toolTip);
                toolTip = null;
            }
        }
    }
}