namespace EmployeeManagement2;
using EmployeeManagement2.Data;
using System.Collections.ObjectModel;

public partial class MainPage : ContentPage
{
    // Objeto de la clase que maneja las consultas a la base de datos
    DatabaseQuery dataBaseQuery = new DatabaseQuery();

    // Listas para almacenar departamentos, ubicaciones y empleados
    List<Department> departmentList = new List<Department>();
    List<Department> locationList = new List<Department>();
    List<Employee> employeeList = new List<Employee>();

    // Variable para almacenar el empleado seleccionado
    Employee SelectedEmployee;

    public MainPage()
    {
        InitializeComponent(); // Inicializa los componentes de la interfaz
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDepartment(); // Carga los departamentos al aparecer la página
        await LoadEmployees(); // Carga los empleados al aparecer la página
    }

    // Método para cargar los departamentos desde la base de datos
    public async Task LoadDepartment()
    {
        departmentList = await dataBaseQuery.SelectDepartmentName(); // Obtiene la lista de departamentos
        DepartmentsName.ItemsSource = departmentList; // Asigna la lista
        pickerDepartment.ItemsSource = departmentList; // Asigna la lista
    }

    // Método para cargar empleados según la ubicación seleccionada
    private async Task LoadEmployees()
    {
        var selectedLocation = pickerDepartmentLocation.SelectedItem as Department; // Obtiene la ubicación seleccionada
        if (selectedLocation != null)
        {
            employeeList = await dataBaseQuery.SelectEmployeesByLocation(selectedLocation.Location); // Obtiene empleados por ubicación
            Employee.ItemsSource = employeeList; // Actualiza la lista de empleados
        }
    }

    // Evento al seleccionar un departamento
    private async void OnDepartmentSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            var selectedDepartment = (Department)e.Item; // Obtiene el departamento seleccionado
            string departmentName = selectedDepartment.Name;

            locationList = await dataBaseQuery.SelectLocationByDepartment(departmentName); // Obtiene ubicaciones por departamento
            DepartmentsLocate.ItemsSource = locationList; // Actualiza la lista de ubicaciones
        }
    }

    // Evento al seleccionar una ubicación
    private async void OnLocationSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            var selectedLocation = (Department)e.Item; // Obtiene la ubicación seleccionada
            string departmentLocation = selectedLocation.Location;

            employeeList = await dataBaseQuery.SelectEmployeesByLocation(departmentLocation); // Obtiene empleados por ubicación
            Employee.ItemsSource = employeeList; // Actualiza la lista de empleados
        }
    }

    // Evento al seleccionar un departamento en el picker
    private async void OnPickerNameDepartmentSelected(object sender, EventArgs e)
    {
        var selectedDepartment = pickerDepartment.SelectedItem as Department;

        if (selectedDepartment != null)
        {
            string departmentName = selectedDepartment.Name;
            locationList = await dataBaseQuery.SelectLocationByDepartment(departmentName); // Obtiene ubicaciones por departamento
            pickerDepartmentLocation.ItemsSource = locationList; // Asigna las ubicaciones al picker
            pickerDepartmentLocation.IsEnabled = true; // Habilita el picker de ubicaciones
        }
        else
        {
            pickerDepartmentLocation.IsEnabled = false; // Deshabilita el picker si no hay departamento seleccionado
        }
    }

    // Evento para guardar un nuevo empleado
    private async void OnSaveEmployee(object sender, EventArgs e)
    {
        // Obtiene los valores de entrada
        string lastName = entryLastName.Text;
        string job = entryJob.Text;
        double salary;
        double commission = string.IsNullOrEmpty(entryComission.Text) ? 0 : Convert.ToDouble(entryComission.Text);
        DateTime startDate = pickerStartDate.Date;

        // Validaciones
        if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(job) ||
            !double.TryParse(entrySalary.Text, out salary) ||
            pickerDepartment.SelectedItem == null ||
            pickerDepartmentLocation.SelectedItem == null)
        {
            await DisplayAlert("Error", "Please fill in all required fields correctly.", "OK");
            return;
        }

        var selectedDepartment = pickerDepartment.SelectedItem as Department;
        var selectedLocation = pickerDepartmentLocation.SelectedItem as Department;

        string departmentName = selectedDepartment.Name;
        string departmentLocation = selectedLocation.Location;

        int departmentId = await dataBaseQuery.SelectDepartmentIdByNameAndLocation(departmentName, departmentLocation); // Obtiene el ID del departamento

        if (departmentId == 0)
        {
            await DisplayAlert("Error", "The department could not be found.", "OK");
            return;
        }

        try
        {
            await dataBaseQuery.InsertEmployee(lastName, job, salary, commission, startDate, departmentId); // Inserta el nuevo empleado
            await DisplayAlert("Success", "Employee added successfully.", "OK");
            ClearInputFields(); // Limpia los campos de entrada
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "An error occurred while saving the employee: " + ex.Message, "OK");
        }
    }

    // Evento para actualizar un empleado existente
    private async void OnUpdateEmployee(object sender, EventArgs e)
    {
        // Obtener los valores de entrada
        string lastName = entryLastName.Text;
        string job = entryJob.Text;
        double salary;
        double commission = string.IsNullOrEmpty(entryComission.Text) ? 0 : Convert.ToDouble(entryComission.Text);
        DateTime startDate = pickerStartDate.Date;

        // Validaciones
        if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(job) ||
            !double.TryParse(entrySalary.Text, out salary) ||
            pickerDepartment.SelectedItem == null ||
            pickerDepartmentLocation.SelectedItem == null ||
            SelectedEmployee == null)
        {
            await DisplayAlert("Error", "Please fill in all required fields correctly.", "OK");
            return;
        }

        var selectedDepartment = pickerDepartment.SelectedItem as Department;
        var selectedLocation = pickerDepartmentLocation.SelectedItem as Department;

        string departmentName = selectedDepartment.Name;
        string departmentLocation = selectedLocation.Location;

        int departmentId = await dataBaseQuery.SelectDepartmentIdByNameAndLocation(departmentName, departmentLocation); // Obtiene el ID del departamento

        if (departmentId == 0)
        {
            await DisplayAlert("Error", "El Departamento no se ha encontrado", "OK");
            return;
        }

        int employeeId = SelectedEmployee.EmployeeId; // Obtiene el ID del empleado seleccionado

        try
        {
            await dataBaseQuery.UpdateEmployee(lastName, job, salary, commission, startDate, departmentId, employeeId); // Actualiza el empleado
            await DisplayAlert("Exito", "Empleado Actualizado Correctamente", "OK");
            ClearInputFields(); // Limpia los campos de entrada
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "A Ocurrido un Error al Actualizar un Empleado", "OK");
        }
    }

    // Evento para limpiar los campos de entrada
    private void OnClearDataEntry(object sender, EventArgs e)
    {
        ClearInputFields(); // Llama al método para limpiar los campos
    }

    // Evento que se dispara al seleccionar un empleado
    private async void OnEmployeeSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            SelectedEmployee = (Employee)e.Item; // Obtiene el empleado seleccionado

            // Rellena los campos de entrada con los datos del empleado seleccionado
            entryLastName.Text = SelectedEmployee.LastName;
            entryJob.Text = SelectedEmployee.Job;
            entrySalary.Text = SelectedEmployee.Salary.ToString();
            entryComission.Text = SelectedEmployee.Commission.ToString();
            pickerStartDate.Date = SelectedEmployee.StartDate;
        }
    }

    // Evento para eliminar un empleado
    private async void OnDeleteEmployee(object sender, EventArgs e)
    {
        if (SelectedEmployee == null)
        {
            await DisplayAlert("Error", "No hay Empleado Seleccionado", "OK");
            return;
        }

        // Confirmar la eliminación
        bool confirm = await DisplayAlert("Confirmación", "Seguro que quieres eliminar este Empleado?", "Yes", "No");
        if (confirm)
        {
            try
            {
                await dataBaseQuery.DeleteEmployee(SelectedEmployee.EmployeeId); // Elimina el empleado
                await DisplayAlert("Exito", "Empleado Eliminado Correctamente", "OK");

                // Limpia los campos de entrad adespués de eliminar un empleado
                ClearInputFields();

                // Actualiza la lista de empleados
                var location = pickerDepartmentLocation.SelectedItem as Department;
                if (location != null)
                {
                    employeeList = await dataBaseQuery.SelectEmployeesByLocation(location.Location); // Carga la lista de empleados
                    Employee.ItemsSource = employeeList; // Actualiza la lista de empleados
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while deleting the employee: " + ex.Message, "OK");
            }
        }
    }

    // Evento al cambiar la opcion de búsqueda
    private void OnSearchByChanged(object sender, EventArgs e)
    {
        string selectedOption = pickerSearchBy.SelectedItem as string;

        // Muestra y oculta el searchBar y el datePicker
        if (selectedOption == "Start Date")
        {
            searchBar.IsVisible = false; 
            datePickerSearch.IsVisible = true;
            searchButton.IsVisible = true;
        }
        else
        {
            searchBar.IsVisible = true;
            datePickerSearch.IsVisible = false;
            searchButton.IsVisible = false;
        }
    }

    // Evento al hacer clic en el botón de búsqueda solo para la fecha de inicio
    private async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        string selectedOption = pickerSearchBy.SelectedItem as string;

        if (selectedOption == "Start Date")
        {
            DateTime selectedDate = datePickerSearch.Date;

            // Filtra por fecha de inicio
            List<Employee> filteredEmployees = employeeList
                .Where(emp => emp.StartDate.Date == selectedDate.Date)
                .ToList();

            Employee.ItemsSource = filteredEmployees; // Actualiza la lista de empleados mostrada
        }
    }

    // Evento al cambiar el texto en el SearchBar
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string selectedOptionPicker = pickerSearchBy.SelectedItem as string;

        if (selectedOptionPicker != "Start Date") //Filtra si no es por fecha
        {
            string searchText = e.NewTextValue;

            if (string.IsNullOrEmpty(searchText))
            {
                // Mostrar todos los empleados si no hay texto
                var location = pickerDepartmentLocation.SelectedItem as Department;
                if (location != null)
                {
                    employeeList = await dataBaseQuery.SelectEmployeesByLocation(location.Location); // Vuelve a cargar la lista de empleados
                    Employee.ItemsSource = employeeList; // Actualiza la lista de empleados
                }
                return;
            }

            List<Employee> filteredEmployees = new List<Employee>();

            switch (selectedOptionPicker)
            {
                case "Last Name":
                    filteredEmployees = employeeList.Where(emp => emp.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "Job":
                    filteredEmployees = employeeList.Where(emp => emp.Job.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    break;
                case "Salary":
                    if (double.TryParse(searchText, out double salary))
                    {
                        filteredEmployees = employeeList.Where(emp => emp.Salary == salary).ToList();
                    }
                    break;
                case "Comission":
                    if (double.TryParse(searchText, out double commission))
                    {
                        filteredEmployees = employeeList.Where(emp => emp.Commission == commission).ToList();
                    }
                    break;
            }

            Employee.ItemsSource = filteredEmployees; // Actualiza la lista de empleados mostrada
        }
    }

    // Método para limpiar los campos de entrada
    private void ClearInputFields()
    {
        entryLastName.Text = string.Empty;
        entryJob.Text = string.Empty;
        entrySalary.Text = string.Empty;
        entryComission.Text = string.Empty;
        pickerStartDate.Date = DateTime.Now;
        pickerDepartment.SelectedIndex = -1;
        pickerDepartmentLocation.SelectedIndex = -1;
    }
}
