using System;
using UnityEngine;

namespace GGJ.Models
{
    [Serializable]
    public class IntroTextModel
    {
        public IntroText IntroText;
    }
    
    [Serializable]
    public class IntroText
    {
        public string OutsideText;
        public string InsideText;
    }
}
