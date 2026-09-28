using UnityEngine;

namespace Tanks2D
{
    public class EnemyHealthBar2D : MonoBehaviour
    {
        private GameObject _backgroundObj;
        private GameObject _fillObj;
        
        private float _maxHealth;
        // ИСПРАВЛЕНО: Сделали полоску еще короче (было 0.45, стало 0.28)
        private float _maxWidth = 0.28f; 

        public void CreateHealthBar(int maxHealth, float offsetY)
        {
            _maxHealth = maxHealth;

            // 1. Создаем черный задний фон
            _backgroundObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(_backgroundObj.GetComponent<Collider>()); 
            _backgroundObj.name = "HP_Background";
            _backgroundObj.transform.SetParent(transform);
            
            _backgroundObj.transform.localPosition = new Vector3(0f, offsetY, 0f);
            _backgroundObj.transform.localRotation = Quaternion.identity;
            
            // ИСПРАВЛЕНО: Сделали подложку ультра-тонкой (высота теперь всего 0.04)
            _backgroundObj.transform.localScale = new Vector3(_maxWidth + 0.03f, 0.04f, 1f); 

            Renderer bgRenderer = _backgroundObj.GetComponent<Renderer>();
            if (bgRenderer != null)
            {
                bgRenderer.material = new Material(Shader.Find("Sprites/Default"));
                bgRenderer.material.color = new Color(0.1f, 0.1f, 0.1f, 1f);
                bgRenderer.sortingOrder = 399; 
            }

            // 2. Создаем саму полоску здоровья (Красную заливку)
            _fillObj = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(_fillObj.GetComponent<Collider>()); 
            _fillObj.name = "HP_Fill";
            _fillObj.transform.SetParent(transform);
            
            _fillObj.transform.localPosition = new Vector3(0f, offsetY, -0.01f); 
            _fillObj.transform.localRotation = Quaternion.identity;
            
            // ИСПРАВЛЕНО: Высота красной линии теперь всего 0.024 — она будет как тонкий пиксельный маркер
            _fillObj.transform.localScale = new Vector3(_maxWidth, 0.024f, 1f);

            Renderer fillRenderer = _fillObj.GetComponent<Renderer>();
            if (fillRenderer != null)
            {
                fillRenderer.material = new Material(Shader.Find("Sprites/Default"));
                fillRenderer.material.color = Color.red; 
                fillRenderer.sortingOrder = 400; 
            }
        }

        public void UpdateHealth(int currentHealth)
        {
            if (_fillObj == null) return;

            float healthPercent = (float)currentHealth / _maxHealth;
            healthPercent = Mathf.Clamp01(healthPercent);

            Vector3 newScale = _fillObj.transform.localScale;
            newScale.x = _maxWidth * healthPercent;
            _fillObj.transform.localScale = newScale;

            float xOffset = (_maxWidth - newScale.x) / 2f;
            Vector3 newPos = _fillObj.transform.localPosition;
            newPos.x = -xOffset;
            _fillObj.transform.localPosition = newPos;
        }
    }
}
