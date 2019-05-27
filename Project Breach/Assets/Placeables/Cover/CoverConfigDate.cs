using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    [Serializable]
    public class CoverConfigDate
    {
        public GameObject coverPrefab;
        public Vector2Int location;
        public float currentHitPoints = -1;
        public float maxHitPoints = 0;
    }
}