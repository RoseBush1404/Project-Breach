using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float damage = 1f;

    Rigidbody2D myRigidBody;
    string teamTag;

    private void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        SetVelocity();
    }

    private void SetVelocity()
    {
        myRigidBody.velocity = transform.up * speed;
        Destroy(gameObject, lifeTime);
    }

    public void SetTeam(string newTag)
    {
        teamTag = newTag;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Character character = other.GetComponent<Character>();
        if(character != null)
        {
            if(other.gameObject.tag != teamTag)
            {
                other.GetComponent<HealthSystem>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else
        {
            Waypoint waypoint = other.GetComponent<Waypoint>();
            if (waypoint != null)
            {
                // ignore waypoint collisions
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }


}
