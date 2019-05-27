using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Breach.Placeable.Characters;
using System;

namespace Breach.Placeable.Component
{
    public class ShootingBehaviour : AbilityBehaviour
    {
        bool isOverlayActive = false;
        GameObject overlay;
        Quaternion firingAngleWithOffset;
        GameObject muzzlePoint;
        SpriteRenderer spriteRenderer;

        //----------------------------------------------------------------

        public override void Use()
        {
            StartCoroutine(FireProjectiles());
        }

        public override void AIUse()
        {
            StartCoroutine(FireProjectiles());
        }

        IEnumerator FireProjectiles()
        {
            bool hasProjectileToShoot = true;
            float lastShotTime = 0f;
            int projectilesFiredSoFar = 0;

            //start shooting animation
            animator.SetBool(ABILITY_FINISHED_BOOL, false);

            while (hasProjectileToShoot)
            {
                yield return new WaitForFixedUpdate();
                bool isTimeToShotAgain = Time.time - lastShotTime > (config as ShootingConfig).GetDelayBetweenShots();

                if (isTimeToShotAgain)
                {
                    PlayAnimation();
                    SpawnProjectile();
                    projectilesFiredSoFar++;
                    lastShotTime = Time.time;
                    isTimeToShotAgain = false;
                    if (projectilesFiredSoFar >= (config as ShootingConfig).GetNumOfShotsToFire()) { hasProjectileToShoot = false; }
                }
            }

            while (GameObject.FindGameObjectWithTag("Projectile") != null)
            {
                yield return new WaitForFixedUpdate();
            }
            Character character = GetComponent<Character>();
            character.UseAbilityAction();
            SetCurrentCoolDown(config.GetMaxCoolDown());
            //finish shooting animation
            animator.speed = 1;
            animator.SetBool(ABILITY_FINISHED_BOOL, true);
            character.TaskFinished();
        }

        private void PlayAnimation()
        {
            Abilities abilitiesComp = GetComponent<Abilities>();
            AnimationClip animationClip = (config as ShootingConfig).GetAbilityAnimation();

            if(animationClip.length > (config as ShootingConfig).GetDelayBetweenShots())
            {
                animator.speed = 2;
            }

            abilitiesComp.SetAbilityAnimation(animationClip);
            animator.SetTrigger(USE_ABILITY_TRIGGER);
        }

        private void SpawnProjectile()
        {
            print("shooting");
            GameObject projectilePrefab = (config as ShootingConfig).GetProjectilePrefab();
            float maxFiringAngle = AngleChanges((config as ShootingConfig).GetFiringAngleRange());
            float randomisedAngle = UnityEngine.Random.Range(0, maxFiringAngle);

            Quaternion angleRotation = Quaternion.AngleAxis(randomisedAngle, Vector3.forward);
            Quaternion finalFiringAngle = firingAngleWithOffset * Quaternion.Inverse(angleRotation);

            GameObject projectile = Instantiate(projectilePrefab, muzzlePoint.transform.position, finalFiringAngle);
            ITeam team = GetComponent<ITeam>();
            projectile.GetComponent<Projectile>().Init(team.GetTeamType());
        }

        private float AngleChanges(float firingAngle)
        {
            int randomNumber = UnityEngine.Random.Range(1, 4);
            if(randomNumber > 1)
            {
                //change the angle to be smaller
                float amountToRemove = (firingAngle / 7) * 2;
                return firingAngle - amountToRemove;
            }
            else
            {
                //return angle unchanged
                return firingAngle;
            }
        }

        //----------------------------------------------------------------

        public override void PlotAbility()
        {
            if (!isOverlayActive)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
                SpawnOverlay();
            }
            AimOverlayTowardsMouse();
        }

        private void SpawnOverlay()
        {
            overlay = Instantiate(config.GetOverlay(), gameObject.transform.position, Quaternion.identity);
            muzzlePoint = gameObject.GetComponentInChildren<MuzzlePoint>().GetGameObject();
            overlay.transform.position = muzzlePoint.transform.position;
            overlay.GetComponentInChildren<Image>().fillAmount = ((config as ShootingConfig).GetFiringAngleRange() / 360);
            isOverlayActive = true;
        }

        private void AimOverlayTowardsMouse()
        {
            //get mouse position
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            //find normal look at rotation with mouse position
            Vector2 direction = new Vector2((mousePosition.x - transform.position.x), (mousePosition.y - transform.position.y));
            Quaternion lookAtRotation = Quaternion.FromToRotation(Vector2.up, direction);

            //find rotation offset to centralize the overlay
            float halfOfFiringAngle = (config as ShootingConfig).GetFiringAngleRange() / 2;
            Quaternion rotationOffset = Quaternion.AngleAxis(halfOfFiringAngle, Vector3.forward);

            //make the new roation for the overlay
            firingAngleWithOffset = rotationOffset * lookAtRotation;
            overlay.GetComponentInChildren<RectTransform>().transform.rotation = firingAngleWithOffset;

            //flip character sprite in direction of mouse
            flipSprite(mousePosition.x);
        }

        private void flipSprite(float pointX)
        {
            Vector2 characterPos = gameObject.transform.position;

            if(characterPos.x >= pointX)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }
        }

        public void AimTowardsPoint(Vector3 point) //Used for AI side of the ability
        {
            //find normal look at rotation with mouse position
            Vector2 direction = new Vector2((point.x - transform.position.x), (point.y - transform.position.y));
            Quaternion lookAtRotation = Quaternion.FromToRotation(Vector2.up, direction);

            //find rotation offset to centralize the overlay
            float halfOfFiringAngle = (config as ShootingConfig).GetFiringAngleRange() / 2;
            Quaternion rotationOffset = Quaternion.AngleAxis(halfOfFiringAngle, Vector3.forward);

            //make the new roation for the overlay
            firingAngleWithOffset = rotationOffset * lookAtRotation;

            //get muzzle point
            muzzlePoint = gameObject.GetComponentInChildren<MuzzlePoint>().GetGameObject();

            spriteRenderer = GetComponent<SpriteRenderer>();
            flipSprite(direction.x);
        }

        //----------------------------------------------------------------

        public override void DisablePlot()
        {
            Destroy(overlay);
            isOverlayActive = false;
        }
    }
}