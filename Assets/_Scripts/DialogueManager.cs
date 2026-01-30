using System.Collections.Generic;
using GGJ.Controllers;
using GGJ.Models;
using GJJ.Managers;
using TMPro;
using UnityEngine;

namespace GGJ.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        
        private Dialogues _dialogues;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void AddDialogues(Dialogues dialogues)
        {
            _dialogues = dialogues;
        }

        public void AddText(List<MaskController> masks)
        {
            for (var i = 0; i < masks.Count; i++)
            {
                var mask = masks[i];
                switch (mask.PersonalityType)
                {
                    case "Calm":
                        mask.Dialogue.text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Deceptive":
                        mask.Dialogue.text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Nervous":
                        mask.Dialogue.text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Aggressive":
                        mask.Dialogue.text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Shady":
                        mask.Dialogue.text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Vague":
                        mask.Dialogue.text = _dialogues.DialogueLines[i].Text;
                        break;
                }
            }
        }
    }
}