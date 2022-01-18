using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    [SerializeField] Image healthBar, a1_Icon, a2_Icon, a3_Icon, a1_Fill, a2_Fill, a3_Fill, player_Icon, player_Fill;
    [SerializeField] TMP_Text currenthealth, maxHealth, a1_tmp, a2_tmp, a3_tmp, player_tmp, shiniesCount;
    [SerializeField] GameObject tabPanel;

    private PlayerHealth playerHp;
    private Ability ab1, ab2, ab3;
    private Inputs inputs;
    float uiTimer = 0;
    private bool set = false;

    public void Setup(PlayerHealth hp) {
        //Icons

        //Cooldowns
        Ability[] tempAb = hp.GetComponents<Ability>();
        foreach (Ability a in tempAb) {
            if(a.input == 1) {
                ab1 = a;
            }
            else if(a.input == 2) {
                ab2 = a;
            }
            else if (a.input == 3) {
                ab3 = a;
            }
        }
        //Health bar & Respawn
        playerHp = hp;
        inputs = FindObjectOfType<Inputs>();
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

            #region Ability TMP
            fillPerc = ab1.GetCoolDownTimer();
            if (fillPerc > 1)
                a1_tmp.text = fillPerc.ToString("F0");
            else
                a1_tmp.text = "";

            fillPerc = ab2.GetCoolDownTimer();
            if (fillPerc > 1)
                a2_tmp.text = fillPerc.ToString("F0");
            else
                a2_tmp.text = "";

            fillPerc = ab3.GetCoolDownTimer();
            if (fillPerc > 1)
                a3_tmp.text = fillPerc.ToString("F0");
            else
                a3_tmp.text = "";
            #endregion

            //Cooldown fill for abilities
            a1_Fill.fillAmount = ab1.GetCoolDownRatio();
            a2_Fill.fillAmount = ab2.GetCoolDownRatio();
            a3_Fill.fillAmount = ab3.GetCoolDownRatio();

            if(inputs.GetCtrlTab() < 0) {
                tabPanel.SetActive(true);
            }
            else {
                tabPanel.SetActive(false);
            }
        }
        else {
            uiTimer += Time.deltaTime;
        }
    }
}