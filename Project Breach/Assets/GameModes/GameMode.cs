using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Manager;

namespace Breach.GameModes
{
    public abstract class GameMode : MonoBehaviour, IGameMode
    {
        [SerializeField] protected GameState gameState;

        public virtual void Start()
        {
            PlaySessionManager.Instance.UpdateGameState(gameState);
        }
    }
}