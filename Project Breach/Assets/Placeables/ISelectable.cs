using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable
{
    public interface ISelectable
    {
        void Select();
        void Deselect();
        void ToggleSelectability(bool newSelectability);
        bool GetSelectability();
        GameObject GetGameObject();
    }
}
