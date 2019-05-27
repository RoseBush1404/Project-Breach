using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Manager.Pathfinding;
using Breach.Placeable.Pathfinding;

namespace Breach.Animation
{
    public class MovementAnimationSelecter : MonoBehaviour
    {

        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] List<MovementAnimationConfig> IdleAnimationConfigs = new List<MovementAnimationConfig> { };
        [SerializeField] List<MovementAnimationConfig> movementAnimationConfigs = new List<MovementAnimationConfig> { };

        const string MOVING_BOOL = "Moving";
        const string DEFAULT_MOVE_STATE = "DEFAULT MOVING";
        const string DEFAULT_IDLE_STATE = "DEFAULT IDLE";

        TileType nextTileType;
        TileType currentTileType;
        Vector2 direction;
        SpriteRenderer spriteRenderer;
        Animator animator;

        public void Init()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetAnimation(Waypoint currentWaypoint, Waypoint nextWaypoint = null)
        {
            ResetAnimatorValues();
            StoreInformation(currentWaypoint, nextWaypoint);
            animator.runtimeAnimatorController = animatorOverrideController;

            if (nextWaypoint == null || nextWaypoint == currentWaypoint)
            {
                SearchForCorrectIdleAnimation();
                return;
            }

            SearchForCorrectMovingAnimation();

            SetFacingDirection();
        }

        private void StoreInformation(Waypoint currentWaypoint, Waypoint nextWaypoint)
        {
            if (nextWaypoint != null)
            {
                nextTileType = nextWaypoint.GetTileType();
                direction = nextWaypoint.direction;
            }
            currentTileType = currentWaypoint.GetTileType();
        }

        private void ResetAnimatorValues()
        {
            animator.SetBool(MOVING_BOOL, false);
        }

        private void SetFacingDirection()
        {
            if (direction == Vector2.right || direction == new Vector2(1, 1) || direction == new Vector2(1, -1))
            {
                spriteRenderer.flipX = false;
            }
            else if (direction == Vector2.left || direction == new Vector2(-1, 1) || direction == new Vector2(-1, -1))
            {
                spriteRenderer.flipX = true;
            }
        }

        private void PlaySelectedMovementAnimation(MovementAnimationConfig animationConfig)
        {
            animatorOverrideController[DEFAULT_MOVE_STATE] = animationConfig.GetAnimationClip();
            animator.SetBool(MOVING_BOOL, true);
        }

        private void PlaySelectedIdleAnimation(MovementAnimationConfig animationConfig)
        {
            animatorOverrideController[DEFAULT_IDLE_STATE] = animationConfig.GetAnimationClip();
        }

        private void SearchForCorrectIdleAnimation()
        {
            List<MovementAnimationConfig> validAnimations = new List<MovementAnimationConfig>(IdleAnimationConfigs);
            List<MovementAnimationConfig> tempList = new List<MovementAnimationConfig>(validAnimations);

            for (int i = 0; i < validAnimations.Count; i++)
            {
                if (validAnimations[i].GetFromTile() != currentTileType)
                {
                    tempList.Remove(validAnimations[i]);
                }
            }

            validAnimations.Clear();
            validAnimations = new List<MovementAnimationConfig>(tempList);

            if (validAnimations.Count > 0)
            {
                PlaySelectedIdleAnimation(validAnimations[0]);
            }
            else
            {
                print("no valid animation to play from the idle animation selecter for " + gameObject.name);
            }
        }

        private void SearchForCorrectMovingAnimation()
        {
            List<MovementAnimationConfig> validAnimations = new List<MovementAnimationConfig>(movementAnimationConfigs);
            List<MovementAnimationConfig> tempList = new List<MovementAnimationConfig>(validAnimations);

            for (int i = 0; i < validAnimations.Count; i++)
            {
                if (validAnimations[i].GetFromTile() != currentTileType)
                {
                    tempList.Remove(validAnimations[i]);
                }
            }

            validAnimations.Clear();
            validAnimations = new List<MovementAnimationConfig>(tempList);

            for (int i = 0; i < validAnimations.Count; i++)
            {
                if (validAnimations[i].GetToTile() != nextTileType)
                {
                    tempList.Remove(validAnimations[i]);
                }
            }

            validAnimations.Clear();
            validAnimations = new List<MovementAnimationConfig>(tempList);

            for (int i = 0; i < validAnimations.Count; i++)
            {
                bool directionMatch = false;
                Vector2Int[] configDirections = validAnimations[i].GetDirections();
                for (int x = 0; x < configDirections.Length; x++)
                {
                    if (configDirections[x] == direction)
                    {
                        directionMatch = true;
                    }
                }
                if (directionMatch == false)
                {
                    tempList.Remove(validAnimations[i]);
                }
            }

            validAnimations.Clear();
            validAnimations = new List<MovementAnimationConfig>(tempList);

            if (validAnimations.Count > 0)
            {
                PlaySelectedMovementAnimation(validAnimations[0]);
            }
            else
            {
                print("no valid animation to play from the movment animation selecter for " + gameObject.name);
            }
        }
    }
}