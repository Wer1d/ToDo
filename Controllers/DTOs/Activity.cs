using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ToDo.DTOs
{
    public class Activity
    {
        [JsonIgnore]
        public int Id { get; set; }

        /// <example>ทานข้าว</example>
        public string ActivityName { get; set; }

        // <example>2021-08-01T00:00:00</example>
        public DateTime When { get; set; }

        [JsonIgnore]
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        
    }
}