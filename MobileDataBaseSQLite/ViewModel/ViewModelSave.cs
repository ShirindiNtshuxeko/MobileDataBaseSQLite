using MobileDataBaseSQLite.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static MobileDataBaseSQLite.Model.ModelCustomers;

namespace MobileDataBaseSQLite.ViewModel
{
    public class ViewModelSave : INotifyPropertyChanged
    {
        private readonly ViewModelDBService _dbService = new ViewModelDBService();

        // Use ObservableCollection to notify the UI of changes
        public ObservableCollection<Customer> Customers { get; set; } = new ObservableCollection<Customer>();

        //What we want to save:
        private string _name;
        private string _email;
        private string _password;

        // Flag and reference for editing
        public bool IsEditMode { get; set; }
        public Customer EditingCustomer { get; set; }
        //Lets define them
        public string Name 
        {
            get => _name; 
            set {  _name = value; OnPropertyChanged(); }
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        public ICommand SaveCommand { get;}

        private Page _page;



        //Class construtor
        public ViewModelSave(Page page) 
        {
            _page = page;
            SaveCommand = new Command(async () => await SaveMethod());

            // Load initial data
            Loadcustomer();

        }

        

        private async Task SaveMethod() 
        {

            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                MessagingCenter.Send(this, "DisplayAlert", "Please fill in all fields.");
                return;
            }

            if (IsEditMode && EditingCustomer != null)
            {
                // Update properties of the existing customer
                EditingCustomer.Name = Name;
                EditingCustomer.Email = Email;
                EditingCustomer.Password = Password;

                // Save the updated customer in the database
                await _dbService.update(EditingCustomer);

                // Refresh the ObservableCollection by removing and re-adding the customer
                int index = Customers.IndexOf(EditingCustomer);
                if (index >= 0)
                {
                    Customers.RemoveAt(index);
                    Customers.Insert(index, EditingCustomer); // Re-add to trigger UI refresh
                }

                MessagingCenter.Send(this, "DisplayAlert", "Customer updated successfully.");
                IsEditMode = false;
                EditingCustomer = null;
            }
            else
            {
                var existingCustomers = await _dbService.GetCustomers();
                bool emailExists = existingCustomers.Any(c => c.Email == Email);
                if (emailExists)
                {
                    MessagingCenter.Send(this, "DisplayAlert", "This email is already registered.");
                    return;
                }

                var newCustomer = new Customer { Name = Name, Email = Email, Password = Password };
                await _dbService.Create(newCustomer);
                Customers.Add(newCustomer);
                MessagingCenter.Send(this, "DisplayAlert", "Customer added successfully.");
            }

            // Clear fields and reset
            Name = Email = Password = string.Empty;
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(Email));
            OnPropertyChanged(nameof(Password));

        }

        private async void Loadcustomer()
        {
            var customerList = await _dbService.GetCustomers();
            Customers.Clear();

            foreach (var customer in customerList)
            {
                Customers.Add(customer);
            }
        }

        public async Task OnCustomerTapped(Customer customer)
        {
            var action = await _page.DisplayActionSheet("Action", "Cancel", null, "Edit", "Delete");

            switch (action)
            {
                case "Edit":
                    Name = customer.Name;
                    Email = customer.Email;
                    Password = customer.Password;
                    IsEditMode = true;
                    EditingCustomer = customer;
                    break;

                case "Delete":
                    await _dbService.Delete(customer);
                    Customers.Remove(customer); // Remove from collection to refresh view
                    break;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
