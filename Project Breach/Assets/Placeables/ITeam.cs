using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    [Serializable]
    public enum TeamType { Player, Enemy, Neutral}

    public interface ITeam
    {
        TeamType GetTeamType();
    }
}