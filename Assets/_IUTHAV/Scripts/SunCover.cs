using UnityEngine;

namespace _IUTHAV.Scripts {
    public class SunCover : MonoBehaviour
    {
        private Quaternion initRotation;
        private Quaternion newRotation; 
        private bool isUp;
        [SerializeField] private float rotationAmount; 
        [SerializeField] private GameObject toggleActive; 
    
        void Start()
        {
            initRotation = transform.rotation; 
            newRotation = Quaternion.Euler(initRotation.eulerAngles.x + rotationAmount, initRotation.eulerAngles.y, initRotation.eulerAngles.z);
        }

        public void ToggleSunCover()
        {
            transform.rotation = isUp ? initRotation : newRotation;
            isUp = !isUp; 
        }

        public void ToggleActive() {
            toggleActive.SetActive(!toggleActive.activeSelf);
        }

    }
}
