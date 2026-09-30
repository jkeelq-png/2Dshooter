using UnityEngine;

namespace Tanks2D
{
    public class BulletMovement2D : MonoBehaviour
    {
        private float _speed = 0f;
        private bool _isInitialized = false;

        [Header("Damage Text Prefab")]
        // Сюда в Инспекторе префаба пули мы просто перетащим твой готовый префаб цифр урона
        [SerializeField] private GameObject damageTextPrefab;

        // Метод для запуска пули из контроллера игрока
        public void Initialize(float speed)
        {
            _speed = speed;
            _isInitialized = true;

            // Удаляем пулю через 4 секунды автоматически, чтобы игра не лагала
            Destroy(gameObject, 4f);
        }

        private void Update()
        {
            if (!_isInitialized) return;

            // Движение вперед по направлению, в которое развернута пуля
            transform.Translate(Vector3.right * _speed * Time.deltaTime, Space.Self);
        }

        // ОБРАБОТКА СТОЛКНОВЕНИЯ (Если коллайдер пули настроен как триггер)
        private void OnTriggerEnter2D(Collider2D other)
        {
            HandleCollision(other.gameObject);
        }

        // ОБРАБОТКА СТОЛКНОВЕНИЯ (Если коллайдер пули твердый / физический)
        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleCollision(collision.gameObject);
        }

        // Единый метод для нанесения урона
        private void HandleCollision(GameObject hitObject)
        {
            // Проверяем, что попали именно во врага (у свинки в Unity должен стоять тег "Enemy")
            if (hitObject.CompareTag("Enemy"))
            {
                // Ищем скрипт свинки на объекте
                PigEnemy enemy = hitObject.GetComponent<PigEnemy>();
                
                if (enemy != null)
                {
                    // ИЗМЕНЕНО: Вместо фиксированных 10 единиц, берем текущий урон из скрипта улучшения!
                    int currentDamage = UpgradeButton.BulletDamage;
                    
                    enemy.ApplyDamage(currentDamage);
                    
                    // Спавним сочный всплывающий текст урона напрямую через префаб
                    if (damageTextPrefab != null)
                    {
                        Instantiate(damageTextPrefab, transform.position, Quaternion.identity);
                    }
                }

                // Уничтожаем пулю при попадании
                Destroy(gameObject);
            }
        }
    }
}
