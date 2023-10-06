using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.DTOs
{
    public class Activity
    {
        public int Id { get; set; }
        public string ActivityName { get; set; }
        public DateTime When { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        
    }
}