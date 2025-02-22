using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement2
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string LastName { get; set; }
        public string Job { get; set; }
        public double Salary { get; set; }
        public double Commission { get; set; } = 0;
        public DateTime StartDate { get; set; }
        public int IdDpto { get; set; }

       


        
    }
}
