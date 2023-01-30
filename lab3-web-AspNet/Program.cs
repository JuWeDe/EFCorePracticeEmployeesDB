using System;
using System.Linq;
using System.Text;
using EmployeesDB.Data.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace EmployeesDB
{
    class Program : DbContext
    {
        private static readonly EmployeesContext Context = new EmployeesContext();


        static void Main()
        {
            Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            //Console.WriteLine(GetEmployeesWithHighSalaryLinq()); // task 1 Linq

            // String newAddressWithLinq = "Grow street home"; // task 2  Linq
            // String newTownWithLinq = "San-Andreas";
            // RelocateBrowns(newAddressWithLinq, newTownWithLinq);
            //Console.WriteLine(GetAllBrownsLinq());
            //
            // Console.WriteLine(GetEmployeesForAuditLinq()); // task 3 Linq
            //
            // int employeeIdToSearchWithLinq = Convert.ToInt32(Console.ReadLine());
            // Console.WriteLine(GetEmployeeProfileLinq(employeeIdToSearchWithLinq)); // task 4 Linq
            //
            // Console.WriteLine(GetSmallDepartmentsLinq()); // task 5 Linq
            //
            //
            Console.WriteLine(GetEmployeesInformation());
            // Console.WriteLine(GetEmployeesWithHighSalary()); //task 1
            //
            // String newAddress = "Trinity Street 5"; //task 2
            // String newTown = "Matrix";
            // RelocateBrowns(newAddress, newTown);
            // Console.WriteLine(GetAllBrowns());
            //
            // Console.WriteLine(GetEmployeesForAudit()); //task 3
            //
            // int employeeIdToSearch = Convert.ToInt32(Console.ReadLine());
            // Console.WriteLine(GetEmployeeProfile(employeeIdToSearch)); // task 4
            //
            // Console.WriteLine(GetSmallDepartments()); // task 5
            //
            // IncreaseSalary("Marketing", 30); // task 6
            //
            // DeleteDepartment(18); // task 7 
            //
            // DeleteTown("New Asgard"); // task 8
        }

        static string GetEmployeeProfileLinq(int id)
        {
            var sb = new StringBuilder();

            var employee = from e in Context.Employees where e.EmployeeId == id select e;

            foreach (var emp in employee)
            {
                sb.Append($"{emp.FirstName} {emp.MiddleName} {emp.LastName} - {emp.JobTitle} \n");
            }

            var employerProjects = from project in Context.EmployeesProjects
                where project.EmployeeId == id
                select project.Project;

            foreach (var projects in employerProjects)
            {
                sb.AppendLine(projects.Name);
            }

            return sb.ToString();
        }

        static String GetEmployeesForAuditLinq()
        {
            var sb = new StringBuilder();

            var pickEmployees = (from ep in Context.EmployeesProjects
                where ep.Project.StartDate.Year >= 2002 && ep.Project.StartDate.Year <= 2005
                select ep.Employee).Distinct().Take(5).ToList();

            foreach (var st in pickEmployees)
            {
                sb.AppendLine(st.FirstName + " " + st.LastName + " " + st.ManagerId);
                var employeeProjects = (from emp in Context.EmployeesProjects
                    where emp.Project.StartDate.Year >= 2002 && emp.Project.StartDate.Year <= 2005 &&
                          emp.Employee.EmployeeId == st.EmployeeId
                    select emp.Project).ToList();
                foreach (var projects in employeeProjects)
                {
                    if (projects.EndDate == null)
                    {
                        sb.Append(" " + projects.Name + "\n  Начат " + projects.StartDate +
                                  "\n  НЕ ЗАВЕРЕШЕН \n");
                    }
                    else
                    {
                        sb.Append(" " + projects.Name + "\n  Начат " + projects.StartDate +
                                  "\n  Завершен " +
                                  projects.EndDate.Value + "\n");
                    }
                }
            }

            return sb.ToString();
        }

        static String GetAllBrownsLinq()
        {
            var employees = from e in Context.Employees where e.LastName == "Brown" select e;
            var sb = new StringBuilder();
            foreach (var emp in employees)
            {
                //sb.AppendLine(emp.ToString());
                sb.AppendLine(
                    $"{emp.FirstName} {emp.LastName} {emp.MiddleName} {emp.JobTitle} {emp.DepartmentId} {emp.ManagerId} {emp.HireDate} {emp.Salary} {emp.AddressId}");
                sb.AppendLine("---Separator---");
            }

            return sb.ToString().TrimEnd();
        }

        static String GetEmployeesWithHighSalaryLinq()
        {
            var wellPaidEmployees = from e in Context.Employees where e.Salary > 48000 orderby e.LastName select e;
            var sb = new StringBuilder();
            foreach (var emp in wellPaidEmployees)
            {
                //sb.AppendLine(emp.ToString());
                sb.AppendLine(
                    $"{emp.FirstName} {emp.LastName} {emp.MiddleName} {emp.JobTitle} {emp.DepartmentId} {emp.ManagerId} {emp.HireDate} {emp.Salary} {emp.AddressId}");
                sb.AppendLine("---Separator---");
            }

            return sb.ToString().TrimEnd();
        }

        static String GetSmallDepartments()
        {
            var sb = new StringBuilder();
            var departments = Context.Departments.Select(d => new { d.DepartmentId, d.Name }).ToList();
            foreach (var t in departments)
            {
                if (Context.Employees.Count(e => e.DepartmentId == t.DepartmentId) < 5)
                {
                    sb.AppendLine(t.Name);
                }
            }


            return sb.ToString();
        }

        static String GetSmallDepartmentsLinq()
        {
            var sb = new StringBuilder();
            var departments = (from d in Context.Departments select d).ToList();
            foreach (var t in departments)
            {
                if ((from d in Context.Employees where d.DepartmentId == t.DepartmentId select d).Count() < 5)
                {
                    sb.AppendLine(t.Name);
                }
            }


            return sb.ToString();
        }

        static String GetEmployeesForAudit()
        {
            var sb = new StringBuilder();

            var temp = Context.EmployeesProjects
                .Where(p => p.Project.StartDate.Year >= 2002 && p.Project.StartDate.Year <= 2005)
                .Select(e => new
                {
                    e.Employee.EmployeeId,
                    e.Employee.FirstName,
                    e.Employee.LastName,
                    e.Employee.ManagerId,
                })
                .Distinct().Take(5).ToList();

            foreach (var st in temp)
            {
                sb.AppendLine(st.FirstName + " " + st.LastName + " " + st.ManagerId);
                var employeeProjects =
                    Context.EmployeesProjects
                        .Where(p => (p.Project.StartDate.Year >= 2002 && p.Project.StartDate.Year <= 2005) &&
                                    (p.Employee.EmployeeId == st.EmployeeId)).Select(p =>
                            new
                            {
                                p.Project.Name,
                                p.Project.StartDate,
                                p.Project.EndDate
                            }).ToList();
                foreach (var projects in employeeProjects)
                {
                    if (projects.EndDate == null)
                    {
                        sb.Append(" " + projects.Name + "\n  Начат " + projects.StartDate + "\n  НЕ ЗАВЕРЕШЕН \n");
                    }
                    else
                    {
                        sb.Append(" " + projects.Name + "\n  Начат " + projects.StartDate + "\n  Завершен " +
                                  projects.EndDate.Value + "\n");
                    }
                }
            }


            return sb.ToString();
        }

        static String GetEmployeeProfile(int employeeId)
        {
            // task 4
            var sb = new StringBuilder();

            var employer = Context.Employees.Where(p => p.EmployeeId == employeeId).Select(p => new
            {
                p.FirstName,
                p.MiddleName,
                p.LastName,
                p.JobTitle
            });


            foreach (var emp in employer)
            {
                sb.Append($"{emp.FirstName} {emp.MiddleName} {emp.LastName} - {emp.JobTitle} \n");
            }

            var employerProjectsId = Context.EmployeesProjects.Where(p => p.EmployeeId == employeeId).Select(p => new
            {
                projectId = p.ProjectId
            }).ToList();
            int? prjId;
            foreach (var tId in employerProjectsId)
            {
                prjId = tId.projectId;

                var employerProjects = Context.Projects.Where(p => p.ProjectId == prjId).Select(p => new
                {
                    p.Name
                }).ToList();

                foreach (var projects in employerProjects)
                {
                    sb.AppendLine(projects.Name);
                }
            }

            return sb.ToString();
        }

        static void DeleteDepartment(int departmentId)
        {
            // task 7
            var departmentDelete = Context.Departments
                .Include(e => e.Employees)
                .ThenInclude(e => e.EmployeesProjects)
                .First(dep => dep.DepartmentId == departmentId);
            Context.Remove(departmentDelete);
            Context.SaveChanges();
        }

        static void DeleteTown(string townName)
        {
            //task 8
            int townIdToSearch = 0;
            var townId = Context.Towns.Where(t => t.Name == townName).Select(t => new
            {
                townIdToSearch = t.TownId
            }).ToArray();
            foreach (var id in townId)
            {
                townIdToSearch = id.townIdToSearch;
            }

            // нашли id города теперь обновим значения в Adresses на null где TownId совпадает
            Context.Addresses.Where(t => t.TownId == townIdToSearch).Update(t => new Addresses
            {
                TownId = null
            });
            // заменили на null
            Context.Towns.Where(t => t.Name == townName).Delete();
            Context.SaveChanges();
        }

        static void IncreaseSalary(String departmentName, float x)
        {
            //ищем отдел
            int departmentIdToSerach = 0;

            var departmentId = Context.Departments.Where(t => t.Name == departmentName).Select(t => new
            {
                departmentIdToSerach = t.DepartmentId
            }).ToArray();
            foreach (var id in departmentId)
            {
                departmentIdToSerach = id.departmentIdToSerach;
            }

            Context.Employees.Where(e => e.DepartmentId == departmentIdToSerach).Update(e =>
                new Employees
                {
                    Salary = e.Salary * (decimal)(1 + (x * 0.01))
                });
        }

        static void RelocateBrowns(String newLocationAddress, String newLocationTown)
        {
            CreateNewAddress(newLocationAddress, newLocationTown);
            //ищем адрес
            var adressIdContext = Context.Addresses.Where(t => t.AddressText == newLocationAddress).Select(t => new
            {
                test = t.AddressId
            }).ToArray();
            string ans = null;
            foreach (var tId in adressIdContext)
                ans = (tId.test).ToString();
            // нашли Id адреса для переселения
            // собираем объекты сущностей Employyes с фамилией Brown с одним полем AdressId в один лист
            Context.Employees.Where(e => e.LastName == "Brown").Update(u => new Employees()
            {
                AddressId = Int32.Parse(ans)
            });
            Context.SaveChanges();
        }

        static void CreateTown(String townText)
        {
            var newTownsContext = Context.Towns;
            if (townText != null)
            {
                var newTownObject = new Towns()
                {
                    Name = townText
                };
                newTownsContext.Add(newTownObject);
            }


            Context.SaveChanges();
        }

        static void CreateNewAddress(String adressText, String townName)
        {
            var newAddressContext = Context.Addresses;
            var townIdContext = Context.Towns.Where(t => t.Name == townName).Select(t => new
            {
                townIdToInsert = t.TownId
            }).ToArray();
            String ans = null;
            foreach (var tId in townIdContext)
                ans = tId.townIdToInsert.ToString();

            if (ans != null)
            {
                var newAdressObject = new Addresses()
                {
                    AddressText = adressText,
                    TownId = Int32.Parse(ans)
                };
                newAddressContext.Add(newAdressObject);
                Context.SaveChanges();
            }
            else
            {
                CreateTown(townName);
                var newTownIdContext = Context.Towns.Where(t => t.Name == townName).Select(t => new
                {
                    newTownIdToInsert = t.TownId
                }).ToArray();

                String newTownId = null;
                foreach (var tId in newTownIdContext)
                    newTownId = (tId.newTownIdToInsert).ToString();
                if (newTownId != null)
                {
                    var newAdressWithNewTown = new Addresses()
                    {
                        AddressText = adressText,
                        TownId = Int32.Parse(newTownId)
                    };
                    newAddressContext.Add(newAdressWithNewTown);
                }

                Context.SaveChanges();
            }
        }

        static String GetAllBrowns()
        {
            var employees = Context.Employees.Where(e => e.LastName == "Brown").Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.DepartmentId,
                    e.ManagerId,
                    e.HireDate,
                    e.Salary,
                    e.AddressId
                })
                .ToList();

            var sb = new StringBuilder();
            foreach (var emp in employees)
            {
                sb.AppendLine(emp.ToString());
                //sb.AppendLine($"{emp.FirstName} {emp.LastName} {emp.MiddleName} {emp.JobTitle} {emp.DepartmentId} {emp.ManagerId} {emp.HireDate} {emp.Salary} {emp.AddressId}");
                sb.AppendLine("---Separator---");
            }

            return sb.ToString().TrimEnd();
        }

        static String GetEmployeesWithHighSalary()
        {
            var wellPaidEmployees = Context.Employees
                .OrderBy(e => e.LastName)
                .Where(s => s.Salary > 48000)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.DepartmentId,
                    e.ManagerId,
                    e.HireDate,
                    e.Salary,
                    e.AddressId
                })
                .ToList();
            var sb = new StringBuilder();
            foreach (var emp in wellPaidEmployees)
            {
                //sb.AppendLine(emp.ToString());
                sb.AppendLine(
                    $"{emp.FirstName} {emp.LastName} {emp.MiddleName} {emp.JobTitle} {emp.DepartmentId} {emp.ManagerId} {emp.HireDate} {emp.Salary} {emp.AddressId}");
                sb.AppendLine("---Separator---");
            }

            return sb.ToString().TrimEnd();
        }

        static string GetEmployeesInformation()
        {
            var employees = Context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle
                }).ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle}");
            }
            return sb.ToString().TrimEnd();
        }
    }
}