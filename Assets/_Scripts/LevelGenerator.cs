using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Models;
using UnityEngine;

namespace GGJ
{
    public class LevelGenerator : MonoBehaviour
    {
        private const string DIALOGUE_RESOURCE_PATH = "Data/mask_dialogue";
        private Dialogues _dialogueModel;

        private void Start()
        {
            ReadDialogue();
        }

        public void GenerateLevel()
        {
            
        }

        private void ReadDialogue()
        {
            _dialogueModel = JsonUtility.FromJson<Dialogues>(Resources.Load<TextAsset>(DIALOGUE_RESOURCE_PATH).text);
            Debug.Log(_dialogueModel.Dialogue_Lines.First().Text);
        }
    }
}

