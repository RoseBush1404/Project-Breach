using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    public interface ITurn
    {
        void PlayerUpkeep();
        void PlayerEndStep();
        void EnemyUpkeep();
        void EnemyEndStep();
    }
}
