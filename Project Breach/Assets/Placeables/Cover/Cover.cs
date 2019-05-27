using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Breach.Placeable.Pathfinding;
using Breach.Placeable.Characters;

namespace Breach.Placeable.Component
{
    public class Cover : BreachObject, IDamageable
    {
        [SerializeField] float maxHitPoints = 5f;
        [SerializeField] bool canBeDamaged = false;
        [SerializeField] Image healthBar;
        [SerializeField] int toHitModifier = 25;

        List<Waypoint> waypoints = new List<Waypoint>();

        BoxCollider2D collider;
        Character characterUsingCover;
        float currentHitPoints = 0f;
        bool canCoverBeUsed = true;
        int toHitPercentage = 50;

        public void SetMaxHitPoints(float amount)
        {
            maxHitPoints = amount;
        }

        public float GetMaxHitPoints()
        {
            return maxHitPoints;
        }

        public void SetCurrentHitPoints(float amount)
        {
            currentHitPoints = amount;
        }

        public float GetCurrentHitPoints()
        {
            return currentHitPoints;
        }

        public void Init()
        {
            Vector2 LeftPosition = transform.position + new Vector3(-1, 0.3f, 0);
            Vector2 RightPosition = transform.position + new Vector3(1, 0.3f, 0);
            Collider2D[] collidersLeft = Physics2D.OverlapCircleAll(LeftPosition, 0.1f);
            CheckDirectionForWaypoint(collidersLeft);
            Collider2D[] collidersRight = Physics2D.OverlapCircleAll(RightPosition, 0.1f);
            CheckDirectionForWaypoint(collidersRight);

            collider = GetComponent<BoxCollider2D>();
            collider.enabled = false;

            Canvas canvas = healthBar.GetComponentInParent<Canvas>();
            canvas.enabled = false;
            TakeDamage(0);

            CheckForCharactersInRange();
        }

        private void CheckDirectionForWaypoint(Collider2D[] colliders)
        {
            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    Waypoint waypointHit = collider.gameObject.GetComponent<Waypoint>();
                    if (waypointHit != null)
                    {
                        if (!waypoints.Contains(waypointHit))
                        {
                            waypoints.Add(waypointHit);
                            waypointHit.CoverOnThisTile.Add(this);
                            return;
                        }
                    }
                }
            }
        }

        public void EnableCover(Character character)
        {
            if(teamType == TeamType.Neutral && canCoverBeUsed)
            {
                collider.enabled = true;
                teamType = character.GetTeamType();
                characterUsingCover = character;
                character.OnCharacterMove += DisableCover;
                character.OnCharacterDeath += DisableCover;
            }
        }

        private void DisableCover(Character character)
        {
            if (characterUsingCover != null)
            {
                collider.enabled = false;
                teamType = TeamType.Neutral;
                characterUsingCover.OnCharacterMove -= DisableCover;
                characterUsingCover.OnCharacterDeath -= DisableCover;
                CheckForCharactersInRange();
            }
        }

        private void CheckForCharactersInRange()
        {
            foreach(Waypoint waypoint in waypoints)
            {
                if (waypoint.characterOnTile != null && characterUsingCover != waypoint.characterOnTile)
                {
                    EnableCover(waypoint.characterOnTile);
                }
            }
        }

        public IDamageable GetUserOfCover()
        {
            return characterUsingCover.GetComponent<IDamageable>();
        }

        public bool ChanceToHit()
        {
            //genater a random number between 1 to 100
            int randomNumber = Random.Range(1, 101);
            //is that number higher then the to hit
            if (randomNumber >= toHitPercentage)
            {
                //yes, remove set amount from 'tohit'
                toHitPercentage = Mathf.Clamp(toHitPercentage - toHitModifier, 0, 100);
                return true;
            }
            else
            {
                //no, add set amount to 'tohit'
                toHitPercentage = Mathf.Clamp(toHitPercentage + toHitModifier, 0, 100);
                return false;
            }
        }

        public void TakeDamage(float damage)
        {
            if (canBeDamaged)
            {
                canCoverBeUsed = (currentHitPoints - damage > 0);
                currentHitPoints = Mathf.Clamp(currentHitPoints - damage, 0f, maxHitPoints);
                if (currentHitPoints >= maxHitPoints) { return; }

                Canvas canvas = healthBar.GetComponentInParent<Canvas>();
                canvas.enabled = true;
                UpdateHealthBar();
                if (!canCoverBeUsed)
                {
                    GetComponent<SpriteRenderer>().color = Color.gray;
                    DisableCover(null);
                }
            }
        }

        private float healthAsPercentage { get { return currentHitPoints / maxHitPoints; } }

        private void UpdateHealthBar()
        {
            if (healthBar)
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }
    }
}
