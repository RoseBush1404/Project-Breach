using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Pathfinding;

namespace Breach.Placeable.Characters
{
    public interface ICharacter
    {
        void PlotMovement(Waypoint waypoint = null);
        void Move( Waypoint waypoint);
        void AbilityPressed(int abilityIndex);
        bool GetMoveAction();
        bool GetAbilityAction();
        void ResetActionPoints();
    }
}
