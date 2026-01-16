using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_ANDROID
using UnityEditor;
using UnityEditor.Android;

class GradlePropertiesPatcher : IPostGenerateGradleAndroidProject
{
    public int callbackOrder { get
        {
            return 0;
        }
    }

    public void OnPostGenerateGradleAndroidProject(string path)
    {
        if (!EditorUserBuildSettings.buildAppBundle)
            return;
        string gradleProperties = null;
        var pathsToCheck = new[] { Path.Combine(path, "gradle.properties"), Path.Combine(path, "../gradle.properties") };
        foreach(var p in pathsToCheck)
        {
            if (File.Exists(p))
            {
                gradleProperties = p;
                break;
            }
        }
        if (string.IsNullOrEmpty(gradleProperties))
            throw new System.Exception("gradle.properties not found");
        var contents = File.ReadAllText(gradleProperties);
        var tag = "android.bundle.enableUncompressedNativeLibs";
        if (contents.Contains(tag))
        {
            Debug.Log("Skip adding " + tag);
            return;
        }
        contents += "\n" + tag + "=false";
        Debug.Log("Patching " + gradleProperties);
        File.WriteAllText(gradleProperties, contents);
    }
}
#endif
