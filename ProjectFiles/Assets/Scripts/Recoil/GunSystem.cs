using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.HID;
using Photon.Pun;
using System.Drawing;

public class GunSystem : MonoBehaviourPunCallbacks
{
    //Gun stats
    public float damage;
    public float timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    [HideInInspector]
    public int bulletsLeft, totalAmmo;
    public int magazineSize, bulletsPerTap, magazineAmaount;
    public bool allowButtonHold;
    int bulletsShot;
    public bool holding = false;
    //bools 
    bool shooting, readyToShoot, reloading;

    //Reference
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    public AdvancedCamRecoil CamRecoil;
    public AdvancedWeaponRecoil WeaponRecoil;

    //Graphics
    public GameObject bulletHoleGraphic, bleedingGraphic,sandGraphic;
    public ParticleSystem muzzleFlash;
    //public CamShake camShake;
    public float camShakeMagnitude, camShakeDuration;
    public PlayerStatus player;

    public Animator animator;
    private void Awake()
    {
        bulletsLeft = magazineSize;
        totalAmmo = (magazineAmaount-1) * magazineSize;
        readyToShoot = true;
    }
    public void FinnishShoot()
    {
        readyToShoot = true;
    }
    public void TryShoot()
    {
        if (player.isDead) return;
        if (!allowButtonHold && !holding)
            return;
        //Shoot
        if (readyToShoot  && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
            
        }
        updateAmmoUI();
    }
    private void Shoot()
    {
        CamRecoil.Fire();
        WeaponRecoil.Fire();
        readyToShoot = false;
        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            //Debug.Log(rayHit.collider.name);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, rayHit.normal);
            //Debug.Log(rotation);
            //rotation = Quaternion.Euler(-90, 0, 0) * rotation;
            if (rayHit.collider.CompareTag("Player"))
            {
                Instantiate(bleedingGraphic, rayHit.point, rotation);
                PlayerStatus player = GameManager.instance.GetPlayer(rayHit.collider.gameObject);
                player.photonView.RPC("OnHealChange", player.photonPlayer, damage);
            }             
            else if (rayHit.collider.CompareTag("Ground"))
            {
                photonView.RPC("BulletHoleEffect", RpcTarget.AllBuffered, rayHit.point, rotation);
                //Instantiate(bulletHoleGraphic, rayHit.point, rotation);
                //Instantiate(sandGraphic, rayHit.point, Quaternion.Euler(0, 180, 0));
            }
            else
            {
                //Instantiate(bulletHoleGraphic, rayHit.point, rotation);
                photonView.RPC("BulletHoleEffect", RpcTarget.AllBuffered,rayHit.point,rotation);
            }

            //if (rayHit.collider.CompareTag("Enemy"))
            //rayHit.collider.GetComponent<ShootingAi>().TakeDamage(damage);
        }
        else
        {
            rayHit = new RaycastHit();
        }
        //Graphics

        photonView.RPC("PlayMuzzleFlash", RpcTarget.AllBuffered);

        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke("Shoot", timeBetweenShots);
    }
    [PunRPC]
    private void BulletHoleEffect(Vector3 hitPosition, Quaternion rotation)
    {
        Instantiate(bulletHoleGraphic, hitPosition, rotation);
        Instantiate(sandGraphic, hitPosition, Quaternion.Euler(0, 180, 0));
    }
    [PunRPC]
    private void PlayMuzzleFlash()
    {
        muzzleFlash.Play();
    }
    private void ResetShot()
    {
        readyToShoot = true;
    }
    public void Reload()
    {
        if(totalAmmo > 0 && !reloading && bulletsLeft < magazineSize)
        {
            reloading = true;
            animator.SetTrigger("ReloadTrigger");
            animator.SetBool("ReloadBool", true);
            Invoke("ReloadFinished", reloadTime);
        }
    }
    private void ReloadFinished()
    {
        int total = bulletsLeft + totalAmmo;
        bulletsLeft = total > magazineSize ? magazineSize: total;
        totalAmmo = total - bulletsLeft;
        animator.SetBool("ReloadBool", false);
        reloading = false;
        updateAmmoUI();
    }
    public void OnAimStatusChange(bool onAim)
    {
        WeaponRecoil.aiming = onAim;
        CamRecoil.aiming = onAim;
    }
    [PunRPC]
    public void GainAmmo(int amount)
    {
        totalAmmo += amount;
        updateAmmoUI();
    }
    public void updateAmmoUI()
    {
        GameUIManager.instance.UpdateBulletInfo(bulletsLeft, totalAmmo);
    }
}