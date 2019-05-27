using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.UI
{
    public interface IPauseable
    {
        void SetUpPauseMenu();
        void PauseGame();
        void ResumeGame();
    }
}