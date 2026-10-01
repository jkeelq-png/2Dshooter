using UnityEngine;

namespace Tanks2D
{
    public class BulletMovement2D : MonoBehaviour
    {
        private float _speed = 0f;
        private bool _isInitialized = false;

        [Header("Damage Text Prefab")]
        [SerializeField] private GameObject damageTextPrefab;

        public void Initialize(float speed)
        {
            _speed = speed;
            _isInitialized = true;
            Destroy(gameObject, 4f);
        }

        private void Update()
        {
            if (!_isInitialized) return;
            transform.Translate(Vector3.right * _speed * Time.deltaTime, Space.Self);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollision(other.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void HandleCollision(GameObject hitObject)
        {
            if (hitObject.CompareTag("Enemy"))
            {
                PigEnemy enemy = hitObject.GetComponent<PigEnemy>();
                
                if (enemy != null)
                {
                    int currentDamage = UpgradeButton.BulletDamage;
                    enemy.ApplyDamage(currentDamage);
                    
                    if (damageTextPrefab != null)
                    {
                        Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}
