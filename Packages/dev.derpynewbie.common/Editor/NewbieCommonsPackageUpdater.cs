using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DerpyNewbie.Common.Editor
{
    public static class NewbieCommonsPackageUpdater
    {
        [MenuItem("Newbie Commons/Update NewbieCommons")]
        public static void UpdatePackages()
        {
            var request =
                Client.Add("https://github.com/DerpyNewbie/NewbieCommons.git?path=/Packages/dev.derpynewbie.common");

            while (!request.IsCompleted)
            {
                Thread.Sleep(100);
                EditorUtility.DisplayProgressBar("Updating NewbieCommons Package", "Updating NewbieCommons Package", 0);
            }

            EditorUtility.ClearProgressBar();
            switch (request.Status)
            {
                case StatusCode.Success:
                    EditorUtility.DisplayDialog("Updating NewbieCommons Package",
                        $"Successfully updated NewbieCommons package to {request.Result.version}@{request.Result.git.hash}", "OK!");
                    break;
                case StatusCode.Failure:
                    EditorUtility.DisplayDialog("Updating NewbieCommons Package",
                        $"Failed to update Package\n{request.Error.message}", "OK!");
                    break;
                case StatusCode.InProgress:
                    Debug.LogWarning("NewbieCommons Package Update is in progress?");
                    break;
            }
        }
    }
}