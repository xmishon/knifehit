using System.Collections;
using UnityEngine;

public class DestroyedApple : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _partOne;
    [SerializeField]
    private Rigidbody2D _partTwo;

    private void Awake()
    {
        _partOne.AddForce(Vector2.left * 2.0f, ForceMode2D.Impulse);
        _partTwo.AddForce(Vector2.right * 2.0f, ForceMode2D.Impulse);
        StartCoroutine(SelfDestroyWithDelay(2.0f));
    }

    private IEnumerator SelfDestroyWithDelay(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
