using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTransform : MonoBehaviour {

    [SerializeField] private Transform[] transformPoints;

    private int _currentIndex;

    public void NextTransform() {

        if (_currentIndex < transformPoints.Length) {
            
            transform.SetLocalPositionAndRotation(transformPoints[_currentIndex].localPosition, transformPoints[_currentIndex].localRotation);

            _currentIndex++;
        }
        
    }

}
