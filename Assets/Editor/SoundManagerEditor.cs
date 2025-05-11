using _IUTHAV.Scripts.Core.Audio;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundAssets))]
public class SoundManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get reference to the target SoundManager component
        SoundAssets soundManager = (SoundAssets)target;

        // Draw the default inspector (shows all serialized fields)
        DrawDefaultInspector();

        // Ensure every new entry in the array has a default volume of 1
        if (soundManager.soundAudioClips != null)
        {
            foreach (var clip in soundManager.soundAudioClips)
            {
                if (clip != null) 
                    clip.Validate();  // Apply the fix
            }
        }

        // Save changes to the serialized object
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}