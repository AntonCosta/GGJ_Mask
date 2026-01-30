using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Models
{
    [Serializable]
    public class Dialogues
    {
        public List<DialogueModel> DialogueLines;
    }
    
    [Serializable]
    public class DialogueModel
    {
        public int Id;
        public string Text;
    }
}