using NadekoBot.DataModels;
using SQLite;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NadekoBot.Classes
{

    internal class DbHandler
    {

        private class Operation
        {
            public IDataModel model { get; set; }
            public Type type { get; set; }
            public int id { get; set; }
            public IEnumerable<IDataModel> enumerable { get; set; }
            public Method method { get; set; }
        }

        private enum Method
        {
            INSERT,
            INSERT_MANY,
            INSERT_OR_REPLACE,
            UPDATE,
            UPDATEALL,
            DELETE,
            DELETEALL,
        }

        public static DbHandler Instance { get; } = new DbHandler();

        private static ConcurrentQueue<Operation> waitingQueries = new ConcurrentQueue<Operation>();

        private static bool stoped { get; set; } = false;

        private Task updater = new Task(() =>
        {
            while(!stoped)
            {
                Task.Delay(200);
                DbHandler.Instance.UpdateDatabase();
            }
            DbHandler.Instance.UpdateDatabase(); //make sure that we updated the database completely
        });

        private string FilePath { get; } = "data/nadekobot.sqlite";

        public SQLiteConnection Connection { get; set; }

        static DbHandler() { }
        public DbHandler()
        {
            Connection = new SQLiteConnection(FilePath);
            Connection.CreateTable<Stats>();
            Connection.CreateTable<Command>();
            Connection.CreateTable<Announcement>();
            Connection.CreateTable<Request>();
            Connection.CreateTable<TypingArticle>();
            Connection.CreateTable<CurrencyState>();
            Connection.CreateTable<CurrencyTransaction>();
            Connection.CreateTable<Donator>();
            Connection.CreateTable<UserPokeTypes>();
            Connection.CreateTable<UserQuote>();
            Connection.CreateTable<Reminder>();
            Connection.CreateTable<SongInfo>();
            Connection.CreateTable<PlaylistSongInfo>();
            Connection.CreateTable<MusicPlaylist>();
            Connection.CreateTable<Incident>();
            Connection.Execute(Queries.TransactionTriggerQuery);
            updater.Start();
            try
            {
                Connection.Execute(Queries.DeletePlaylistTriggerQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        internal T FindOne<T>(Expression<Func<T, bool>> p) where T : IDataModel, new()
        {
            return Connection.Table<T>().Where(p).FirstOrDefault();
        }

        internal IList<T> FindAll<T>(Expression<Func<T, bool>> p) where T : IDataModel, new()
        {
            return Connection.Table<T>().Where(p).ToList();
        }

        internal void DeleteAll<T>() where T : IDataModel
        {
            Operation op = new Operation()
            {
                method = Method.DELETEALL,
                type = typeof(T)
            };
            waitingQueries.Enqueue(op);
        }

        internal void DeleteWhere<T>(Expression<Func<T, bool>> p) where T : IDataModel, new()
        {
            var id = Connection.Table<T>().Where(p).FirstOrDefault()?.Id;
            if (id.HasValue)
            {
                Operation op = new Operation()
                {
                    method = Method.DELETE,
                    type = typeof(T),
                    id = id.Value,
                };
                waitingQueries.Enqueue(op);
            }
        }

        internal void InsertData<T>(T o) where T : IDataModel
        {
            Operation op = new Operation()
            {
                method = Method.INSERT,
                type = typeof(T),
                model = o
            };
            waitingQueries.Enqueue(op);
        }

        internal void InsertMany<T>(T objects) where T : IEnumerable<IDataModel>
        {
            Operation op = new Operation()
            {
                enumerable = objects,
                method = Method.INSERT_MANY,
            };
            waitingQueries.Enqueue(op);
        }

        internal void UpdateData<T>(T o) where T : IDataModel
        {
            Operation op = new Operation()
            {
                method = Method.UPDATE,
                type = typeof(T),
                model = o
            };
            waitingQueries.Enqueue(op);
        }

        internal void UpdateAll<T>(IEnumerable<T> objs) where T : IDataModel, new()
        {
            Operation op = new Operation()
            {
                method = Method.UPDATEALL,
                type = typeof(T),
                enumerable = objs
            };
            waitingQueries.Enqueue(op);
        }

        internal HashSet<T> GetAllRows<T>() where T : IDataModel, new()
        {
            return new HashSet<T>(Connection.Table<T>());
        }

        internal CurrencyState GetStateByUserId(long id)
        {
            return Connection.Table<CurrencyState>().Where(x => x.UserId == id).FirstOrDefault();
        }

        internal T Delete<T>(int id) where T : IDataModel, new() //Can't add to the waiting queries queue do to the awainting response
        {
            var found = Connection.Find<T>(id);
            if (found != null)
                Connection.Delete<T>(found.Id);
            return found;
        }

        /// <summary>
        /// Updates an existing object or creates a new one
        /// </summary>
        internal void Save<T>(T o) where T : IDataModel, new()
        {
            var found = Connection.Find<T>(o.Id);
            if (found == null)
            {
                Operation op = new Operation()
                {
                    method = Method.INSERT,
                    type = typeof(T),
                    model = o
                };
                waitingQueries.Enqueue(op);
            }
            else
            {
                Operation op = new Operation()
                {
                    method = Method.UPDATE,
                    type = typeof(T),
                    model = o
                };
                waitingQueries.Enqueue(op);
            }
        }

        /// <summary>
        /// Updates an existing object or creates a new one
        /// </summary>
        internal void SaveAll<T>(IEnumerable<T> ocol) where T : IDataModel, new()
        {
            foreach (var o in ocol)
            {
                Operation op = new Operation()
                {
                    method = Method.INSERT_OR_REPLACE,
                    type = typeof(T),
                    model = o
                };
                waitingQueries.Enqueue(op);
            }
        }

        internal T GetRandom<T>(Expression<Func<T, bool>> p) where T : IDataModel, new()
        {
            var r = new Random();
            return Connection.Table<T>().Where(p).ToList().OrderBy(x => r.Next()).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="num">Page number (0+)</param>
        /// <returns></returns>
        internal List<PlaylistData> GetPlaylistData(int num)
        {
            return Connection.Query<PlaylistData>(
@"SELECT mp.Name as 'Name',mp.Id as 'Id', mp.CreatorName as 'Creator', Count(*) as 'SongCnt' FROM MusicPlaylist as mp
INNER JOIN PlaylistSongInfo as psi
ON mp.Id = psi.PlaylistId
Group BY mp.Name
Order By mp.DateAdded desc
Limit 20 OFFSET ?", num * 20);
        }

        internal IEnumerable<CurrencyState> GetTopRichest(int n = 10)
        {
            return Connection.Table<CurrencyState>().OrderByDescending(cs => cs.Value).Take(n).ToList();
        }

        private void UpdateDatabase()
        {
            if (waitingQueries.IsEmpty)
                return;

            Connection.BeginTransaction();
            while(!waitingQueries.IsEmpty)
            {
                Operation op = null;
                bool res = waitingQueries.TryDequeue(out op);
                if (!res)
                    return; //Do not continue, let's try the next datate update
                switch(op.method)
                {
                    case Method.INSERT:
                        Connection.Insert(op.model, "ON CONFLICT IGNORE", op.type);
                        break;
                    case Method.INSERT_MANY:
                        foreach (var q in op.enumerable)  //Don't use internal InsertAll, We are already in transaction
                        {
                            Connection.Insert(q, "ON CONFLICT IGNORE", op.type);
                        }
                        break;
                    case Method.INSERT_OR_REPLACE:
                        Connection.InsertOrReplace(op, op.type);
                        break;
                    case Method.UPDATE:
                        Connection.Update(op.model, op.type);
                        break;
                    case Method.UPDATEALL:
                        foreach (var q in op.enumerable)
                        {
                            Connection.Update(q, op.type);

                        }
                        break;
                    case Method.DELETE:
                        typeof(SQLiteConnection).GetMethod("Delete").MakeGenericMethod(op.type).Invoke(this, new object[] {op.id }); //Reflexion to get the generic method, not the most elegant way
                        break;
                    case Method.DELETEALL:
                        typeof(SQLiteConnection).GetMethod("DeleteAll").MakeGenericMethod(op.type).Invoke(this, new object[] {}); //Reflexion to get the generic method, not the most elegant way
                        break;
                }

            }
            Connection.Commit();
        }

        internal void Stop()
        {
            stoped = true;
            updater.Wait();
        }
    }
}

public class PlaylistData
{
    public string Name { get; set; }
    public int Id { get; set; }
    public string Creator { get; set; }
    public int SongCnt { get; set; }
}

public static class Queries
{
    public const string TransactionTriggerQuery = @"
CREATE TRIGGER IF NOT EXISTS OnTransactionAdded
AFTER INSERT ON CurrencyTransaction
BEGIN
INSERT OR REPLACE INTO CurrencyState (Id, UserId, Value, DateAdded) 
    VALUES (COALESCE((SELECT Id from CurrencyState where UserId = NEW.UserId),(SELECT COALESCE(MAX(Id),0)+1 from CurrencyState)),
            NEW.UserId, 
            COALESCE((SELECT Value+New.Value FROM CurrencyState Where UserId = NEW.UserId),NEW.Value),  
            NEW.DateAdded);
END
";
    public const string DeletePlaylistTriggerQuery = @"
CREATE TRIGGER IF NOT EXISTS music_playlist
AFTER DELETE ON MusicPlaylist
FOR EACH ROW
BEGIN
    DELETE FROM PlaylistSongInfo WHERE PlaylistId = OLD.Id;
END";
}
