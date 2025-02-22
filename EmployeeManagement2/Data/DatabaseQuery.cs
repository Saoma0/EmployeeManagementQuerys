using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace EmployeeManagement2.Data
{
    public class DatabaseQuery
    {
        // Cadena de conexión a la base de datos SQLite
        private readonly string _connectionString;

        public DatabaseQuery()
        {
            // Ruta de la base de datos SQLite
            var dataBasePath = @"C:\\Users\\samue\\source\\repos\\EmployeeManagement2\\EmployeeManagement2\\Data\\EmployeeManagementSQLite.db";
            _connectionString = $"Data Source={dataBasePath}";
            // CreateTables(); // Descomentar si es necesario crear tablas
            // insertsData(); // Descomentar si es necesario insertar datos
        }

        // Método para insertar datos iniciales en las tablas
        public async Task insertsData()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); // Abre la conexión a la base de datos

                // Consulta para insertar departamentos
                var insertQuery = @"
                    INSERT INTO Department (Name, Location) VALUES ('Human Resources', 'New York');
                    INSERT INTO Department (Name, Location) VALUES ('Human Resources', 'Los Angeles');
                    INSERT INTO Department (Name, Location) VALUES ('Software Development', 'San Francisco');
                    INSERT INTO Department (Name, Location) VALUES ('Software Development', 'Seattle');
                    INSERT INTO Department (Name, Location) VALUES ('Finance', 'Chicago');
                    INSERT INTO Department (Name, Location) VALUES ('Finance', 'Houston');
                    INSERT INTO Department (Name, Location) VALUES ('Sales', 'Miami');
                    " ;
                using (var command = new SqliteCommand(insertQuery, connection))
                {
                    await command.ExecuteNonQueryAsync(); // Ejecuta la consulta de inserción
                }
                Debug.WriteLine("Departamentos Insertados"); // Mensaje de depuración

                // Consulta para insertar empleados
                var insertEmployeeQuery = @"
                    INSERT INTO Employee (LastName, Job, Salary, Commission, StartDate, IdDept) 
                    VALUES ('Doe', 'HR Manager', 60000, 500, '2020-05-15', 1);
    
                    INSERT INTO Employee (LastName, Job, Salary, Commission, StartDate, IdDept) 
                    VALUES ('Smith', 'HR Specialist', 50000, 400, '2021-07-20', 2);

                    INSERT INTO Employee (LastName, Job, Salary, StartDate, IdDept) 
                    VALUES ('Johnson', 'Software Engineer', 80000, '2019-09-10', 3);

                    INSERT INTO Employee (LastName, Job, Salary, StartDate, IdDept) 
                    VALUES ('Davis', 'Software Engineer', 85000, '2022-02-01', 4);

                    INSERT INTO Employee (LastName, Job, Salary, Commission, StartDate, IdDept) 
                    VALUES ('Brown', 'Financial Analyst', 70000, 1400, '2018-11-25', 5);

                    INSERT INTO Employee (LastName, Job, Salary, StartDate, IdDept) 
                    VALUES ('Wilson', 'Finance Manager', 75000, '2020-08-05', 6);

                    INSERT INTO Employee (LastName, Job, Salary, Commission, StartDate, IdDept) 
                    VALUES ('Martinez', 'Sales Representative', 55000, 5000, '2021-03-15', 7);
                    ";

                using (var command = new SqliteCommand(insertEmployeeQuery, connection))
                {
                    await command.ExecuteNonQueryAsync(); // Ejecuta la consulta de inserción de empleados
                }
                Debug.WriteLine("Empleados Insertados"); // Mensaje de depuración
            }
        }

        // Método para crear las tablas en la base de datos
        public async void CreateTables()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); 

                // Consulta para crear la tabla de departamentos
                string queryCreateTableDepartment = "CREATE TABLE IF NOT EXISTS Department(" +
                    "DeptId INTEGER PRIMARY KEY AUTOINCREMENT," + 
                    "Name TEXT NOT NULL," + 
                    "Location TEXT NOT NULL" + 
                    ");";
                using (var command = new SqliteCommand(queryCreateTableDepartment, connection))
                {
                    await command.ExecuteNonQueryAsync(); // Ejecuta la consulta para crear la tabla
                }

                // Consulta para crear la tabla de empleados
                string queryCreateTableEmployee = "CREATE TABLE IF NOT EXISTS Employee(" +
                    "EmployeeId INTEGER PRIMARY KEY AUTOINCREMENT," +
                    "LastName TEXT NOT NULL," + 
                    "Job TEXT NOT NULL," + 
                    "Salary REAL NOT NULL," + 
                    "Commission REAL DEFAULT 0," + 
                    "StartDate DATE NOT NULL," + 
                    "IdDept INTEGER," +
                    "FOREIGN KEY(IdDept) REFERENCES Department(DeptId) ON DELETE CASCADE" + 
                    ");";

                using (var command = new SqliteCommand(queryCreateTableEmployee, connection))
                {
                    await command.ExecuteNonQueryAsync(); // Ejecuta la consulta para crear la tabla
                }
            }
        }

        // Método para seleccionar todos los departamentos
        public async Task<List<Department>> SelectDepartmentName()
        {
            var departments = new List<Department>(); // Lista para almacenar los departamentos

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); 
                string query = "SELECT DISTINCT Name FROM Department;"; // Consulta para obtener nombres de departamentos

                using (var command = new SqliteCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync()) // Ejecuta la consulta y obtiene un lector
                {
                    while (await reader.ReadAsync()) // Lee los resultados
                    {
                        var department = new Department
                        {
                            Name = reader.GetString(0), // Asigna el nombre del departamento
                        };

                        departments.Add(department); // Agrega el departamento a la lista
                    }
                }
            }
            return departments; // Devuelve la lista de departamentos
        }

        // Método para obtener el ID de un departamento dado su nombre y ubicación
        public async Task<int> SelectDepartmentIdByNameAndLocation(string name, string location)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); // Abre la conexión a la base de datos

                string query = "SELECT DeptId FROM Department WHERE Name = @Name AND Location = @Location;"; // Consulta para obtener el ID del departamento

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", name); // Agrega el parámetro del nombre
                    command.Parameters.AddWithValue("@Location", location); // Agrega el parámetro de la ubicación

                    var result = await command.ExecuteScalarAsync(); 
                    return result != null ? Convert.ToInt32(result) : 0; // Devuelve el ID o 0 si no se encontró
                }
            }
        }

        // Método para seleccionar ubicaciones de un departamento dado su nombre
        public async Task<List<Department>> SelectLocationByDepartment(string nameDpto)
        {
            var locations = new List<Department>(); // Lista para almacenar las ubicaciones

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); 
                string query = "SELECT DISTINCT Location FROM Department WHERE Name = @Name;"; // Consulta para obtener ubicaciones

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", nameDpto); // Agrega el parámetro del nombre del departamento

                    using (var reader = await command.ExecuteReaderAsync()) 
                    {
                        while (await reader.ReadAsync()) // Lee los resultados
                        {
                            var location = new Department
                            {
                                Location = reader.GetString(0) // Asigna la ubicación
                            };

                            locations.Add(location); // Agrega la ubicación a la lista
                        }
                    }
                }
            }
            return locations; // Devuelve la lista de ubicaciones
        }

        // Método para seleccionar empleados por ubicación
        public async Task<List<Employee>> SelectEmployeesByLocation(string location)
        {
            var employees = new List<Employee>(); // Lista para almacenar los empleados

            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); 
                string query = "SELECT Employee.EmployeeId, Employee.LastName, Employee.Job, Employee.Salary, Employee.Commission, Employee.StartDate, Employee.IdDept " +
                "FROM Employee " +
                "JOIN Department ON Employee.IdDept = Department.DeptId " + 
                "WHERE Department.Location = @Location;"; 

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Location", location); // Agrega el parámetro de ubicación

                    using (var reader = await command.ExecuteReaderAsync()) // Ejecuta la consulta y obtiene un lector
                    {
                        while (await reader.ReadAsync()) // Lee los resultados
                        {
                            var employee = new Employee
                            {
                                EmployeeId = reader.GetInt32(0), // Asigna el ID del empleado
                                LastName = reader.GetString(1), // Asigna el apellido
                                Job = reader.GetString(2), // Asigna el trabajo
                                Salary = reader.GetDouble(3), // Asigna el salario
                                Commission = reader.GetDouble(4), // Asigna la comisión
                                StartDate = reader.GetDateTime(5), // Asigna la fecha de inicio
                                IdDpto = reader.GetInt32(6) // Asigna el ID del departamento
                            };

                            employees.Add(employee); // Agrega el empleado a la lista
                        }
                    }
                }
            }
            return employees; // Devuelve la lista de empleados
        }

        // Método para insertar un nuevo empleado
        public async Task InsertEmployee(string lastName, string job, double salary, double commission, DateTime startDate, int departmentId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); // Abre la conexión a la base de datos

                string query = "INSERT INTO Employee (LastName, Job, Salary, Commission, StartDate, IdDept) " +
                               "VALUES (@LastName, @Job, @Salary, @Commission, @StartDate, @IdDept)";

                using (var command = new SqliteCommand(query, connection))
                {
                    // Agrega los parámetros necesarios para la inserción
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Job", job);
                    command.Parameters.AddWithValue("@Salary", salary);
                    command.Parameters.AddWithValue("@Commission", commission);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@IdDept", departmentId);

                    await command.ExecuteNonQueryAsync(); // Ejecuta la consulta de inserción
                }
            }
        }

        // Método para actualizar un empleado existente
        public async Task UpdateEmployee(string lastName, string job, double salary, double commission, DateTime startDate, int departmentId, int employeeId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Employee SET LastName = @LastName, Job = @Job, Salary = @Salary, Commission = @Commission, StartDate = @StartDate, IdDept = @IdDept " +
                               "WHERE EmployeeId = @EmployeeId";

                using (var command = new SqliteCommand(query, connection))
                {
                    // Agrega los parámetros necesarios para la actualización
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Job", job);
                    command.Parameters.AddWithValue("@Salary", salary);
                    command.Parameters.AddWithValue("@Commission", commission);
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@IdDept", departmentId);
                    command.Parameters.AddWithValue("@EmployeeId", employeeId);

                    await command.ExecuteNonQueryAsync(); // Ejecuta la consulta de actualización
                }
            }
        }

        // Método para eliminar un empleado
        public async Task DeleteEmployee(int employeeId)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                await connection.OpenAsync(); 

                string query = "DELETE FROM Employee WHERE EmployeeId = @EmployeeId"; // Consulta para eliminar un empleado

                using (var command = new SqliteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@EmployeeId", employeeId); // Agrega el parámetro del ID del empleado
                    await command.ExecuteNonQueryAsync(); // Ejecuta la consulta de eliminación
                }
            }
        }

        
        
    }
} 