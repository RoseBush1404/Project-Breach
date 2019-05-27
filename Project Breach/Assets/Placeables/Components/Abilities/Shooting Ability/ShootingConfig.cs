using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Breach.Placeable.Component
{
    [CreateAssetMenu(menuName = ("my Game/Abilities/Shooting ability"))]
    public class ShootingConfig : AbilityConfig
    {

        [Header("Shooting Ability Specific")]
        [SerializeField] float firingAngleRange;
        [SerializeField] int numOfShotsToFire;
        [SerializeField] float delayBetweenShots;
        [SerializeField] GameObject projectilePrefab;


        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<ShootingBehaviour>();
        }

        public override void SetUser(GameObject user)
        {
            behaviour = user.GetComponent<ShootingBehaviour>();
        }

        public float GetFiringAngleRange()
        {
            return firingAngleRange;
        }

        public int GetNumOfShotsToFire()
        {
            return numOfShotsToFire;
        }

        public GameObject GetProjectilePrefab()
        {
            return projectilePrefab;
        }

        public float GetDelayBetweenShots()
        {
            return delayBetweenShots;
        }
    }
}