namespace software_center
{
    public class PackageInfo
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public PackageVersion Available { get; set; }
        public PackageVersion Version { get; set; }
        public string Source { get; set; }
    }
}