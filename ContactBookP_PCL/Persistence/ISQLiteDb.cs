using SQLite;

namespace ContactBookP.Persistence {

    public interface ISQLiteDb {
        SQLiteAsyncConnection GetConnection();
    }

}
