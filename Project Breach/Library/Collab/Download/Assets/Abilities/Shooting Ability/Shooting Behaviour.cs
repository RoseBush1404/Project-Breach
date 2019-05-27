using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShootingBehaviour : AbilityBehaviour {

    bool isOverlayActive = false;
    GameObject overlay;
    Quaternion firingAngleWithOffset;

    //----------------------------------------------------------------

    public override void Use()
    {
        StartCoroutine(FireProjectiles());
    }

    IEnumerator FireProjectiles()
    {
        bool hasProjectileToShoot = true;
        float lastShotTime = 0f;
        int projectilesFiredSoFar = 0;

        while(hasProjectileToShoot)
        {
            yield return new WaitForFixedUpdate();
            bool isTimeToShotAgain = Time.time - lastShotTime > (config as ShootingConfig).GetDelayBetweenShots();

            if(isTimeToShotAgain)
            {
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
        character.TaskFinished();
    }

    private void SpawnProjectile()
    {
        GameObject projectilePrefab = (config as ShootingConfig).GetProjectilePrefab();
        float randomisedAngle = UnityEngine.Random.Range(0, (config as ShootingConfig).GetFiringAngleRange());

        Quaternion angleRotation = Quaternion.AngleAxis(randomisedAngle, Vector3.forward);
        Quaternion finalFiringAngle = firingAngleWithOffset * Quaternion.Inverse(angleRotation);

        GameObject projectile = Instantiate(projectilePrefab, gameObject.transform.position, finalFiringAngle);
        projectile.GetComponent<Projectile>().SetTeam(gameObject.tag);
    }

    //----------------------------------------------------------------

    public override void PlotAbility()
    {
        if (!isOverlayActive)
        {
            SpawnOverlay();
        }
        print(gameObject.name);
        AimOverlayTowardsMouse();
    }

    private void SpawnOverlay()
    {
        overlay = Instantiate(config.GetOverlay(), gameObject.transform.position, Quaternion.identity);
        overlay.transform.position = gameObject.transform.position;
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
    }

    //----------------------------------------------------------------

    public override void DisablePlot()
    {
        Destroy(overlay);
        isOverlayActive = false;
    }
}
