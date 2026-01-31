using System;
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
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _inGameUI;
        [SerializeField] private GameObject _outsideShot;
        [SerializeField] private TextMeshProUGUI _outsideShotText;
        [SerializeField] private GameObject _insideShot;
        [SerializeField] private TextMeshProUGUI _insideShotText;

        public MurderModel MurderData => _murderModel;
        public string CrimeLocation => _crimeLocation;
        public string CrimeTime => _crimeTime;

        private MurderModel _murderModel;
        private IntroTextModel _introTextModel;
        private LevelGenerator _levelGenerator;
        private bool _canPressSpace;
        private int _currentScreenCounter;
        private string _crimeLocation;
        private string _crimeTime;

        private void Start()
        {
            _startButton.onClick.AddListener(GenerateGame);
            _levelGenerator = LevelGenerator.Instance;
            ReadMurderData();
            ReadIntroText();
            GenerateCrimeLocation();
            GenerateCrimeTime();
            GenerateIntroText();
        }

        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame && _canPressSpace)
            {
                _canPressSpace = false;
                _currentScreenCounter++;
                GoToNextScreen();
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
                    ScreenFader.Instance.FadeHide(ShowGame);
                    break;
            }
        }
        
        private void ShowOutsideShot()
        {
            _outsideShot.SetActive(true);
            _insideShot.SetActive(false);
            _canPressSpace = true;
        }

        private void ShowInsideShot()
        {
            _outsideShot.SetActive(false);
            _insideShot.SetActive(true);
            _canPressSpace = true;
        }

        private void ShowGame()
        {
            _outsideShot.SetActive(false);
            _insideShot.SetActive(false);
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
            _crimeLocation = _murderModel.MurderData.MurderLocations.RandomElement().Name;
        }

        private void GenerateCrimeTime()
        {
            _crimeTime = Enumerable.Range(0, 24)
                .Select(h => $"{h:00}:00")
                .ToList().RandomElement();
        }
    }
}