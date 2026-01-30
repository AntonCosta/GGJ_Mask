using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Utils
{
    public static class Constants
    {
        public static readonly List<string> MASK_ROLES = new() { "Innocent", "Witness", "Guilty" };
        public static readonly List<string> MASK_PERSONALITY_TYPES = new() { "Calm", "Deceptive", "Nervous", "Aggressive", "Shady", "Vague" };
    }
}
