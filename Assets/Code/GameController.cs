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
        [SerializeField]
        private GameObject _gameUIObj;
        [SerializeField]
        private GameObject _mainMenu;

        private GameUI _gameUI;
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
        private int _hitsToWin;
        private int _currHitsNum;
        private Timer _timer;

        public void StartNewGame()
        {
            _gameUI = _gameUIObj.GetComponent<GameUI>();
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
            _timer = new Timer();
            _executableObjects.Add(_timer);

            _logGO = Instantiate(Resources.Load<GameObject>(Names.LOG));
            _logSpriteRenderer = _logGO.GetComponent<SpriteRenderer>();
            _logGO.transform.position = _settings.logPosition;
            _log = new Log(_logGO, _rotationPatterns[0].curve);

            _executableObjects.Add(_log);
        }

        private void AddKnife()
        {
            GameObject newKnife = Instantiate(Resources.Load<GameObject>(Names.KNIFE));
            _currentKnife = newKnife;
            _currentKnife.GetComponent<Knife>().knifeHit += OnKnifeHit;
            _currentKnife.GetComponent<Knife>().gameOver += OnGameOver;
            _knifeSpriteRenderer = _currentKnife.GetComponent<SpriteRenderer>();
            _knifeSpriteRenderer.sprite = _knifes[_currentKnifeSpriteNum].knifeSprite;
            _currentKnife.transform.position = _settings.knifePosition;
            _knifesGO.Add(_currentKnife);
        }

        private void InitializeNewGame()
        {
            _currentLevel = 0;
        }

        private void InitializeLevel()
        {
            _currentLevel++;
            _currHitsNum = 0;
            _log.rotationCurve = _rotationPatterns[Random.Range(0, _rotationPatterns.Count)].curve;
            if (_currentLevel % 5 == 0)
            {
                _isBoss = true;
                _logSpriteRenderer.sprite = _bossLogs[Random.Range(0, _bossLogs.Count)].logSprite;
                _hitsToWin = Random.Range(_settings.knifeNumMinBoss, _settings.knifeNumMaxBoss);
            }
            else
            {
                _isBoss = false;
                int n = Random.Range(0, _logs.Count);
                _logSpriteRenderer.sprite = _logs[n].logSprite;
                _hitsToWin = Random.Range(_settings.knifeNumMin, _settings.knifeNumMax);
            }
            for(int i = 0; i < _logGO.transform.GetChildCount(); i++)
            {
                Destroy(_logGO.transform.GetChild(i).gameObject);
            }
            _gameUI.SetupKnifeCount(_hitsToWin);
            AddKnife();
        }

        private void OnKnifeHit()
        {
            Debug.Log("Knife hit!");
            _currentKnife.GetComponent<Knife>().knifeHit -= OnKnifeHit;
            _currentKnife.GetComponent<Knife>().gameOver -= OnGameOver;
            _currHitsNum++;
            _gameUI.SetupKnifeCount(_hitsToWin - _currHitsNum);
            if (_currHitsNum == _hitsToWin)
            {
                StartCoroutine(DestroyWithDelay(1.0f, _currentKnife));
                StartCoroutine(InitializeLevelWithDelay(1.0f));
                _currentKnife = null;
            }
            else
            {
                _currentKnife = null;
                StartCoroutine(AddKnifeWithDelay(0.1f));
            }
        }

        private IEnumerator DestroyWithDelay(float time, GameObject obj)
        {
            yield return new WaitForSeconds(time);
            Destroy(obj);
        }

        private IEnumerator InitializeLevelWithDelay(float time)
        {
            yield return new WaitForSeconds(time);
            InitializeLevel();
        }

        private void OnGameOver()
        {
            StartCoroutine(OnGameOverWithDelay(1.5f));
        }

        private IEnumerator OnGameOverWithDelay(float time)
        {
            yield return new WaitForSeconds(time);
            HideGameUI();
            ShowMainMenu();
            _currentKnife.GetComponent<Knife>().knifeHit -= OnKnifeHit;
            _currentKnife.GetComponent<Knife>().gameOver -= OnGameOver;
            Dispose();
        }

        private void HideGameUI()
        {
            _gameUIObj.gameObject.SetActive(false);
        }

        private void ShowMainMenu()
        {
            _mainMenu.gameObject.SetActive(true);
        }

        private IEnumerator AddKnifeWithDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AddKnife();
        }

        private void Dispose()
        {

            _executableObjects.Remove(_log);
            _executableObjects.Remove(_timer);
            Destroy(_logGO);
            Destroy(_currentKnife);
            _log = null;
            _logSpriteRenderer = null;
        }
    }
}