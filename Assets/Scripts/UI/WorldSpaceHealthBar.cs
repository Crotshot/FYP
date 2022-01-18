using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceHealthBar : MonoBehaviour
{
    [SerializeField] GameObject healthbarFab;
    [SerializeField] float yOffset;
    [SerializeField] bool minion;
    private Transform cameraTrans;
    private Image healthFill;

    public void SetupDelayed() {
        Invoke("Setup", 4f);
    }

    private void Setup() {
        GameObject localUI = Instantiate(healthbarFab, transform);
        GetComponent<Health>().HealthChanged.AddListener(HealthDisplay);
        healthFill = localUI.transform.GetChild(0).GetComponent<Image>();
        healthFill.transform.parent.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        cameraTrans = FindObjectOfType<Camera>().transform;
        if (minion) {
            localUI.transform.localScale = Vector3.one * 0.5f;
            if (GetComponent<Team>().GetTeam() == GameObject.Find("Local").GetComponent<Team>().GetTeam())
                healthFill.color = FindObjectOfType<Colors>().minionCol;
            else
                healthFill.color = FindObjectOfType<Colors>().enemyMinionCol;

        }
        else {
            healthFill.color = FindObjectOfType<Colors>().enemyConqCol;
        }
    }

    private void HealthDisplay(float health, float maxHealth) {
        healthFill.fillAmount = health/maxHealth;
        Debug.Log("Displayer Updated: " + health + ", " + maxHealth);
    }

    private void Update() {
        if(cameraTrans != null)
            healthFill.transform.parent.LookAt(cameraTrans);
    }
}