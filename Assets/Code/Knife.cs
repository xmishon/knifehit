using System;
using UnityEngine;

namespace knifehit
{
    class Knife : MonoBehaviour
    {
        public event Action knifeHit;
        public event Action gameOver;
        

        [SerializeField]
        private AudioSource _knifeHitLog;
        [SerializeField]
        private AudioSource _knifeHitKnife;

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
                    _knifeHitLog.Play();
                    GameObject particles = Instantiate(Resources.Load<GameObject>(Names.FLINDERS));
                    particles.transform.position = collision.GetContact(0).point;
                    Destroy(particles, 2.0f);
                }
                else
                {
                    gameObject.GetComponent<Collider2D>().enabled = false;
                    gameOver?.Invoke();
                    gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(2.0f, -5.0f), ForceMode2D.Impulse);
                    gameObject.GetComponent<Rigidbody2D>().AddTorque(-4.0f, ForceMode2D.Impulse);
                    _knifeHitKnife.Play();
                }

            }
        }
    }
}
