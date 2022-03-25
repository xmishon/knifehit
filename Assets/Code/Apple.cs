using System;
using UnityEngine;

namespace knifehit
{
    public class Apple : MonoBehaviour
    {
        public event Action knifeHitApple;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            knifeHitApple?.Invoke();
            GameObject appleGO = Instantiate(Resources.Load<GameObject>(Names.DESTROYED_APPLE));
            appleGO.transform.position = collision.transform.position;
            appleGO.transform.rotation = collision.transform.rotation;
            Destroy(gameObject);
        }
    }
}