using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace knifehit
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private List<KnifeSO> _knifes = new List<KnifeSO>();
        [SerializeField]
        private List<LogSO> _logs = new List<LogSO>();
        [SerializeField]
        private List<LogSO> _bossLogs = new List<LogSO>();
        [SerializeField]
        private List<RotationCurve> _rotationPatterns = new List<RotationCurve>();
        [SerializeField]
        private Settings _settings;
        
        private Log _log;
        private GameObject _logGO;
        private SpriteRenderer _logSpriteRenderer;
        private GameObject _currentKnife;
        private SpriteRenderer _knifeSpriteRenderer;
        private int _currentLevel = 0;
        private int _currentKnifeSpriteNum;
        private bool _isBoss = false;
        private List<GameObject> _knifesGO = new List<GameObject>();
        private List<IExecute> _executableObjects = new List<IExecute>();

        public void StartNewGame()
        {
            _currentKnifeSpriteNum = 0;
            Load();
            InitializeNewGame();
            InitializeLevel();
        }

        public void ThrowKnife()
        {
            _currentKnife?.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10.0f, ForceMode2D.Impulse);
        }

        private void Update()
        {
            foreach (IExecute executable in _executableObjects)
            {
                executable.Execute();
            }
        }

        private void Load()
        {
            Timer timer = new Timer();
            _executableObjects.Add(timer);

            _logGO = Instantiate(Resources.Load<GameObject>(Names.LOG));
            _logSpriteRenderer = _logGO.GetComponent<SpriteRenderer>();
            _logGO.transform.position = _settings.logPosition;
            _log = new Log(_logGO, _rotationPatterns[0].curve);

            AddKnife();

            _executableObjects.Add(_log);
        }

        private void AddKnife()
        {
            GameObject newKnife = Instantiate(Resources.Load<GameObject>(Names.KNIFE));
            _currentKnife = newKnife;
            _currentKnife.GetComponent<Knife>().knifeHit += OnKnifeHit;
            _knifeSpriteRenderer = _currentKnife.GetComponent<SpriteRenderer>();
            _knifeSpriteRenderer.sprite = _knifes[_currentKnifeSpriteNum].knifeSprite;
            _currentKnife.transform.position = _settings.knifePosition;
            _knifesGO.Add(_currentKnife);
        }

        private void InitializeNewGame()
        {
            _currentLevel = 1;
        }

        private void InitializeLevel()
        {
            if (_currentLevel % 5 == 0)
            {
                _isBoss = true;
                _logSpriteRenderer.sprite = _bossLogs[Random.Range(0, _bossLogs.Count)].logSprite;
            }
            else
            {
                _isBoss = false;
                int n = Random.Range(0, _logs.Count);
                _logSpriteRenderer.sprite = _logs[n].logSprite;
            }
            
        }

        private void OnKnifeHit()
        {
            Debug.Log("Knife hit!");
            _currentKnife.GetComponent<Knife>().knifeHit -= OnKnifeHit;
            _currentKnife = null;
            StartCoroutine(AddKnifeWithDelay(1));
        }

        private IEnumerator AddKnifeWithDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AddKnife();
        }

        private void Dispose()
        {
            _executableObjects.Remove(_log);
            Destroy(_logGO);
            Destroy(_currentKnife);
            _log = null;
            _logSpriteRenderer = null;
        }
    }
}