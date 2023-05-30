using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FirstPersonMobileTools.Utility
{

    public class DisplaySliderValue : MonoBehaviour
    {

        public float Set_Value 
        {
            get { return Set_Value; } 
            set 
            { 
                if (Text != null) Text.text = Mathf.RoundToInt(value).ToString(); 
                else Debug.LogWarning("No TextmeshproUGUI", this);
            } 
        }

        [SerializeField] private Slider slider;

        private TextMeshProUGUI Text;

        private void Start() {
            Text = GetComponent<TextMeshProUGUI>();
            Text.text = slider.value.ToString();
        }
        
    }

}