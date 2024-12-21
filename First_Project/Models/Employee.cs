using Departments;
using SecureMVCApp.Models;

public class Employee : BaseClass
{
    public Employee(string firstName, string lastName, string email, Department department)
        : base(firstName, lastName, email)
    {
        Department = department;
    }

    public Department Department { get; set; }
    
}

