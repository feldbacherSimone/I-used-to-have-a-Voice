using UnityEngine;

namespace _IUTHAV.Scripts
{
    public class ModelChange : MonoBehaviour
    {
        [SerializeField] private GameObject[] models;

        public void ChangeModel(int index)
        {
            for (int i = 0; i < models.Length; i++)
            {
                models[i].SetActive(i == index);
            }
        }
    }
}
