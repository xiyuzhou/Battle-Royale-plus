using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private PlayerStatus player;
    [Header("Health Bar")]
    public Image FrontHealthBar;
    public Image BackHealthBar;
    public float chipSpeed = 2f;
    private float lerpTimer;
    [Header("Damage Effect")]
    public Image overlay;
    public float duration;
    public float fadeSpeed;
    private float durationTimer;
    public float startAlpha;

    [Header("InteractionText")]
    [SerializeField]
    private TextMeshProUGUI promptText;

    [Header("Weapon")]
    public TextMeshProUGUI BulletInfo;

    public Image winBackground;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI playerInfoText;

    public static GameUIManager instance;
    private bool started = false;
    void Awake()
    {
        instance = this;
    }
    public void Initialize(PlayerStatus localPlayer)
    {
        player = localPlayer;
        UpdateHealthBar();
        UpdatePlayerInfoText();
        player.GetComponentInChildren<GunSystem>().updateAmmoUI();
        started = true;
    }
    private void Update()
    {
        if (started)
        {
            UpdateHealthBar();
            UpdateDamageEffectOverlay();
        }
        
    }
    public void UpdateHealthBar()
    {
        float fillFront = FrontHealthBar.fillAmount;
        float fillBack = BackHealthBar.fillAmount;
        float hFraction = player.health/player.maxHealth;
        if (!player.HPchange) 
        {
            FrontHealthBar.fillAmount = hFraction;
            //BackHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            BackHealthBar.fillAmount = Mathf.Lerp(fillBack, hFraction, percentComplete);    
        }
        else
        {
            //BackHealthBar.color = Color.green;
            BackHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            FrontHealthBar.fillAmount = Mathf.Lerp(fillFront, hFraction, percentComplete);
        }

    }
    public void ShowOverlay()
    {
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, startAlpha);
    }
    public void UpdateDamageEffectOverlay()
    {
        if (overlay.color.a > 0)
        {
            durationTimer += Time.deltaTime;
            if(durationTimer > duration)
            {
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r,overlay.color.g, overlay.color.b,tempAlpha);
            }
        }
    }
    public void clearLerpTimer()
    {
        lerpTimer = 0;
        durationTimer = 0;

    }
    public void UpdateInteractText(string promptMessage)
    {
        promptText.text = promptMessage;
    }

    public void UpdateBulletInfo(int bulletsLeft, int totalAmmo)
    {
        string a = bulletsLeft == 0 ? "reload" : bulletsLeft.ToString();
        BulletInfo.text = a + " / " + totalAmmo;
    }

    public void SetWinText(string winnerName)
    {
        winBackground.gameObject.SetActive(true);
        winText.text = winnerName + " wins";
    }
    public void UpdatePlayerInfoText()
    {
        playerInfoText.text = "<b>Alive:</b> " + GameManager.instance.alivePlayers + "\n<b> Kills:</b> " + player.kills;
    }
}
