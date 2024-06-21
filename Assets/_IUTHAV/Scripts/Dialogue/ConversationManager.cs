using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace _IUTHAV.Scripts.Dialogue {
    public class ConversationManager : MonoBehaviour {

        [SerializeField] private bool isDebug;
        [Header("Textbox Parameters")] [Tooltip("This script will search for any Characterboxes within the given ACT gameobject. It will check the Characterbox parameter 'characterName' and remember it, so it knows if a character is involved in this conversation")]
        [SerializeField] private CharacterBox[] characterBoxes;

        private Dictionary<string, CharBoxContainer> _comicBoxes;
        
        private void Awake() {

            if (characterBoxes == null || 
            characterBoxes.Length == 0 ||
            characterBoxes.Length != transform.childCount) {
                characterBoxes = gameObject.transform.GetComponentsInChildren<CharacterBox>();
            }
            PopulateConversation();
        }

#region Helper Classes
        [Serializable]
        private class CharBoxContainer {

            public List<CharacterBox> Boxes;
            public int CurrentIndex;

            public CharBoxContainer() {
                Boxes = new List<CharacterBox>();
            }
            public CharacterBox CurrentBox() {
                if (CurrentIndex < Boxes.Count) {
                    
                    return Boxes[CurrentIndex];
                }
                
                return Boxes[0];
            }

            public CharacterBox PreviousBox() {
                if (CurrentIndex-1 >= 0) {
                    
                    return Boxes[CurrentIndex-1];
                }
                
                return null;
            }

        }

#endregion

#region Public Functions

        public void ActivateBox(string cName) {
            
            if (_comicBoxes.TryGetValue(cName, out var cont)) {
                
                //Check if Previous box should be deactivated
                if (_comicBoxes[cName].PreviousBox() != null && _comicBoxes[cName].PreviousBox().hideBoxOnBoxChange) {
                    _comicBoxes[cName].PreviousBox().ToggleBubble(false);
                }
                
                cont.CurrentBox().ToggleBubble(true);
            }
            else {
                Log("Conversation doesn't contain any boxes for " + cName);
            }
            
        }

        public Vector3 GetContinueButtonPosition(string cName) {
            Vector3[] v = new Vector3[4];
            _comicBoxes[cName].CurrentBox().GetComponent<RectTransform>().GetWorldCorners(v);

            if (CurrentCharacterBox(cName).PlaceContinueButtonRight) {
                return v[0];
            }
            else {
                return v[3];
            }
        }
        
        public CharacterBox CurrentCharacterBox(string cName) {
            if (_comicBoxes.TryGetValue(cName, out var box)) {
                return box.CurrentBox();
            }
            
            LogWarning("No Characterbox found with Key: " + cName);
            return null;
        }

        public CharacterBox NextBox(string cName) {
            if (_comicBoxes.TryGetValue(cName, out var box)) {
                box.CurrentIndex++;
                return box.CurrentBox();
            }
            LogWarning("No Characterbox found with Key: " + cName);
            return null;
        }

        public List<CharacterBox> GetAllBoxes() {

            List<CharacterBox> result = new List<CharacterBox>();

            foreach (var container in _comicBoxes.Values) {
                result.AddRange(container.Boxes);
            }

            return result;
        }

        public string GetFirstCharacter() {

            foreach (string key in _comicBoxes.Keys) {
                return key;
            }

            return "";
        }

        public bool ContainsCharacter(string cName) {
            return _comicBoxes.ContainsKey(cName);
        }

#endregion

#region Private Functions

        private void PopulateConversation() {

            _comicBoxes = new Dictionary<string, CharBoxContainer>();

            foreach (var box in characterBoxes) {

                if (!_comicBoxes.ContainsKey(box.characterName)) {

                    _comicBoxes.Add(box.characterName, new CharBoxContainer());
                    _comicBoxes[box.characterName].Boxes.Add(box);
                    Log("New Character " + box.characterName);
                }
                else {
                    _comicBoxes[box.characterName].Boxes.Add(box);
                    Log("Added Box " + box.gameObject.name);
                }
                
            }
            
        }

        private void Log(string msg) {
            if (!isDebug) return;
            Debug.Log("[ComicBoxView] " + msg);
        }
        
        private void LogWarning(string msg) {
            if (!isDebug) return;
            Debug.LogWarning("[ComicBoxView] " + msg);
        }

#endregion
        
        
    }
}