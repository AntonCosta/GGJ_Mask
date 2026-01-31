using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Models
{
    [Serializable]
    public class MurderModel
    {
        public MurderData  MurderData;
    }

    [Serializable]
    public class MurderData
    {
        public List<Location>  MurderLocations;
    }

    [Serializable]
    public class Location
    {
        public int Id;
        public string Name;
    }
}