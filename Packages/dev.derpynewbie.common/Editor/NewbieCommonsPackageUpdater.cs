using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DerpyNewbie.Common.Editor
{
    public static class NewbieCommonsPackageUpdater
    {
        [MenuItem("DerpyNewbie/Update/Update NewbieCommons")]
        public static void UpdateNewbieCommonsPackage()
        {
            UpdatePackage("NewbieCommons", "https://github.com/DerpyNewbie/NewbieCommons.git?path=/Packages/dev.derpynewbie.common");
        }

        public static void UpdatePackage(string displayName, string gitUrl)
        {
            var request = Client.Add(gitUrl);

            while (!request.IsCompleted)
            {
                Thread.Sleep(100);
                EditorUtility.DisplayProgressBar($"Updating {displayName} Package", $"Updating {displayName} Package", 0);
            }

            EditorUtility.ClearProgressBar();
            switch (request.Status)
            {
                case StatusCode.Success:
                    EditorUtility.DisplayDialog($"Updating {displayName} Package",
                        $"Successfully updated {request.Result.displayName} package to {request.Result.version}@{request.Result.git.hash.Substring(0, 5)}",
                        "OK!");
                    break;
                case StatusCode.Failure:
                    EditorUtility.DisplayDialog($"Updating {displayName} Package",
                        $"Failed to update Package\n{request.Error.message}", "OK!");
                    break;
                case StatusCode.InProgress:
                    Debug.LogWarning($"{displayName} Package Update is in progress?");
                    break;
            }
        }
    }
}