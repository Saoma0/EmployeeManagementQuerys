namespace EmployeeManagement2;
using EmployeeManagement2.Data;
using System.Collections.ObjectModel;

public partial class MainPage : ContentPage
{
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

    //Carga los departamentos
    public async Task LoadDepartment()
    {
        departmentList = await dataBaseQuery.SelectDepartmentName(); 
        DepartmentsName.ItemsSource = departmentList; 
        pickerDepartment.ItemsSource = departmentList;
    }

    // Carga los empleados
    private async Task LoadEmployees()
    {
        var selectedLocation = pickerDepartmentLocation.SelectedItem as Department; // Obtiene la ubicación seleccionada
        if (selectedLocation != null)
        {
            employeeList = await dataBaseQuery.SelectEmployeesByLocation(selectedLocation.Location);
            Employee.ItemsSource = employeeList; // Actualiza la lista de empleados
        }
    }

    // Al seleccionar departamento muestra las localizaciones
    private async void OnDepartmentSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            var selectedDepartment = (Department)e.Item; // Obtiene el departamento seleccionado
            string departmentName = selectedDepartment.Name;

            locationList = await dataBaseQuery.SelectLocationByDepartment(departmentName); 
            DepartmentsLocate.ItemsSource = locationList; // Actualiza la lista de localizaciones
        }
    }

    //Al seleccionar localizacione muestra empleados en esa misma ubicacion
    private async void OnLocationSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            var selectedLocation = (Department)e.Item; // Obtiene la localizacion seleccionada
            string departmentLocation = selectedLocation.Location;

            employeeList = await dataBaseQuery.SelectEmployeesByLocation(departmentLocation);
            Employee.ItemsSource = employeeList;
        }
    }

    // Al seleccionar el departamento del picker
    private async void OnPickerNameDepartmentSelected(object sender, EventArgs e)
    {
        var selectedDepartment = pickerDepartment.SelectedItem as Department; //Obtiene el departamento seleccionado

        if (selectedDepartment != null)
        {
            string departmentName = selectedDepartment.Name;
            locationList = await dataBaseQuery.SelectLocationByDepartment(departmentName);
            pickerDepartmentLocation.ItemsSource = locationList;
            pickerDepartmentLocation.IsEnabled = true; 
        }
        else
        {
            pickerDepartmentLocation.IsEnabled = false; // Deshabilita el picker si no hay departamento seleccionado
        }
    }

    //Inserta nuevo empleados
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

        // Obtiene el ID del departamento
        string departmentName = selectedDepartment.Name;
        string departmentLocation = selectedLocation.Location;

        int departmentId = await dataBaseQuery.SelectDepartmentIdByNameAndLocation(departmentName, departmentLocation); 

        if (departmentId == 0)
        {
            await DisplayAlert("Error", "El departamento no se ha podido encontrar", "OK");
            return;
        }

        try
        {
            // Inserta el nuevo empleado
            await dataBaseQuery.InsertEmployee(lastName, job, salary, commission, startDate, departmentId); 
            await DisplayAlert("Exito", "Empleado añadido correctamente", "OK");
            ClearInputFields(); // Limpia los campos de entrada
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "A Ocurrido un Error al añadir un empleado", "OK");
        }
    }

   //Actualiza empleado existente
    private async void OnUpdateEmployee(object sender, EventArgs e)
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
            pickerDepartmentLocation.SelectedItem == null ||
            SelectedEmployee == null)
        {
            await DisplayAlert("Error", "Por favor Introduce todos los Datos", "OK");
            return;
        }

        var selectedDepartment = pickerDepartment.SelectedItem as Department;
        var selectedLocation = pickerDepartmentLocation.SelectedItem as Department;

        // Obtiene el ID del departamento
        string departmentName = selectedDepartment.Name;
        string departmentLocation = selectedLocation.Location;

        int departmentId = await dataBaseQuery.SelectDepartmentIdByNameAndLocation(departmentName, departmentLocation); 

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
            ClearInputFields(); 
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "A Ocurrido un Error al Actualizar un Empleado", "OK");
        }
    }

    // Limpia los campos de entrada
    private void OnClearDataEntry(object sender, EventArgs e)
    {
        ClearInputFields(); // Llama al método para limpiar los campos
    }

    //Evento al seleccionar un empleado
    private async void OnEmployeeSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item != null)
        {
            SelectedEmployee = (Employee)e.Item; // Obtiene el empleado seleccionado

            
            entryLastName.Text = SelectedEmployee.LastName;
            entryJob.Text = SelectedEmployee.Job;
            entrySalary.Text = SelectedEmployee.Salary.ToString();
            entryComission.Text = SelectedEmployee.Commission.ToString();
            pickerStartDate.Date = SelectedEmployee.StartDate;
        }
    }

    // Elimina empleado seleccionado
    private async void OnDeleteEmployee(object sender, EventArgs e)
    {
        if (SelectedEmployee == null)
        {
            await DisplayAlert("Error", "No hay Empleado Seleccionado", "OK");
            return;
        }

        // Confirma la eliminación
        bool confirm = await DisplayAlert("Confirmación", "Seguro que quieres eliminar este Empleado?", "Yes", "No");
        if (confirm)
        {
            try
            {
                await dataBaseQuery.DeleteEmployee(SelectedEmployee.EmployeeId); // Elimina el empleado
                await DisplayAlert("Exito", "Empleado Eliminado Correctamente", "OK");

                // Limpia los campos de entrada
                ClearInputFields();

                // Actualiza la lista de empleados
                var location = pickerDepartmentLocation.SelectedItem as Department;
                if (location != null)
                {
                    employeeList = await dataBaseQuery.SelectEmployeesByLocation(location.Location); 
                    Employee.ItemsSource = employeeList; 
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "A Ocurrido un error al Eliminar un Empleado", "OK");
            }
        }
    }

    // Evento al cambiar la opcion de búsqueda
    private void OnSearchByChanged(object sender, EventArgs e)
    {
        string selectedOption = pickerSearchBy.SelectedItem as string;

        // Muestra y oculta el searchBar y el datePicker
        if (selectedCriteria == "Start Date")
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

    // Evento para filtrar por Fecha    
    private async void OnSearchButtonClicked(object sender, EventArgs e)
    {
        string selectedOption = pickerSearchBy.SelectedItem as string;

        if (selectedOption == "Start Date")
        {
            DateTime selectedDate = datePickerSearch.Date;

            if (employeeList != null)
            {
                // Filtra por fecha de inicio
                List<Employee> filteredEmployees = employeeList
                    .Where(emp => emp.StartDate.Date == selectedDate.Date)
                    .ToList();

                Employee.ItemsSource = filteredEmployees; // Actualiza la lista de empleados mostrada
            }
        }
    }
    //Evento para filtrar por
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string selectedOptionPicker = pickerSearchBy.SelectedItem as string;

        if (selectedOptionPicker != "Start Date") // Filtra si no es por fecha
        {
            string searchText = e.NewTextValue;

            if (string.IsNullOrEmpty(searchText))
            {
                // Si no hay texto en el searchbar,muestra los empleados por busqueda
                var location = DepartmentsLocate.SelectedItem as Department;
                if (location != null)
                {
                    employeeList = await dataBaseQuery.SelectEmployeesByLocation(location.Location);
                    Employee.ItemsSource = employeeList; // Actualizamos la lista
                }
                return;
            }

            List<Employee> filteredEmployees = new List<Employee>();

            // Filtra segun la opcion del picker
            switch (selectedOptionPicker)
            {
                case "Last Name":
                    filteredEmployees = employeeList
                        .Where(emp => emp.LastName.Contains(searchText, StringComparison.OrdinalIgnoreCase)) //Ignora mayusuclas minusculas
                        .ToList();
                    break;
                case "Job":
                    filteredEmployees = employeeList
                        .Where(emp => emp.Job.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    break;
                case "Salary":
                    if (double.TryParse(searchText, out double salary))
                    {
                        filteredEmployees = employeeList
                            .Where(emp => emp.Salary.ToString().Contains(searchText))
                            .ToList();
                    }
                    break;
                case "Comission":
                    if (double.TryParse(searchText, out double commission))
                    {
                        filteredEmployees = employeeList
                            .Where(emp => emp.Commission.ToString().Contains(searchText)) //Contains verifica si encuentra la cadena
                            .ToList();
                    }
                    break;
            }

            // Actualizamos el ItemsSource con los resultados filtrados
            Employee.ItemsSource = filteredEmployees;


            if (string.IsNullOrEmpty(searchBar.Text))
            {
                LoadEmployees();
            }
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
        pickerSearchBy.SelectedIndex = -1;
        searchBar.Text = string.Empty;
    }
}
