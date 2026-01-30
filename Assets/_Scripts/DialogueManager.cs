using System.Collections.Generic;
using GGJ.Models;
using GJJ.Managers;
using TMPro;
using UnityEngine;

namespace GGJ.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }
        [SerializeField] public List<GameObject> UIDialogueLines;
        
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
            UIDialogueLines.ForEach(line => line.SetActive(false));
        }

        public void AddText()
        {
            for (var i = 0; i < MaskManager.Instance.Masks.Count; i++)
            {
                var mask = MaskManager.Instance.Masks[i];
                switch (mask.PersonalityType)
                {
                    case "Calm":
                        UIDialogueLines[i].GetComponentInChildren<TextMeshProUGUI>().text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Deceptive":
                        UIDialogueLines[i].GetComponentInChildren<TextMeshProUGUI>().text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Nervous":
                        UIDialogueLines[i].GetComponentInChildren<TextMeshProUGUI>().text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Aggressive":
                        UIDialogueLines[i].GetComponentInChildren<TextMeshProUGUI>().text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Shady":
                        UIDialogueLines[i].GetComponentInChildren<TextMeshProUGUI>().text = _dialogues.DialogueLines[i].Text;
                        break;
                    case "Vague":
                        UIDialogueLines[i].GetComponentInChildren<TextMeshProUGUI>().text = _dialogues.DialogueLines[i].Text;
                        break;
                }
            }
        }

        public void ShowDialogue(int id)
        {
            UIDialogueLines[id].SetActive(true);
        }
        
        public void HideDialogue(int id)
        {
            UIDialogueLines[id].SetActive(false);
        }
    }
}