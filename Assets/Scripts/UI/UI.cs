using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] Image healthBar, a1_Icon, a2_Icon, a3_Icon, a1_Fill, a2_Fill, a3_Fill, player_Icon, player_Fill;
    [SerializeField] TMP_Text currenthealth, maxHealth, a1_tmp, a2_tmp, a3_tmp, player_tmp;
    private PlayerHealth playerHp;
    float uiTimer = 0;
    private bool set = false;

    public void Setup(PlayerHealth hp) {
        //Icons
        //Cooldowns

        //Health bar
        playerHp = hp;

        //Respawn

        set = true;
    }


    private void Update() {
        if (!set)
            return;
        if(uiTimer >= 1.0f / 30.0f) {
            uiTimer = 0;
            float fillPerc = 0;
            if(playerHp.GetCurrentHealth() > 0)
                fillPerc = playerHp.GetCurrentHealth() / playerHp.GetMaxHealth();
            healthBar.fillAmount = fillPerc;
            currenthealth.text = playerHp.GetCurrentHealth().ToString("F0");
            maxHealth.text = "/" + playerHp.GetMaxHealth().ToString("F0");

            fillPerc = playerHp.GetRespawnTimer();

            if (fillPerc > 1)
                player_tmp.text = fillPerc.ToString("F0");
            else
                player_tmp.text = "";

            if (fillPerc >= 0)
                fillPerc /= playerHp.GetRespawnTime();
            player_Fill.fillAmount = fillPerc;


        }
        else {
            uiTimer += Time.deltaTime;
        }
    }
}