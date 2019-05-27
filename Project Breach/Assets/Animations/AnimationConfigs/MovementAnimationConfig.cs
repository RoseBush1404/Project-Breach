using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Manager.Pathfinding;

namespace Breach.Animation
{
    [CreateAssetMenu(menuName = ("my Game/Animation/Movement Animation"))]
    public class MovementAnimationConfig : ScriptableObject
    {

        [SerializeField] AnimationClip animationClip;
        [SerializeField] TileType fromTile;
        [SerializeField] TileType toTile;
        [SerializeField] Vector2Int[] directions;

        public AnimationClip GetAnimationClip()
        {
            return animationClip;
        }

        public TileType GetFromTile()
        {
            return fromTile;
        }

        public TileType GetToTile()
        {
            return toTile;
        }

        public Vector2Int[] GetDirections()
        {
            return directions;
        }
    }
}
