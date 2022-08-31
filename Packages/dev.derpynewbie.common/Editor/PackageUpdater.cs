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
            while (!request.IsCompleted)
                Thread.Sleep(1000);

            switch (request.Status)
            {
                case StatusCode.Success:
                    Debug.Log("NewbieCommons Package Update completed");
                    break;
                case StatusCode.Failure:
                    Debug.LogError(
                        $"NewbieCommons Package Update failed:{request.Error.errorCode} {request.Error.message}");
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