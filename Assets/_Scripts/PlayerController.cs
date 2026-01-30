using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        
        private void Update()
        {
            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                LevelGenerator.Instance.NextMask();
            }

            if (Keyboard.current.aKey.wasPressedThisFrame)
            {
                LevelGenerator.Instance.PreviousMask();
            }
        }
    }
}