namespace KeyPresser
{
    public class DataServices: IDataServices
    {
        private readonly string dataDirectory;

        public DataServices(string dataDirectory)
        {
            this.dataDirectory = dataDirectory;
        }

        public void SaveProfile(Profile profile)
        {
            string filePath = dataDirectory + profile.Id + ".json";
            JsonFileHelper.SaveObjectsToJson<Profile>(profile, filePath);
        }

        public Profile GetProfile(int id)
        {
            string filePath = dataDirectory + id + ".json";
            return JsonFileHelper.GetObjectsFromJson<Profile>(filePath);
        }

        public List<ProfileListItem> GetProfilesList(bool includeChildProfiles)
        {
            var result = new List<ProfileListItem>();
            foreach (var file in Directory.GetFiles(dataDirectory))
            {
                int id;
                int.TryParse(Path.GetFileNameWithoutExtension(file), out id);

                var profile = GetProfile(id);
                //result.Add(new ProfileListItem { Id = id, ParentId = 0, Name = profile.Name });
                //if (includeChildProfiles)
                //{
                //    foreach (var child in profile.ChildProfiles)
                //    {
                //        result.Add(new ProfileListItem { Id = child.Id, ParentId = id, Name = $"    {child.Name}" });
                //    }
                //}
            }

            return result;
        }
    }
}
