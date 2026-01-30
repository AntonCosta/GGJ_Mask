using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _exitButton;
        
        private LevelGenerator _levelGenerator;

        private void Start()
        {
            _startButton.onClick.AddListener(GenerateGame);
        }

        private void GenerateGame()
        {
            _levelGenerator.GenerateLevel();
        }
    }
}