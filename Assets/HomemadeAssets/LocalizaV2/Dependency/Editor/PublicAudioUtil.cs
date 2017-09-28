using UnityEngine;
using System;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif
public static class PublicAudioUtil {
   
    public static void PlayClip(AudioClip clip) {
#if UNITY_EDITOR
        Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
        Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
        MethodInfo method = audioUtilClass.GetMethod(
            "PlayClip",
            BindingFlags.Static | BindingFlags.Public,
            null,
            new System.Type[] {
                typeof(AudioClip)
            },
            null
        );
        method.Invoke(
            null,
            new object[] {
                clip
            }
        );
#endif
    } // PlayClip()
	
     public static void StopAllClips() {
#if UNITY_EDITOR
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass =
                  unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "StopAllClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[]{},
                null
            );
            method.Invoke(
                null,
                new object[] {}
            );
#endif
        }

} // class PublicAudioUtil
 