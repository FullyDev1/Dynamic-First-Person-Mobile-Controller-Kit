using UnityEngine;
using UnityEngine.Events;

namespace FirstPersonMobileTools.Utility
{
        
    public class DropDownManager : MonoBehaviour {

        [SerializeField] private UnityEvent[] OnChangedValue;
        public void HandleInput(int value)
        {
            OnChangedValue[value]?.Invoke();
        }

    }
    
}
