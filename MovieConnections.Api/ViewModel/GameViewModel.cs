namespace MovieConnections.Api.ViewModel
{
    public class GameViewModel
    {
    }

    public class PopCornViewModel
    {
        public PopCornViewModel(int popcornPoint, int level)
        {
            PopcornPoint = popcornPoint;
            Level = level;
        }
        public int PopcornPoint { get; set; }
        public int Level { get; set; }
    }
}