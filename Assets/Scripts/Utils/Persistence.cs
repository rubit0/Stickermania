using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Domain;
using UnityEngine;

namespace Utils
{
    public static class Persistence
    {
        private const string folderName = "BinaryPlayerData";
        private const string fileName = "PlayerData.dat";
        private static string dataPath;

        static Persistence()
        {
            PrepareDataPath();
        }

        public static PlayerData LoadPlayerData(List<int> allStickers)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
            {
                // No data found - create base file
                if (fileStream.Length == 0)
                {
                    Debug.Log("player data file did not exist. created it!");
                    var playerData = PlayerData.Create(allStickers);
                    fileStream.Close();
                    SavePlayerData(playerData);

                    return playerData;
                }

                return (PlayerData)binaryFormatter.Deserialize(fileStream);
            }
        }

        public static void SavePlayerData(PlayerData playerData)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var fileStream = File.Open(dataPath, FileMode.OpenOrCreate))
            {
                binaryFormatter.Serialize(fileStream, playerData);
            }
        }

        public static void ClearPlayerData()
        {
            if (File.Exists(dataPath))
            {
                File.Delete(dataPath);
            }
        }

        private static void PrepareDataPath()
        {
            var folderPath = Path.Combine(Application.persistentDataPath, folderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            dataPath = Path.Combine(folderPath, fileName);
        }
    }
}
