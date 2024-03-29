using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;

namespace WrongWarp
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] public RectTransform rectTransform;
        [SerializeField] public RectTransform healthBarMaskTransform;
        [SerializeField] public RectTransform healthBarTransform;

        [SerializeField] public Image healthBarContainerImage;
        [SerializeField] public Image healthBarImage;

        [SerializeField] public Gradient healthGradient;

        [OnValueChanged("UpdateHealthAmount")]
        public float health;

        public float maxHealth;

        [Button]
        private void TestInit()
        {
            Init(100, 100);
            rectTransform.sizeDelta = new Vector2(100, Random.Range(5, 100));
        }

        public void Init(float health, float maxHealth, Vector2? scale = null)
        {
            this.health = Mathf.Min(health, maxHealth);
            this.maxHealth = Mathf.Max(health, maxHealth);

            rectTransform.sizeDelta = scale == null ? new Vector2(100, 9) : (Vector2)scale;
            UpdateHealthAmount();
        }

        public void UpdateHealthAmount()
        {
            if(health <= 0)
            {
                if(healthBarImage.gameObject.activeInHierarchy)
                {
                    healthBarImage.gameObject.SetActive(false);
                }
                return;
            }
            else
            {
                if(!healthBarImage.gameObject.activeInHierarchy)
                {
                    healthBarImage.gameObject.SetActive(true);
                }
            }

            float healthPercentage = health / maxHealth;

            // Set offset from right
            float offsetValue = Mathf.Clamp(healthPercentage * rectTransform.sizeDelta.x, 0, rectTransform.sizeDelta.x) - rectTransform.sizeDelta.x;
            healthBarMaskTransform.offsetMax = new Vector2(offsetValue, 0);
            healthBarTransform.offsetMax = new Vector2(-offsetValue, 0);

            // Set color to position in gradient
            healthBarImage.color = healthGradient.Evaluate(healthPercentage);
        }

        public void SetHealth(float health)
        {
            health = Mathf.Clamp(health, 0, maxHealth);
            this.health = health;
            UpdateHealthAmount();
        }
    }
}
