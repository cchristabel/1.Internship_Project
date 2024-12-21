using Departments;
using SecureMVCApp.Models;

namespace ManagerClass
{

    public class Manager : BaseClass
    {
            public Manager(string firstName, string lastName, string email, Department department)
            : base(firstName, lastName, email)
        {
            Department = department;
        }

        
        public Department Department { get; set; }

    }
}

