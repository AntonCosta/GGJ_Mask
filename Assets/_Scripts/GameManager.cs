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

        
        [Header("Rain Audio")]
        [SerializeField] private AudioSource _rainSource;
        [SerializeField] private AudioLowPassFilter _rainLowPass;

        [SerializeField] private float _menuVolume = 0.6f;
        [SerializeField] private float _gameVolume = 0.25f;
        [SerializeField] private float _convictionVolume = 0f;

        [SerializeField] private float _menuCutoff = 22000f;
        [SerializeField] private float _gameCutoff = 3000f;
        [SerializeField] private float _convictionCutoff = 500f;

        [Header("Thunder Audio")]
        [SerializeField] private AudioSource _thunderSource;
        [SerializeField] private AudioClip _thunderClip;
        
        [Header("UI audio")]
        [SerializeField] private AudioSource _uiSource;
        
        [SerializeField] private AudioClip _clickClip;
        [SerializeField] private AudioClip _hoverClip;
        [SerializeField] private AudioClip _positiveClip;
        [SerializeField] private AudioClip _negativeClip;
        
        [Header("NPC Voice One Shots")]
        [SerializeField] private AudioSource _npcVoiceSource;
        [SerializeField] private AudioClip _voiceLow;
        [SerializeField] private AudioClip _voiceNormal;
        [SerializeField] private AudioClip _voiceHigh;

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
        }
        
        private void Start()
        {
            _startButton.onClick.AddListener(() =>
            {
                PlayUIPositive();
                GenerateGame();
            });
            _exitButton.onClick.AddListener(() =>
            {
                PlayUIClick();
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
            if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame) && (_youWon || _youLost))
            {
                _youWinUI.SetActive(false);
                _youLoseUI.SetActive(false);
                _convictionScreen.SetActive(false);
                PlayerController.Instance.Navigation.SetActive(false);
                _mainMenu.gameObject.SetActive(true);
                _levelGenerator = LevelGenerator.Instance;
                ReadMurderData();
                ReadIntroText();
                GenerateCrimeLocation();
                GenerateCrimeTime();
                GenerateIntroText();
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
            
            StartRainSFX(_menuVolume, _menuCutoff);
            
            
            _outsideShot.SetActive(true);
            _insideShot.SetActive(false);
            _tutorialShot.SetActive(false);
            _canPressSpace = true;
        }

        private void ShowInsideShot()
        {
            PlayThunder();
            
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
            StartRainSFX(_gameVolume, _gameCutoff);
            
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
            StartRainSFX(_convictionVolume, _convictionCutoff);
            
            _maskPositions.SetActive(false);
            _inGameUI.SetActive(false);
            _notepad.SetActive(false);
            PlayerController.Instance.Navigation.SetActive(false);
            _convictionScreen.SetActive(true);
            _convictionScreen.GetComponent<ConvictionPhaseController>().MoveMasksToConviction();
        }

        public void YouWin()
        {
            PlayUIPositive();
            _youWinUI.SetActive(true);
            _youWon = true;
        }

        public void YouLose()
        {
            PlayUINegative();
            _youLoseUI.SetActive(true);
            _youLost = true;
        }
        
      private void StartRainSFX(float volume, float cutoff)
      {
          if (!_rainSource.isPlaying)
              _rainSource.Play();
      
          StopCoroutine("FadeRainSFX");
          StartCoroutine(FadeRainSFX(volume, cutoff));
      }
      
      private IEnumerator FadeRainSFX(float targetVolume, float targetCutoff)
      {
          float startVol = _rainSource.volume;
          float startCut = _rainLowPass.cutoffFrequency;
      
          float t = 0f;
          float dur = 0.8f;
      
          while (t < dur)
          {
              t += Time.deltaTime;
              float k = t / dur;
      
              _rainSource.volume = Mathf.Lerp(startVol, targetVolume, k);
              _rainLowPass.cutoffFrequency = targetCutoff;
      
              yield return null;
          }
      
          _rainSource.volume = targetVolume;
          _rainLowPass.cutoffFrequency = targetCutoff;
      }

      private void PlayThunder()
      {
          if (_thunderSource != null && _thunderClip != null)
          {
              _thunderSource.PlayOneShot(_thunderClip);
          }
      }

      public void PlayUIClick()
      {
          PlayUI(_clickClip, 0.7f);
      }

      public void PlayUIHover()
      {
          PlayUI(_hoverClip, 0.4f);
      }

      public void PlayUIPositive()
      {
          PlayUI(_positiveClip, 0.8f);
      }

      public void PlayUINegative()
      {
          PlayUI(_negativeClip, 0.8f);
      }

      private void PlayUI(AudioClip clip, float vol = 1f)
      {
          if (_uiSource != null && clip != null)
              _uiSource.PlayOneShot(clip, vol);
      }
      public void PlayNpcVoiceOneShot(string voice)
      {
          if (_npcVoiceSource == null) return;

          AudioClip clip = _voiceNormal;

          if (voice == "Low") clip = _voiceLow;
          else if (voice == "High") clip = _voiceHigh;

          if (clip == null) return;

          _npcVoiceSource.PlayOneShot(clip, 1f);
      }



    }
}