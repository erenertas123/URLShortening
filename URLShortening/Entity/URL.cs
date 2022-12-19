namespace URLShortening.Entity
{
    public class URL
    {
        public int Id { get; set; }
        public string FullUrl { get; set; }

        public string? ShortUrl { get; set; }
    }
}
