namespace KeyPresser
{
    public interface IDataServices
    {
        Profile GetProfile(int id);
        List<ProfileListItem> GetProfilesList(bool includeChildProfiles);
        void SaveProfile(Profile profile);
    }
}
