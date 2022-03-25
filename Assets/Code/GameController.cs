using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        private GameObject _gameOverMenu;
        [SerializeField]
        private Text _gameOverText;
        [SerializeField]
        private Sprite _vibrationOnSprite;
        [SerializeField]
        private Sprite _vibrationOffSprite;
        [SerializeField]
        private Image _vibrationButtonImage;
        [SerializeField]
        private SoundConfig _soundConfig;
        [SerializeField]
        private AudioSource _backgroundMusic;
        [SerializeField]
        private AudioSource _buttonPressSound;
        [SerializeField]
        private AudioSource _effectsSound;
        [SerializeField]
        private Text _appleCountText;
        [SerializeField]
        private Text _personalRecordText;

        private GameUI _gameUI;
        private Log _log;
        private GameObject _logGO;
        private SpriteRenderer _logSpriteRenderer;
        private GameObject _currentKnife;
        private SpriteRenderer _knifeSpriteRenderer;
        private int _currentLevel = 0;
        private int _currentKnifeSpriteNum;
        private bool _isBoss = false;
        private readonly List<GameObject> _knifesGO = new List<GameObject>();
        private readonly List<IExecute> _executableObjects = new List<IExecute>();
        private int _hitsToWin;
        private int _currHitsNum;
        private Timer _timer;
        private int _logSpriteNum;
        private bool _vibrate = true;
        private int _appleCount;
        private int _levelRecord;
        private readonly float _throwForce = 15.0f;
        private SaveData _saveData;

        public void StartNewGame()
        {
            _gameUI = _gameUIObj.GetComponent<GameUI>();
            _currentKnifeSpriteNum = 0;
            Load();
            InitializeNewGame();
            InitializeLevel();
        }

        public void PlayButtonPressSound()
        {
            _buttonPressSound.Play();
        }

        public void SwitchVibration()
        {
            _vibrate = !_vibrate;
            _vibrationButtonImage.sprite = _vibrate ? _vibrationOnSprite : _vibrationOffSprite;
            if (_vibrate)
                Vibration.Vibrate(20);
        }

        public void ThrowKnife()
        {
            _currentKnife?.GetComponent<Rigidbody2D>().AddForce(Vector2.up * _throwForce, ForceMode2D.Impulse);
            _effectsSound.Play();
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

        private void InitializeNewGame()
        {
            _currentLevel = 0;
        }

        private void InitializeLevel()
        {
            _effectsSound.clip = _soundConfig.knifeThrown;
            _currentLevel++;
            _currHitsNum = 0;
            _log.rotationCurve = _rotationPatterns[Random.Range(0, _rotationPatterns.Count)].curve;
            if (_currentLevel % 5 == 0)
            {
                _isBoss = true;
                _logSpriteNum = Random.Range(0, _bossLogs.Count);
                _logSpriteRenderer.sprite = _bossLogs[_logSpriteNum].logSprite;
                _hitsToWin = Random.Range(_settings.knifeNumMinBoss, _settings.knifeNumMaxBoss);
            }
            else
            {
                _isBoss = false;
                _logSpriteNum = Random.Range(0, _logs.Count);
                _logSpriteRenderer.sprite = _logs[_logSpriteNum].logSprite;
                _hitsToWin = Random.Range(_settings.knifeNumMin, _settings.knifeNumMax);
            }
            for(int i = 0; i < _logGO.transform.childCount; i++)
            {
                Destroy(_logGO.transform.GetChild(i).gameObject);
            }
            _logSpriteRenderer.enabled = true;
            _gameUI.SetupKnifeCount(_hitsToWin - 1);
            GenerateRandomKnifes();
            GenerateApple();

            AddKnife();
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

        public void GenerateRandomKnifes()
        {
            int num = Random.Range(1, 3);
            for (int i = 0; i < num; i++)
            {
                GameObject knife = Instantiate(Resources.Load<GameObject>(Names.KNIFE));
                knife.GetComponent<SpriteRenderer>().sprite = _knifes[_currentKnifeSpriteNum].knifeSprite;
                Vector2 randomV2 = Random.insideUnitCircle;
                Vector3 randomDirV3 = (new Vector3(randomV2.x, randomV2.y, 0.0f) - _logGO.transform.position).normalized;
                knife.transform.position = (_logGO.transform.position + randomDirV3) * 1.4f;
                //knife.transform.LookAt(_logGO.transform.position); // doesn't working
                knife.transform.up = _logGO.transform.position - knife.transform.position; // fix for LookAt()
                knife.gameObject.layer = LayerMask.NameToLayer("KnifeNotIntersect");
                knife.transform.SetParent(_logGO.transform);
            }
        }

        public void GenerateApple()
        {
            if(Random.Range(0.0f, 1.0f) < _settings.appleBonusProbability)
            {
                GameObject apple = Instantiate(Resources.Load<GameObject>(Names.APPLE));
                Vector2 randomV2 = Random.insideUnitCircle;
                Vector3 randomDirV3 = (new Vector3(randomV2.x, randomV2.y, 0.0f) - _logGO.transform.position).normalized;
                apple.transform.position = (_logGO.transform.position + randomDirV3) * 1.1f;
                apple.transform.up = -(_logGO.transform.position - apple.transform.position);
                apple.transform.SetParent(_logGO.transform);
                apple.GetComponent<Apple>().knifeHitApple += OnKnifeHitApple;
            }

        }

        private void OnKnifeHit()
        {
            Debug.Log("Knife hit!");
            _currentKnife.GetComponent<Knife>().knifeHit -= OnKnifeHit;
            _currentKnife.GetComponent<Knife>().gameOver -= OnGameOver;
            _currHitsNum++;
            _gameUI.SetupKnifeCount(_hitsToWin - _currHitsNum - 1);
            if (_currHitsNum == _hitsToWin)
            {
                OnLevelCleared();
            }
            else
            {
                _currentKnife = null;
                StartCoroutine(AddKnifeWithDelay(_settings.knifeSpawnDelay));
                if(_vibrate)
                    Vibration.Vibrate(30);
                StartCoroutine(ShutterLog());
            }
        }
        
        private IEnumerator ShutterLog()
        {
            _logGO.transform.position += Vector3.up * 0.05f;
            yield return new WaitForSeconds(0.1f);
            _logGO.transform.position -= Vector3.up * 0.05f;
        }

        private void OnKnifeHitApple()
        {
            _appleCount++;
            _appleCountText.text = _appleCount.ToString();
            _saveData.Save(new SaveDataItem(_appleCount, _levelRecord));
        }

        public void OnLevelCleared()
        {
            if (_isBoss)
            {
                if (_currentKnifeSpriteNum < _knifes.Count - 1)
                    _currentKnifeSpriteNum++;
            }
            StartCoroutine(DestroyWithDelay(1.0f, _currentKnife));
            StartCoroutine(InitializeLevelWithDelay(1.0f));
            _currentKnife = null;
            SpawnDestroyedLog();
            _logGO.GetComponent<SpriteRenderer>().enabled = false;
            for (int i = 0; i < _logGO.transform.childCount; i++)
            {
                Vector2 random = new Vector2(Random.Range(-3.0f, 3.0f), Random.Range(-6.0f, 6.0f));
                _logGO.transform.GetChild(i).GetComponent<Rigidbody2D>().AddForce(random, ForceMode2D.Impulse);
                _logGO.transform.GetChild(i).GetComponent<Rigidbody2D>().AddTorque(Random.Range(-4.0f, 4.0f), ForceMode2D.Impulse);
            }
            if (_vibrate)
                Vibration.Vibrate(new long[] { 30, 150, 200 }, -1);
            if (_currentLevel > _levelRecord)
            {
                _levelRecord = _currentLevel;
                _saveData.Save(new SaveDataItem(_appleCount, _levelRecord));
                _personalRecordText.text = $"Record: {_levelRecord}";
            }

        }

        private void SpawnDestroyedLog()
        {
            _logSpriteRenderer.enabled = false;
            GameObject destroyedLog = Instantiate<GameObject>(Resources.Load<GameObject>(Names.DESTROYED_LOG));
            destroyedLog.transform.position = _logGO.transform.position;
            destroyedLog.transform.rotation = _logGO.transform.rotation;
            if (_isBoss)
            {
                destroyedLog.GetComponent<DestroyedLog>().SetSprites(_bossLogs[_logSpriteNum].destroyedSprites);
            }
            else
            {
                destroyedLog.GetComponent<DestroyedLog>().SetSprites(_logs[_logSpriteNum].destroyedSprites);
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
            _currentKnife.GetComponent<Knife>().knifeHit -= OnKnifeHit;
            _currentKnife.GetComponent<Knife>().gameOver -= OnGameOver;
            _currentKnife = null;
            StartCoroutine(OnGameOverWithDelay(1.5f));
            if (_vibrate)
                Vibration.Vibrate(400);
            _effectsSound.clip = _soundConfig.gameOver;
            _backgroundMusic.volume = 0.3f;
            _effectsSound.Play();
            StartCoroutine(RestoreBackroundMusicVolume(2.0f));
            _saveData.Save(new SaveDataItem(_appleCount, _levelRecord < _currentLevel - 1 ? _currentLevel - 1 : _levelRecord));
        }

        private IEnumerator RestoreBackroundMusicVolume(float time)
        {
            yield return new WaitForSeconds(time);
            _backgroundMusic.volume = 1.0f;
        }

            private IEnumerator OnGameOverWithDelay(float time)
        {
            yield return new WaitForSeconds(time);
            HideGameUI();
            ShowGameOverMenu();
            Dispose();
        }

        private void HideGameUI()
        {
            _gameUIObj.gameObject.SetActive(false);
        }

        private void ShowGameOverMenu()
        {
            _gameOverMenu.gameObject.SetActive(true);
            _gameOverText.text = $"Levels cleared: {_currentLevel - 1}";
        }

        private IEnumerator AddKnifeWithDelay(float time)
        {
            yield return new WaitForSeconds(time);
            AddKnife();
        }

        private void Awake()
        {
            _saveData = new SaveData();
            SaveDataItem saveDataItem = _saveData.Load();
            _appleCount = saveDataItem.appleCount;
            _levelRecord = saveDataItem.levelRecord;
            _appleCountText.text = _appleCount.ToString();
            _personalRecordText.text = $"Record: {_levelRecord}";

            Vibration.Init();
            InitializeSound();
            _vibrationButtonImage.sprite = _vibrate ? _vibrationOnSprite : _vibrationOffSprite;
        }

        private void InitializeSound()
        {
            _buttonPressSound.clip = _soundConfig.buttonPressed;
            _backgroundMusic.clip = _soundConfig.backgroundSong;
            _effectsSound.clip = _soundConfig.knifeThrown;
            _backgroundMusic.Play();
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