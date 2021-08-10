using System.Collections;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace ActionHelper.scripts.Editor
{
    public class Update
    {
        private const string Version = "2021.08.10_2";
        public static bool IsUpdated { get; set; }

        public static IEnumerator Check()
        {
            var baseURL = "https://api.github.com/repos/ribeKim/ActionHelper/releases/latest";

            var request = UnityWebRequest.Get(baseURL);
            yield return request.SendWebRequest();
            if (request.error == null)
            {
                var json = JsonConvert.DeserializeObject<GithubResponse>(request.downloadHandler.text);
                Validate(json?.Name);
            }

            yield return new WaitForSecondsRealtime(1f);
        }

        private static void Validate(string latestVersion)
        {
            if (GetActionHelperVersion() == latestVersion)
                IsUpdated = false;
            else
                IsUpdated = true;
        }

        private static string GetActionHelperVersion()
        {
            var str = "";
            var path = $"{Application.dataPath}/ActionHelper/version.txt";
            if (File.Exists(path))
            {
                var strArray = File.ReadAllLines(path);
                if (strArray.Length != 0) str = strArray[1];
            }
            else
            {
                str = Version;
            }

            return str;
        }
    }
}