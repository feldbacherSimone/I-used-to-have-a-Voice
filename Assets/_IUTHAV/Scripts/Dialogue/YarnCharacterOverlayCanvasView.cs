using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Unity.Example;

namespace _IUTHAV.Scripts.Dialogue {
    public class YarnCharacterOverlayCanvasView : DialogueViewBase {
        
        public static YarnCharacterOverlayCanvasView instance; // very minimal implementation of singleton manager (initialized lazily in Awake)
        public List<YarnCharacter> allCharacters = new List<YarnCharacter>(); // list of all YarnCharacters in the scene, who register themselves in YarnCharacter.Start()
        //Camera worldCamera; // this script assumes you are using a full-screen Unity UI canvas along with a full-screen game camera
        
        //IUTHAV dev note: you assumed wrong

        [Tooltip("display dialogue choices for this character, and display any no-name dialogue here too")]
        public YarnCharacter playerCharacter;
        YarnCharacter speakerCharacter;

        [Tooltip("for best results, set the rectTransform anchors to middle-center, and make sure the rectTransform's pivot Y is set to 0")]
        public RectTransform dialogueBubbleRect, optionsBubbleRect;

        void Awake()
        {
            // ... this is important because we must set the static "instance" here, before any YarnCharacter.Start() can use it
            instance = this; 
        }

        /// <summary>automatically called by YarnCharacter.Start() so that YarnCharacterView knows they exist</summary>
        public void RegisterYarnCharacter(YarnCharacter newCharacter)
        {
            if (!YarnCharacterOverlayCanvasView.instance.allCharacters.Contains(newCharacter))
            {
                allCharacters.Add(newCharacter);
            }
        }

        /// <summary>automatically called by YarnCharacter.OnDestroy() to clean-up</summary>
        public void ForgetYarnCharacter(YarnCharacter deletedCharacter)
        {
            if (YarnCharacterOverlayCanvasView.instance.allCharacters.Contains(deletedCharacter))
            {
                allCharacters.Remove(deletedCharacter);
            }
        }

        public override void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            // Try and get the character name from the line
            string characterName = dialogueLine.CharacterName;

            // if null, Update() will use the playerCharacter instead
            speakerCharacter = !string.IsNullOrEmpty(characterName) ? FindCharacter(characterName) : null;

            // IMPORTANT: we must mark this view as having finished its work, or else the DialogueRunner gets stuck forever
            onDialogueLineFinished();
        }

        /// <summary>simple search through allCharacters list for a matching name, returns null and LogWarning if no match found</summary>
        YarnCharacter FindCharacter(string searchName)
        {
            foreach (var character in allCharacters)
            {
                if (character.characterName == searchName)
                {
                    return character;
                }
            }

            Debug.LogWarningFormat("YarnCharacterView couldn't find a YarnCharacter named {0}!", searchName );
            return null;
        }

        void Update()
        {
            // this all in Update instead of RunLine because characters might walk around or move during the dialogue
            if (dialogueBubbleRect.gameObject.activeInHierarchy)
            {
                if (speakerCharacter != null) 
                {
                    //dialogueBubbleRect.anchoredPosition = WorldToAnchoredPosition(dialogueBubbleRect, speakerCharacter.positionWithOffset, bubbleMargin);
                    dialogueBubbleRect.position = speakerCharacter.GetComponent<RectTransform>().position;
                } 
                else 
                {   // if no speaker defined, then display speech above playerCharacter as a default
                    //dialogueBubbleRect.anchoredPosition = WorldToAnchoredPosition(dialogueBubbleRect, playerCharacter.positionWithOffset, bubbleMargin);
                    dialogueBubbleRect.position = speakerCharacter.GetComponent<RectTransform>().position;
                }
            }

            // put choice option UI above playerCharacter
            if (optionsBubbleRect.gameObject.activeInHierarchy)
            {
                //optionsBubbleRect.anchoredPosition = WorldToAnchoredPosition(optionsBubbleRect, playerCharacter.positionWithOffset, bubbleMargin);
                dialogueBubbleRect.position = speakerCharacter.GetComponent<RectTransform>().position;
            }
        }
        
    }
}