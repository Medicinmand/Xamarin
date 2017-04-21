using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace VVS.Database
{
    public class DBMeter
    {
        private SQLiteAsyncConnection _connection;

        public DBMeter()
        {
            _connection = DependencyService.Get<ISQLiteDB>().GetConnection();
        }
    }
}
