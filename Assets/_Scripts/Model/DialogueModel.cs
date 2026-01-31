using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Models
{
    [Serializable]
    public class Dialogues
    {
        public List<DialogueTypes> DialogueData;
    }
    
    [Serializable]
    public class DialogueTypes
    {
        public string Personality;
        public List<DialogueKnowledgeGroup> Text;
    }
    
    [Serializable]
    public class DialogueKnowledgeGroup
    {
        public string Id;
        public List<DialogueEntry> Dialogues;
    }
    
    [Serializable]
    public class DialogueEntry
    {
        public string Id;
        public string Text;
    }
}