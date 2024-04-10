using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;
using Yarn.Unity.Example;

namespace _IUTHAV.Core_Programming.Dialogue {


    public class Director : MonoBehaviour {

        [SerializeField] private ComicBoxView lineView;
        [SerializeField] private GameObject continueButton;
        [SerializeField] private CharacterBox[] characterBoxes;

        private Dictionary<string, CharacterBox> _mCharacterBoxes;

#region Unity Functions

        private void Awake() {
            Configure();
        }

#endregion

#region Public Functions

        

#endregion

#region Private Functions

        private void Configure() {

            _mCharacterBoxes = new Dictionary<string, CharacterBox>();
            foreach (CharacterBox box in characterBoxes) {
                _mCharacterBoxes.Add(box.name, box);
            }

        }

        private IEnumerator WaitAndContinue(int waitTime) {

            yield return new WaitForSeconds(waitTime);
            lineView.OnContinueClicked();

        }

#endregion

    }
}
