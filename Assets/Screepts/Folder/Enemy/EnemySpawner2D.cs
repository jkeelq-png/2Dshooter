using UnityEngine;

namespace Tanks2D
{
    public class EnemySpawner2D : MonoBehaviour
    {
        // Вспомогательный класс для настроек конкретного врага
        [System.Serializable]
        public class EnemySpawnConfig
        {
            [Tooltip("Имя для удобства отображения в инспекторе")]
            public string enemyName;
            
            [Tooltip("Префаб этого врага")]
            public GameObject enemyPrefab;
            
            [Range(0, 100), Tooltip("Шанс появления (относительный вес). Чем выше, тем чаще спавнится.")]
            public int spawnChance = 50;
        }

        [Header("References (Ссылки)")]
        [SerializeField, Tooltip("Список врагов и их индивидуальные настройки")]
        private EnemySpawnConfig[] _enemiesConfigs;

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
            if (_enemiesConfigs == null || _enemiesConfigs.Length == 0) return;

            if (Time.time >= _nextSpawnTime)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + _spawnRate;
            }
        }

        private void SpawnEnemy()
        {
            GameObject selectedPrefab = GetRandomEnemyPrefab();

            if (selectedPrefab == null) return;

            float randomY = Random.Range(_minY, _maxY);
            Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

            GameObject newEnemy = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);

            Vector3 safePos = newEnemy.transform.position;
            safePos.z = 0f;
            newEnemy.transform.position = safePos;
        }

        // Алгоритм взвешенного случайного выбора (Weighted Random)
        private GameObject GetRandomEnemyPrefab()
        {
            int totalWeight = 0;

            // 1. Считаем сумму всех шансов
            foreach (var config in _enemiesConfigs)
            {
                if (config.enemyPrefab != null)
                {
                    totalWeight += config.spawnChance;
                }
            }

            if (totalWeight == 0) return null;

            // 2. Генерируем случайное число в пределах общей суммы
            int randomWeight = Random.Range(0, totalWeight);
            int currentWeightSum = 0;

            // 3. Находим, в какой отрезок попало число
            foreach (var config in _enemiesConfigs)
            {
                if (config.enemyPrefab == null) continue;

                currentWeightSum += config.spawnChance;
                if (randomWeight < currentWeightSum)
                {
                    return config.enemyPrefab;
                }
            }

            return null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 top = new Vector3(transform.position.x, _maxY, 0f);
            Vector3 bottom = new Vector3(transform.position.x, _minY, 0f);
            Gizmos.DrawLine(top, bottom);
        }
    }
}
