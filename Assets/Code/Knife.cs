using UnityEngine;

namespace knifehit
{
    class Knife : MonoBehaviour
    {
        public event System.Action knifeHit;
        public event System.Action gameLoose;

        private bool isHit = false;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isHit)
            {
                if (collision.collider.CompareTag("Log"))
                {
                    knifeHit?.Invoke();
                    gameObject.layer = LayerMask.NameToLayer("KnifeNotIntersect");
                    gameObject.transform.SetParent(collision.transform);
                    isHit = true;
                }
                else
                {
                    gameObject.GetComponent<Collider2D>().enabled = false;
                    gameLoose?.Invoke();
                    gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(2.0f, -5.0f), ForceMode2D.Impulse);
                    gameObject.GetComponent<Rigidbody2D>().AddTorque(-4.0f, ForceMode2D.Impulse);
                }

            }
        }
    }
}
