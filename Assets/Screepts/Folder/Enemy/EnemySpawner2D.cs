using UnityEngine;

namespace Tanks2D
{
    public class EnemySpawner2D : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawnConfig
        {
            public string enemyName;
            public GameObject enemyPrefab;
            [Range(0, 100)] public int spawnChance = 50;
            [Min(0)] public int goldReward = 10;
        }

        [Header("References (Ссылки)")]
        [SerializeField] private EnemySpawnConfig[] _enemiesConfigs;

        [Header("Spawn Settings (Настройки появления)")]
        [SerializeField] private float _spawnRate = 2f;

        [Header("Spawn Zone Boundaries (Границы зоны спавна)")]
        [SerializeField] private float _minY = -4f;
        [SerializeField] private float _maxY = 4f;

        private float _nextSpawnTime = 0f;
        
        // ВНУТРЕННЯЯ ПЕРЕМЕННАЯ ДЛЯ ХРАНЕНИЯ СТЕНЫ
        private Wall _cachedWallOnScene;

        private void Start()
        {
            // На старте игры спавнер сам находит стену на сцене ОДИН РАЗ
            _cachedWallOnScene = Object.FindFirstObjectByType<Wall>();
            if (_cachedWallOnScene == null)
            {
                Debug.LogError("[EnemySpawner2D] Ошибка: Спавнер не смог найти объект со скриптом Wall на сцене!");
            }
        }

        private void Update()
        {
            if (ShopController.IsPaused) return;
            if (_enemiesConfigs == null || _enemiesConfigs.Length == 0) return;

            if (Time.time >= _nextSpawnTime)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + _spawnRate;
            }
        }

        private void SpawnEnemy()
        {
            EnemySpawnConfig selectedConfig = GetRandomEnemyConfig();
            if (selectedConfig == null || selectedConfig.enemyPrefab == null) return;

            float randomY = Random.Range(_minY, _maxY);
            Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

            GameObject newEnemy = Instantiate(selectedConfig.enemyPrefab, spawnPosition, Quaternion.identity);

            Vector3 safePos = newEnemy.transform.position;
            safePos.z = 0f;
            newEnemy.transform.position = safePos;

            PigEnemy enemyScript = newEnemy.GetComponent<PigEnemy>();
            if (enemyScript != null)
            {
                enemyScript.Initialize(selectedConfig.goldReward);
                
                // НОВОЕ: Спавнер лично передает ссылку на стену новорожденному мобу!
                enemyScript.SetTargetWall(_cachedWallOnScene);
            }
            else
            {
                Debug.LogWarning($"[EnemySpawner2D] На префабе {newEnemy.name} не найден скрипт PigEnemy!");
            }
        }

        private EnemySpawnConfig GetRandomEnemyConfig()
        {
            int totalWeight = 0;
            foreach (var config in _enemiesConfigs)
            {
                if (config.enemyPrefab != null && config.spawnChance > 0)
                {
                    totalWeight += config.spawnChance;
                }
            }

            if (totalWeight == 0) return null;

            int randomWeight = Random.Range(0, totalWeight);
            int currentWeightSum = 0;

            foreach (var config in _enemiesConfigs)
            {
                if (config.enemyPrefab == null || config.spawnChance <= 0) continue;

                currentWeightSum += config.spawnChance;
                if (randomWeight < currentWeightSum)
                {
                    return config;
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
