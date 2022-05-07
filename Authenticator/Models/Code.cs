using SQLite;
using System;

namespace Authenticator.Models
{
    [Table("Codes")]
    public class Code
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }
        
        public byte[] SecretCode { get; set; }

        public TimeSpan TimeStep { get; set; }

        public string Algorithm { get; set; }

        public int Length { get; set; }
    }
}