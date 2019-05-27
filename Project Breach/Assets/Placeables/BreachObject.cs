using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Breach.UI;
using Breach.Placeable.Component;

namespace Breach.Placeable
{
    public abstract class BreachObject : MonoBehaviour, ISelectable, IProfileUI, ITeam
    {
        [SerializeField] protected bool canBeSelected = true;
        [SerializeField] protected TeamType teamType = TeamType.Neutral; //TODO consider changing to public or private
        [SerializeField] Sprite profileImage; // TODO change to image

        public virtual void Select() { }

        public virtual void Deselect() { }

        public void ToggleSelectability(bool newSelectability) { canBeSelected = newSelectability; }

        public bool GetSelectability() { return canBeSelected; }

        public GameObject GetGameObject() { return gameObject; }

        public Sprite GetProfileImage() { return profileImage; }

        public TeamType GetTeamType() { return teamType; }

        public void SetTeamType(TeamType newTeam) { teamType = newTeam; }

        public void SetProfileImage(Sprite image) { profileImage = image; }
    }
}
