using System;
using Rsk.Enforcer.PIP;
using Rsk.Enforcer.PolicyModels;

namespace SecureMVCApp.Models
{
    public class BaseClass
    {
        private static int nextId = 1001;
        public BaseClass(string firstName, string lastName, string email)
        {
            Id = nextId++;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

/*
public virtual string GetInfo()
    {
        return $"{FirstName} {LastName} ({Email})";
    }
*/


// Seperating them to let the file focus on a single responsibility, 
// makes your code more organized, easier to maintain, and improves readability.
// And if a class grows in complexity(which it would not for this specific project), you can manage it independently.