namespace Lamborghini.Models
{
    public class detailViewimgModel
    {
        public string? Title { get; set; }
        public List<Product?> Files { get; set; }
        public int? Count { get; set; }
        public string? Color { get; set; } // 三大Latest車系顏色

        public string? Subtype { get; set; } // 其餘5種車款
    }
}
