public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required int Number { get; set; }
        public string? Address { get; set; }
    }