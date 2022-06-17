namespace software_center
{
    public class Header
    {
        public string Name { get; }
        public int Offset { get; }
        public Header Next { get; set; }

        public Header(string name, int offset)
        {
            Name = name;
            Offset = offset;
        }
    }
}