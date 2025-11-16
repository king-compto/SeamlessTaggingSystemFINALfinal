namespace ClothesTagger.Models
{
    public class ClothingItem
    {
        public string ImagePath { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}

