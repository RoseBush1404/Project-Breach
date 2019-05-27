using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Manager.Pathfinding;
using Breach.Placeable.Pathfinding;
using Breach.Placeable.Characters;
using Breach.Animation;

namespace Breach.Placeable.Component
{
    public class CharacterMovement : MonoBehaviour
    {

        [SerializeField] float movementSpeed = 2f;
        [SerializeField] int movementRange = 5;

        Waypoint currentWaypoint;
        MovementAnimationSelecter movementAnimationSelecter;
        Character character;

        public void Init()
        {
            movementAnimationSelecter = GetComponent<MovementAnimationSelecter>();
            character = GetComponent<Character>();

            GetStartingWaypoint();
        }

        public void SetSpeed(float speed)
        {
            movementSpeed = speed;
        }

        public float GetSpeed()
        {
            return movementSpeed;
        }

        public void SetRange(int range)
        {
            movementRange = range;
        }

        public int GetRange()
        {
            return movementRange;
        }

        private void GetStartingWaypoint()
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    Waypoint waypointHit = collider.gameObject.GetComponent<Waypoint>();
                    if (waypointHit)
                    {
                        currentWaypoint = waypointHit;
                        character.SetCurrentWaypoint(currentWaypoint);
                        currentWaypoint.isSelectable = false;
                        currentWaypoint.characterOnTile = character;
                        return;
                    }
                }
            }
        }

        public void PlotMovement(Waypoint destination = null)
        {
            Pathfinder.Instance.GetPath(currentWaypoint, destination, movementRange);
        }

        public void Move(Waypoint destination)
        {
            var path = Pathfinder.Instance.GetPath(currentWaypoint, destination, movementRange);
            if (path.Count > 0)
            {
                character.UseMoveAction();
                StartCoroutine(FollowPath(path));
            }
            else
            {
                character.TaskFinished();
            }
        }

        IEnumerator FollowPath(List<Waypoint> path)
        {
            currentWaypoint.isSelectable = true;
            currentWaypoint.characterOnTile = null;

            foreach (Waypoint nextWaypoint in path)
            {
                movementAnimationSelecter.SetAnimation(currentWaypoint, nextWaypoint);
                Transform startPoint = currentWaypoint.gameObject.transform;
                Transform endPoint = nextWaypoint.gameObject.transform;
                float distanceAlong = 0;

                while (transform.position != nextWaypoint.gameObject.transform.position)
                {
                    distanceAlong = Mathf.Clamp(distanceAlong + (movementSpeed * Time.deltaTime), 0, 1);
                    transform.position = Vector2.Lerp(startPoint.position, endPoint.position, distanceAlong);
                    yield return new WaitForEndOfFrame();
                }
                WaypointManager.Instance.DisableSingleSpriteNode(nextWaypoint);
                currentWaypoint = nextWaypoint;
            }
            character.SetCurrentWaypoint(currentWaypoint);
            currentWaypoint.isSelectable = false;
            currentWaypoint.characterOnTile = character;
            currentWaypoint.LandedOnWaypoint(character);
            movementAnimationSelecter.SetAnimation(currentWaypoint);
            character.TaskFinished();
        }
    }
}