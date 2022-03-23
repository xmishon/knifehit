using System.Collections;
using UnityEngine;

namespace knifehit
{
    class DestroyedLog : MonoBehaviour
    {
        [SerializeField]
        private GameObject _partOne;
        [SerializeField]
        private GameObject _partTwo;
        [SerializeField]
        private GameObject _partThree;

        private void Start()
        {
            _partOne.GetComponent<Rigidbody2D>().AddForce((Vector3.left + Vector3.up) * 5.0f, ForceMode2D.Impulse);
            _partTwo.GetComponent<Rigidbody2D>().AddForce((Vector3.right + Vector3.up) * 5.0f, ForceMode2D.Impulse);
            _partThree.GetComponent<Rigidbody2D>().AddForce((Vector3.down + Vector3.right) * 5.0f, ForceMode2D.Impulse);
            StartCoroutine(SelfDestroyWithDelay(1.5f));
        }

        public void SetSprites(Sprite[] sprites)
        {
            _partOne.GetComponent<SpriteRenderer>().sprite = sprites[0];
            _partTwo.GetComponent<SpriteRenderer>().sprite = sprites[1];
            _partThree.GetComponent<SpriteRenderer>().sprite = sprites[2];
        }

        private IEnumerator SelfDestroyWithDelay(float time)
        {
            yield return new WaitForSeconds(time);
            Destroy(gameObject);
        }
    }
}
