using GJJ.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        
        private void Update()
        {
            if (Keyboard.current.dKey.wasPressedThisFrame && !MaskManager.Instance.IsTweening)
            {
                MaskManager.Instance.NextMask();
            }

            if (Keyboard.current.aKey.wasPressedThisFrame && !MaskManager.Instance.IsTweening)
            {
                MaskManager.Instance.PreviousMask();
            }
        }
    }
}