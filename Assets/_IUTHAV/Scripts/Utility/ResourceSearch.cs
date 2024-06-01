using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _IUTHAV.Scripts.Core.Audio;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _IUTHAV.Scripts.Utility {

    public static class ResourceSearch {
        
        private const string AudioclipPath = "Audioclips/";
        
        //Persistent Audio tables
        private static Hashtable _persistentAudioTable;
        private const string PersistentClipKey = "Generic";
        private static AudioGroupType[] _persistentAudioGroups = new AudioGroupType[] {
            AudioGroupType.UI,
            AudioGroupType.Music,
            AudioGroupType.SFX
        };
        
        //Scene dependent Audio SFX and AMB table
        private static Hashtable _sceneAudioTable;
        private static AudioGroupType[] _sceneAudioGroups = new AudioGroupType[] {
            AudioGroupType.Ambient,
            AudioGroupType.SFX
        };
        
        //Misc settings
        private const int MaxIndexSearch = 20;

        public static bool IsReadyAudio;

        private static bool IsDebug = true;

#region Public Functions

        public static void ConfigureAudio() {
            _sceneAudioTable = new Hashtable();
            _persistentAudioTable = new Hashtable();
            SceneManager.sceneLoaded += LoadSceneAudioTable;
            LoadSceneAudioTable(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            LoadPersistentAudioTables();

            IsReadyAudio = true;
        }

        public static void Dispose() {
            SceneManager.sceneLoaded -= LoadSceneAudioTable;
        }
        
        [CanBeNull]
        public static AudioClip GetAudioClip(string clipname) {
            
            Hashtable table = new Hashtable();
            if (clipname.Contains("generic")) {
                table = _persistentAudioTable;
            }
            else {
                table = _sceneAudioTable;
            }
            
            if (table == null) {
                LogWarning("Given Group type is not saved in persistent Audio! Try GetSFXClipFromScene!");
                return null;
            }

            return (AudioClip)table[clipname];
        }

        public static List<AudioClip> GetAllClips(bool getSceneAudio = false, string searchFilter = "") {
            
            Hashtable table = new Hashtable();
            if (getSceneAudio) {
                table = _sceneAudioTable;
            }
            else {
                table = _persistentAudioTable;
            }
            
            if (table == null || table.Count == 0) {
                LogWarning("Given Group type is not saved or empty!");
                return null;
            }

            List<AudioClip> clipList = new List<AudioClip>();
            
            foreach (AudioClip clip in table.Values) {
                
                if (clip.name.Contains(searchFilter)) {
                    clipList.Add(clip);
                }

            }
            
            return clipList;
        }

#endregion

#region Audio Table private Functions

        private static void LoadSceneAudioTable(Scene scene, LoadSceneMode mode) {

            Hashtable table = new Hashtable();
            AudioClip[] clips = Resources.LoadAll<AudioClip>(AudioclipPath + scene + "/");
            
            //string prefixFilter = scene.name.Replace("SCENE_", "");
            PopulateAudioTable(table, clips);
            
            Log("Loaded [" + table.Count + "] Audioclips in scene [" + scene.name +"]");
        }

        private static void LoadPersistentAudioTables() {

            Hashtable table = new Hashtable();
            AudioClip[] clips = Resources.LoadAll<AudioClip>(AudioclipPath + "Generic" + "/");
            
            PopulateAudioTable(table, clips);

            Log("Loaded [" + table.Count + "] Audioclips for persistent audio");
        }

        private static void PopulateAudioTable(Hashtable table, AudioClip[] clips, string prefixFilter = "") {
            
            foreach (var clip in clips) {
                if (prefixFilter == "" || clip.name.StartsWith(prefixFilter)) {
                    table.Add(clip.name, clip);
                } 
            }
        }

        private static void Log(string msg) {

            if (!IsDebug) return;
            Debug.Log("[ResourceSearch]" + msg);
        }
        private static void LogWarning(string msg) {
            
            if (!IsDebug) return;
            Debug.LogWarning("[ResourceSearch]" + msg);
        }

#endregion
    }
#region Helper Classes
    [Serializable]
    public struct Action {
        public String entityA;
        public String entityB;
        public string result;
    }
    public struct Actions {
        public Action[] action;
    }
    
    public struct BarkWrapper {
    
        public string audioClip;
        public string text;

        public BarkWrapper(string audioClip, string text) {
            this.audioClip = audioClip;
            this.text = text;
        }
    }

    public enum TableType {
        
        BarkTable,
        InteractionTable

    }
#endregion
    
    
}