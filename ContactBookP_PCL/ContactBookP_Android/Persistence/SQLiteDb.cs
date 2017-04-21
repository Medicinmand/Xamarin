using System;
using System.IO;
using SQLite;
using Xamarin.Forms;

using ContactBookP.Persistence;

using ContactBookP.Droid;

[assembly: Dependency(typeof(SQLiteDb))]

namespace ContactBookP.Droid {

    public class SQLiteDb : ISQLiteDb {

        public SQLiteAsyncConnection GetConnection() {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var path = Path.Combine(documentsPath, "MyContactB.db3");

            return new SQLiteAsyncConnection(path);
        }

    };

}