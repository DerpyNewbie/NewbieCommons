using System.Threading;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DerpyNewbie.Common.Editor
{
    [InitializeOnLoad]
    public class PackageUpdater
    {
        public delegate void PackageUpdateEventHandler();

        public static event PackageUpdateEventHandler UpdateEvent;

        [MenuItem("Newbie Commons/Update Packages")]
        public static void UpdatePackages()
        {
            UpdateEvent?.Invoke();
        }

        private static void UpdateNewbieCommonsPackage()
        {
            var request =
                Client.Add("https://github.com/DerpyNewbie/NewbieCommons.git?path=/Packages/dev.derpynewbie.common");

            var progress = 0F;
            while (!request.IsCompleted)
            {
                EditorUtility.DisplayProgressBar(
                    "Updating Package",
                    "Updating NewbieCommons Package",
                    progress += 0.1F);
                Thread.Sleep(100);
            }

            EditorUtility.ClearProgressBar();

            switch (request.Status)
            {
                case StatusCode.Success:
                    Debug.Log($"{request.Result.displayName} is now {request.Result.version}");
                    break;
                case StatusCode.Failure:
                    Debug.LogError($"{request.Error.message}");
                    break;
                case StatusCode.InProgress:
                    Debug.LogWarning("NewbieCommons Package Update is in progress?");
                    break;
            }
        }

        static PackageUpdater()
        {
            UpdateEvent += UpdateNewbieCommonsPackage;
        }
    }
}