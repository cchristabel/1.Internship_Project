using Departments;
using SecureMVCApp.Models;

public class Administrator : BaseClass
{
    public Administrator(string firstName, string lastName, string email, Department department)
        : base(firstName, lastName, email)
    {
        Department = department;
    }

    public Department Department { get; set; }
    
}
