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
    GameObject localUI;

<<<<<<< Updated upstream
    public void SetupDelayed() {
        Invoke("Setup", 4f);
    }

    public void Setup() {
        localUI = Instantiate(healthbarFab, transform);
        GetComponent<Health>().HealthChanged.AddListener(HealthDisplay);
        healthFill = localUI.transform.GetChild(0).GetComponent<Image>();
        healthFill.transform.parent.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        cameraTrans = FindObjectOfType<Camera>().transform;
        int team = GetComponent<Team>().GetTeam();
        if (minion) {
            localUI.transform.localScale = Vector3.one * 0.5f;
            if (team == 1) {
                healthFill.color = FindObjectOfType<Colors>().redMinionCol;
            }
            else {
                healthFill.color = FindObjectOfType<Colors>().blueMinionCol;
            }
        }
        else {
            healthFill.color = FindObjectOfType<Colors>().enemyConqCol;
        }

        localUI.SetActive(false);
    }

=======
    private void Start() {
        Invoke("Setup", 2f);
    }

    public void Setup() {
        GetComponent<Health>().HealthChanged.AddListener(HealthDisplay);
        localUI = Instantiate(healthbarFab, transform);
        healthFill = localUI.transform.GetChild(0).GetComponent<Image>();
        healthFill.transform.parent.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        cameraTrans = FindObjectOfType<HealthBarTarget>().GetCamera();
        OnRespawn();
    }

    public void OnRespawn() {
        int team = GetComponent<Team>().GetTeam();
        if (minion) {
            localUI.transform.localScale = Vector3.one * 0.5f;
            healthFill.color = team == 1 ? FindObjectOfType<Colors>().redMinionCol : team == 2 ? FindObjectOfType<Colors>().blueMinionCol : FindObjectOfType<Colors>().black;
        }//Black color is for Debugging
        else{
            healthFill.color = team == 1 ? FindObjectOfType<Colors>().redConqCol : team == 2 ? FindObjectOfType<Colors>().blueConqCol : FindObjectOfType<Colors>().black;
        }
        localUI.SetActive(false);
    }


>>>>>>> Stashed changes
    private void HealthDisplay(float health, float maxHealth) {
        if (health == maxHealth || health <= 0)
            localUI.SetActive(false);
        else
            localUI.SetActive(true);
        healthFill.fillAmount = health/maxHealth;
        //Debug.Log("Displayer Updated: " + health + ", " + maxHealth);
    }

    private void Update() {
        if(cameraTrans != null)
            healthFill.transform.parent.LookAt(cameraTrans);
    }
}