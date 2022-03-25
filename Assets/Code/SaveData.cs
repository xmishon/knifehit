using System;
using System.IO;
using UnityEngine;

namespace knifehit
{
    public class SaveData
    {
        private readonly string _fileName = "SaveData.dat";

        private string _path;

        public SaveData()
        {
            _path = Application.persistentDataPath + "/" + _fileName;
        }

        public void Save(SaveDataItem item)
        {
            var str = JsonUtility.ToJson(item);
            File.WriteAllText(_path, str);
        }

        public SaveDataItem Load()
        {
            try
            {
                var str = File.ReadAllText(_path);
                return JsonUtility.FromJson<SaveDataItem>(str);
            }
            catch
            {
                return new SaveDataItem();
            }
        }
    }

    [Serializable]
    public struct SaveDataItem
    {
        public int appleCount;
        public int levelRecord;
        public SaveDataItem(int apples = 0, int levels = 0)
        {
            appleCount = apples;
            levelRecord = levels;
        }
    }
}
