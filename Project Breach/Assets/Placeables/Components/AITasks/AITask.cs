using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Characters.AI
{
    public abstract class AITask : ScriptableObject
    {
        public abstract int GetScore(GameObject attachedGameObject);

        public abstract void ExecuteTask(GameObject attachedGameObject);
    }
}
