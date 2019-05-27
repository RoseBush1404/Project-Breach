using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Breach.Placeable.Characters;
using Breach.Placeable.Pathfinding;

namespace Breach.Placeable.Component
{
    public class Projectile : BreachObject
    {
        [SerializeField] float speed = 5f;
        [SerializeField] float lifeTime = 3f;
        [SerializeField] float damage = 1f;
        [SerializeField] GameObject hitEffect;

        Rigidbody2D myRigidBody;
        IDamageable damageableToIgnore;

        public void Init(TeamType team)
        {
            teamType = team;
            canBeSelected = false;
            SetVelocity();
            Destroy(gameObject, lifeTime);
        }

        private void SetVelocity()
        {
            myRigidBody = GetComponent<Rigidbody2D>();
            myRigidBody.velocity = transform.up * speed;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            //is it IDamageable?
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                //is it cover?
                Cover cover = other.GetComponent<Cover>();
                if (cover != null)
                {
                    //which team is it?
                    ITeam team = other.GetComponent<ITeam>();
                    if (team.GetTeamType() != teamType)
                    {
                        //is it dealling damage to it
                        if (cover.ChanceToHit())
                        {
                            //damage the cover
                            damageable.TakeDamage(damage);
                            SpawnHitEffect();
                            Destroy(gameObject);
                        }
                        else
                        {
                            //no, tell project which character to ignore
                            damageableToIgnore = cover.GetUserOfCover();
                        }
                    }
                }
                else //is it anything else with the IDamageable
                {
                    //which team is it and does the projectile need to ignore it?
                    ITeam team = other.GetComponent<ITeam>();
                    if (team.GetTeamType() != teamType && damageable != damageableToIgnore)
                    {
                        //deal damage
                        damageable.TakeDamage(damage);
                        SpawnHitEffect();
                        Destroy(gameObject);
                    }
                }
            }
            else
            {
                //is it a waypoint or another projectile?
                Waypoint waypoint = other.GetComponent<Waypoint>();
                Projectile otherPorjectiles = other.GetComponent<Projectile>();
                if (waypoint != null || otherPorjectiles != null)
                {
                    // ignore waypoint and projectile collisions
                }
                else if(other.name == "CameraSpace")
                {
                    //ignore Camera bounding box
                }
                else
                {
                    SpawnHitEffect();
                    Destroy(gameObject);
                }
            }
        }

        protected void SpawnHitEffect()
        {
            Instantiate(hitEffect, transform.position, transform.rotation);
        }
    }
}