using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace security_service.Utils.Classes
{
    public class User
    {    
        public int Id { get; set; }
      
        public string? Login { get; set; }
      
        public string? FirstName { get; set; }
     
        public string? LastName { get; set; }
       
        public string? Email { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Sex Sex { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role Role { get; set; }
       
        public string? Password { get; set; }
        
        public DateTime RegisterTime { get; set; }
       
        public DateTime LastVisit { get; set; }


        public User() { }

        public User(string? login, string? firstName, string? lastName, string? email, Sex sex, Role role, string? password, DateTime registerTime, DateTime lastVisit)
        {
            Login = login;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Sex = sex;
            Role = role;
            Password = password;
            RegisterTime = registerTime;
            LastVisit = lastVisit;
        }
    }
}
