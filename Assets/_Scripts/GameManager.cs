using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private GameObject _mainMenu;
        
        private LevelGenerator _levelGenerator;

        private void Start()
        {
            _startButton.onClick.AddListener(GenerateGame);
            _levelGenerator = LevelGenerator.Instance;
        }

        private void GenerateGame()
        {
            _levelGenerator.GenerateLevel();
            _mainMenu.gameObject.SetActive(false);
        }
    }
}