using MobileDataBaseSQLite.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MobileDataBaseSQLite.Model.ModelCustomers;

namespace MobileDataBaseSQLite.ViewModel
{
    public class ViewModelDBService
    {
        //DB name
        private const string DB_NAME = "demo_local_db.db3";
        //Connection
        private readonly SQLiteAsyncConnection _connection;

        //Class constructor
        public ViewModelDBService() 
        {
            //DB path as a parameter
            _connection = new SQLiteAsyncConnection(Path.Combine(FileSystem.AppDataDirectory, DB_NAME));
            _ = _connection.CreateTableAsync<Customer>().ConfigureAwait(false);
        }

        public async Task<List<Customer>> GetCustomers() 
        {
            return await _connection.Table<Customer>().ToListAsync();
        }


        //Return a record based on the ID
        public async Task<Customer> GetById(int id) 
        {
            return await _connection.Table<Customer>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        //Insert a customer record
        public async Task Create(Customer customers) 
        {
            await _connection.InsertAsync(customers);
        }

        //Update costomer details
        public async Task update(Customer customers) 
        {
            await _connection.UpdateAsync(customers);
        }

        //Delete method
        public async Task Delete(Customer customers) 
        {
            await _connection.DeleteAsync(customers);
        }

        //The implementation of the Local DB service class in now complete

    }
}
