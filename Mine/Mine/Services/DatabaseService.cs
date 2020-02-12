using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mine.Models;
using System.Linq;

namespace Mine.Services
{
    public class DatabaseService : IDataStore<ItemModel>
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(ItemModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }


        /// <summary>
        /// Add the data to the database
        /// </summary>
        /// <param name="data"></param>
        /// <returns>1 for pass, else fail</returns>
        public async Task<bool> CreateAsync(ItemModel data)
        {
            int result = await Database.InsertAsync(data);
            return result > 0 ? true : false;
        }

        /// <summary>
        /// Update the data with the information passed in
        /// </summary>
        /// <param name="data"></param>
        /// <returns>1 for pass, else fail</returns>
        public async Task<bool> UpdateAsync(ItemModel data)
        {
            int result = await Database.UpdateAsync(data);
            return result > 0 ? true : false;
        }

        /// <summary>
        /// Deletes the Data passed in by
        /// Removing it from the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True for pass, else fail</returns>
        /// 
        public async Task<bool> DeleteAsync(string id)
        {
            var data = Database.Table<ItemModel>().Where(i => i.Id.Equals(id)).FirstOrDefaultAsync();

            int result = await Database.DeleteAsync(data);
            return result > 0 ? true : false;
        }

        /// <summary>
        /// Takes the ID and finds it in the database
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Record if found else null</returns>
        public async Task<ItemModel> ReadAsync(string id)
        {
            return await Database.Table<ItemModel>().Where(i => i.Id.Equals(id)).FirstOrDefaultAsync();
        }


        /// <summary>
        /// Get the full list of data
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public async Task<List<ItemModel>> IndexAsync(bool forceRefresh = false)
        {
            return await Database.Table<ItemModel>().ToListAsync();
        }
    }
}
