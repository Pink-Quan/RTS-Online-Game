using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RTSOnlineGame
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] private Health health;
        [SerializeField] private GameObject healthBarParent;
        [SerializeField] private Image healthBarImage;

        private void Awake()
        {
            health.clientOnHeathUpdated += HandleHeathUpdated;
        }

        private void OnDestroy()
        {
            health.clientOnHeathUpdated -= HandleHeathUpdated;
        }

        private void OnMouseEnter()
        {
            healthBarParent.SetActive(true);
        }

        private void OnMouseExit()
        {
            healthBarParent.SetActive(false);
        }

        private void HandleHeathUpdated(int currentHealth,int maxHealth)
        {
            healthBarImage.fillAmount = (float)currentHealth/maxHealth;
        }
    }
}