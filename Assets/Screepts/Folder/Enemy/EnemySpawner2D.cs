using UnityEngine;

namespace Tanks2D
{
    public class EnemySpawner2D : MonoBehaviour
    {
        [Header("References (Ссылки)")]
        [SerializeField, Tooltip("Префаб вашего 2D-врага (треугольника)")]
        private GameObject _enemyPrefab;

        [Header("Spawn Settings (Настройки появления)")]
        [SerializeField, Tooltip("Задержка между появлением врагов в секундах")]
        private float _spawnRate = 2f;

        [Header("Spawn Zone Boundaries (Границы зоны спавна)")]
        [SerializeField, Tooltip("Минимальная высота (Y) появления врага")]
        private float _minY = -4f;
        
        [SerializeField, Tooltip("Максимальная высота (Y) появления врага")]
        private float _maxY = 4f;

        private float _nextSpawnTime = 0f;

        private void Update()
        {
            if (_enemyPrefab == null) return;

            // Генерируем врагов строго по таймеру
            if (Time.time >= _nextSpawnTime)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + _spawnRate;
            }
        }

        private void SpawnEnemy()
        {
            // Выбираем случайную высоту появления
            float randomY = Random.Range(_minY, _maxY);
            
            // ЖЕСТКАЯ ФИКСАЦИЯ: Координата Z строго равна 0. 
            // Это удерживает 2D-треугольники в зоне видимости ортографической камеры.
            Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

            // Создаем врага плоским к экрану (Quaternion.identity сбрасывает все 3D повороты)
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            
            // Дополнительная страховка: принудительно обнуляем Z координату у созданного клона
            Vector3 safePos = newEnemy.transform.position;
            safePos.z = 0f;
            newEnemy.transform.position = safePos;
        }

        // Отрисовка зоны спавна в редакторе Unity (зеленая вертикальная линия)
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 top = new Vector3(transform.position.x, _maxY, 0f);
            Vector3 bottom = new Vector3(transform.position.x, _minY, 0f);
            Gizmos.DrawLine(top, bottom);
        }
    }
}
