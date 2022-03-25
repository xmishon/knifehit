using UnityEngine;

namespace knifehit
{
    public class Log : IExecute
    {
        public event System.Action onKnifeHit;

        public AnimationCurve rotationCurve;

        private GameObject _log;
        private Vector3 _rotation = new Vector3(0f, 0f, 0f);
        private float _speed = 2.0f;

        public Log(GameObject log, AnimationCurve curve)
        {
            _log = log;
            rotationCurve = curve;
        }

        public void Execute()
        {
            Rotate();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            onKnifeHit?.Invoke();
        }

        private void Rotate()
        {
            _rotation.z = rotationCurve.Evaluate(Timer.levelTime) * Time.deltaTime;
            _rotation.z /= 0.0174533f; // radians to deegrees
            _log.transform.Rotate(_rotation * _speed);
        }
    }
}
