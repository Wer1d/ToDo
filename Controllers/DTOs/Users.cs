namespace ToDo.DTOs
{
    public class UserDTO
    {
        
        public uint Id { get; }

        /// <example>Bernard</example>
        public string? Username { get; set; } 

        /// <example>12345</example>
        public string? Password { get; set; } 
        
    }
}