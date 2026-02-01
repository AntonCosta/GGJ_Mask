using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using GGJ.Controllers;
using GGJ.Models;
using GGJ.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


namespace GGJ.Managers
{
    public class GameManager : MonoBehaviour
    {
        private const string MURDER_DATA_PATH = "Data/murder_data";
        private const string INTRO_TEXT_DATA_PATH = "Data/introText";
        
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _nextPhase;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _inGameUI;
        [SerializeField] private GameObject _outsideShot;
        [SerializeField] private GameObject _maskPositions;
        [SerializeField] private GameObject _convictionScreen;
        [SerializeField] private TextMeshProUGUI _outsideShotText;
        [SerializeField] private GameObject _insideShot;
        [SerializeField] private TextMeshProUGUI _insideShotText;
        [SerializeField] private GameObject _tutorialShot;
        [SerializeField] private TextMeshProUGUI _tutorialShotText;
        [SerializeField] private GameObject _youWinUI;
        [SerializeField] private GameObject _youLoseUI;
        [SerializeField] private GameObject _notepad;

        [Header("Audio")]
        [SerializeField] private AudioManager audioManager;
        
        [Header("Rain Mix (per state)")]
        [Range(0f, 1f)] [SerializeField] private float _menuRainVolume = 0.6f;
        [Range(0f, 1f)] [SerializeField] private float _gameRainVolume = 0.25f;
        [Range(0f, 1f)] [SerializeField] private float _convictionRainVolume = 0f;
        public static GameManager Instance { get; private set; }
        public MurderModel MurderData => _murderModel;
        public string CrimeLocation => _crimeLocation;
        public string CrimeTime => _crimeTime;
        public GameObject ConvictionScreen => _convictionScreen;

        private MurderModel _murderModel;
        private IntroTextModel _introTextModel;
        private LevelGenerator _levelGenerator;
        private bool _canPressSpace;
        private int _currentScreenCounter;
        private string _crimeLocation;
        private string _crimeTime;
        private bool _youWon;
        private bool _youLost;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (audioManager == null)
            {
                audioManager = FindFirstObjectByType<AudioManager>();
            }
        }
        
        private void Start()
        {
            _startButton.onClick.AddListener(() =>
            {
                audioManager.PlayUIPositive();
                GenerateGame();
            });
            _exitButton.onClick.AddListener(() =>
            {
                audioManager.PlayUIClick();
                Application.Quit();
            });
            _levelGenerator = LevelGenerator.Instance;
            ReadMurderData();
            ReadIntroText();
            GenerateCrimeLocation();
            GenerateCrimeTime();
            GenerateIntroText();
        }

        private void Update()
        {
            if ((Keyboard.current.spaceKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) && _canPressSpace && !ScreenFader.Instance.IsTweening)
            {
                _canPressSpace = false;
                _currentScreenCounter++;
                GoToNextScreen();
            }
            if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame) && (_youWon || _youLost)  && !ScreenFader.Instance.IsTweening)
            {
                ScreenFader.Instance.FadeHide(() =>
                {
                    _youWon = false;
                    _youLost = false;
                    _youWinUI.SetActive(false);
                    _youLoseUI.SetActive(false);
                    _convictionScreen.SetActive(false);
                    _maskPositions.SetActive(true);
                    LevelGenerator.Instance.InterviewRooms.ForEach(Destroy);
                    LevelGenerator.Instance.InterviewRooms.Clear();
                    PlayerController.Instance.Navigation.SetActive(false);
                    _mainMenu.gameObject.SetActive(true);
                    _levelGenerator = LevelGenerator.Instance;
                    ReadMurderData();
                    ReadIntroText();
                    GenerateCrimeLocation();
                    GenerateCrimeTime();
                    GenerateIntroText();
                });
            }
        }

        private void ReadMurderData()
        {
            _murderModel = JsonUtility.FromJson<MurderModel>(Resources.Load<TextAsset>(MURDER_DATA_PATH).text);
        }
        
        private void ReadIntroText()
        {
            _introTextModel = JsonUtility.FromJson<IntroTextModel>(Resources.Load<TextAsset>(INTRO_TEXT_DATA_PATH).text);
        }

        private void GenerateGame()
        {
            _mainMenu.gameObject.SetActive(false);
            GoToNextScreen();
        }

        private void GoToNextScreen()
        {
            switch (_currentScreenCounter)
            {
                case 0:
                    ScreenFader.Instance.FadeHide(ShowOutsideShot);
                    break;
                case 1:
                    ScreenFader.Instance.FadeHide(ShowInsideShot);
                    break;
                case 2:
                    ScreenFader.Instance.FadeHide(ShowTutorialShot);
                    break;
                case 3:
                    ScreenFader.Instance.FadeHide(ShowGame);
                    _currentScreenCounter = 0;
                    break;
            }
        }
        
        private void ShowOutsideShot()
        {
            
            audioManager.StartRainSFX(_menuRainVolume, audioManager.menuCutoff);
            
            _outsideShot.SetActive(true);
            _insideShot.SetActive(false);
            _tutorialShot.SetActive(false);
            _canPressSpace = true;
        }

        private void ShowInsideShot()
        {
            audioManager.PlayThunder();
            
            _outsideShot.SetActive(false);
            _insideShot.SetActive(true);
            _tutorialShot.SetActive(false);
            _canPressSpace = true;
        }

        private void ShowTutorialShot()
        {
            _outsideShot.SetActive(false);
            _insideShot.SetActive(false);
            _tutorialShot.SetActive(true);
            _canPressSpace = true;
        }

        private void ShowGame()
        {
            audioManager.StartRainSFX(_gameRainVolume, audioManager.gameCutoff);
            
            _outsideShot.SetActive(false);
            _insideShot.SetActive(false);
            _tutorialShot.SetActive(false);
            _inGameUI.SetActive(true);
            _notepad.SetActive(true);
            _nextPhase.gameObject.SetActive(false);
            PlayerController.Instance.Navigation.SetActive(true);
            _levelGenerator.GenerateLevel();
        }

        private void GenerateIntroText()
        {
            _outsideShotText.text = _introTextModel.IntroText.OutsideText;
            _insideShotText.text = _introTextModel.IntroText.InsideText;
            
            var time = DateTime.ParseExact(_crimeTime, "HH:mm", CultureInfo.InvariantCulture);

            string time1 = time.AddHours(-1).ToString("HH:mm");
            string time2 = time.AddHours( 1).ToString("HH:mm");
            
            _insideShotText.text = _insideShotText.text
                .Replace("${LOCATION}", _crimeLocation)
                .Replace("${TIME_1}", time1)
                .Replace("${TIME_2}", time2);
        }

        private void GenerateCrimeLocation()
        {
            _crimeLocation = GetRandomLocation();
        }

        private void GenerateCrimeTime()
        {
            _crimeTime = GetRandomTime();
        }

        public string GetRandomLocation()
        {
            return _murderModel.MurderData.MurderLocations.RandomElement().Name;
        }

        public string GetRandomTime()
        {
            return Enumerable.Range(0, 24)
                .Select(h => $"{h:00}:00")
                .ToList().RandomElement();
        }
        
        public void ShowNextPhaseButton()
        {
            _nextPhase.gameObject.SetActive(true);
            _nextPhase.onClick.AddListener(() => ScreenFader.Instance.FadeHide(GoToConvictionPhase));
        }

        private void GoToConvictionPhase()
        {
            audioManager.StartRainSFX(_convictionRainVolume, audioManager.convictionCutoff);
            
            _maskPositions.SetActive(false);
            _inGameUI.SetActive(false);
            _notepad.SetActive(false);
            PlayerController.Instance.Navigation.SetActive(false);
            _convictionScreen.SetActive(true);
            _convictionScreen.GetComponent<ConvictionPhaseController>().MoveMasksToConviction();
        }

        public void YouWin()
        {
            audioManager.PlayUIPositive();
            _youWinUI.SetActive(true);
            _youWon = true;
        }

        public void YouLose()
        {
            audioManager.PlayUINegative();
            _youLoseUI.SetActive(true);
            _youLost = true;
        }




    }
}