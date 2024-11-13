using System.Text.Json;


namespace KeyPresser
{
    public static class JsonFileHelper
    {
        public static T GetObjectsFromJson<T>(string filePath)
        {
            var result = default(T);
            if (File.Exists(filePath))
            {
                var allText = File.ReadAllText(filePath);
                if (allText.Length > 1) { 
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        result = JsonSerializer.Deserialize<T>(allText);
                    }
                }
            }
            return result;
        }

        public static void SaveObjectsToJson<T>(T objectToSave, string filePath)
        {
            var directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using (var file = File.Create(filePath))
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true
                };

                
                JsonSerializer.Serialize(file, objectToSave, options);
                file.DisposeAsync();
            }
        }
    }
}
