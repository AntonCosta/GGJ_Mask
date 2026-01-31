using System;
using GGJ.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GGJ.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private GameObject _inGameUI;
        [SerializeField] private GameObject _outsideShot;
        [SerializeField] private GameObject _insideShot;
        
        private LevelGenerator _levelGenerator;
        private bool _canPressSpace;
        private int _currentScreenCounter;

        private void Start()
        {
            _startButton.onClick.AddListener(GenerateGame);
            _levelGenerator = LevelGenerator.Instance;
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
    }
}