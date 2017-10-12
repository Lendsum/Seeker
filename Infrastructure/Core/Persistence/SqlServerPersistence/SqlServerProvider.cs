using Dapper;
using Lendsum.Crosscutting.Common;
using Lendsum.Crosscutting.Common.Extensions;
using Lendsum.Crosscutting.Common.Serialization;
using Lendsum.Infrastructure.Core.Exceptions;
using Lendsum.Infrastructure.Core.Locks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace Lendsum.Infrastructure.Core.Persistence.SqlServerPersistence
{
    /// <summary>
    /// Persistence provider using sql server as storage
    /// </summary>
    public class SqlServerProvider : IPersistenceProvider
    {
        private List<string> sequencesDetected = new List<string>();
        private ITextSerializer serializer;
        private IOptions<ConnectionStrings> configuration;
        private static string SystemSequenceName = "SystemCounter";

        private static int initialized = 0;

        /// <summary>
        /// The last character
        /// </summary>
        public string LastCharacter => this.lastCharacter;

        private string lastCharacter = @"ÿ";

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerProvider" /> class.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        /// <param name="config">The configuration.</param>
        public SqlServerProvider(ITextSerializer serializer, IOptions<ConnectionStrings> config)
        {
            this.random = new Random(DateTime.UtcNow.Millisecond);

            this.serializer = Check.NotNull(() => serializer);

            this.configuration = config;

            InitDatabase();
        }

        /// <summary>
        /// Gets the current system version according to the events stored.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public ulong CurrentSystemVersion
        {
            get
            {
                using (var conn = this.GetNewConnection())
                {
                    this.CreateSequenceIfNotExists(SystemSequenceName);
                    return conn.ExecuteScalar<ulong>(S.Invariant($"SELECT current_value FROM sys.sequences WHERE name = '{SystemSequenceName}' ;"));
                }
            }
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public PersistenceResultEnum Delete(IPersistable item)
        {
            Check.NotNull(() => item);

            Func<SqlConnection, PersistenceResultEnum> func = (conn) =>
            {
                var rows = conn.ExecuteScalar<int>("DELETE FROM [EventSourcing].[EventsTableCompressed] Where DocumentKey=@param1 ", new { param1 = item.DocumentKey }, this.transaction);
                if (rows <= 1) return PersistenceResultEnum.Success;
                else return PersistenceResultEnum.DocumentOutOfDate;
            };

            return this.RunQuery(func);
        }

        /// <summary>
        /// Deletes all keys and values from the persistence layer.
        /// </summary>
        public void DeleteAll()
        {
            NotInProductionPlease();
            Func<SqlConnection, PersistenceResultEnum> func = (conn) =>
            {
                conn.ExecuteScalar<int>("DELETE FROM [EventSourcing].[EventsTableCompressed]", null, this.transaction);
                conn.ExecuteScalar<int>("DELETE FROM [EventSourcing].[Lockers]", null, this.transaction);
                return PersistenceResultEnum.Success;
            };

            this.RunQuery(func);
        }

        /// <summary>
        /// Gets the value stored in the key provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public T GetValue<T>(string key)
        {
            return this.GetValues<T>(new string[] { key }).FirstOrDefault();
        }

        /// <summary>
        /// Gets the values stored in the keys provided.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="keys">The keys.</param>
        /// <returns></returns>
        public IEnumerable<T> GetValues<T>(IEnumerable<string> keys)
        {
            Check.NotNull(() => keys);

            IEnumerable<string> values;
            Func<SqlConnection, IEnumerable<string>> func = (conn) =>
            {
                return conn.Query<string>(
                    "select JsonDecompressed from [EventSourcing].[EventsTableCompressed] Where DocumentKey in @param1 "
                    , new { param1 = keys }
                    , this.transaction);
            };

            values = this.RunQuery(func);

            List<T> result = new List<T>();
            foreach (var value in values)
            {
                var newItem = this.serializer.Deserialize<T>(value) as IPersistable;
                result.Add((T)newItem);
            }

            return result;
        }

        /// <summary>
        /// Gets the keys by key pattern.
        /// </summary>
        /// <param name="startKey">The start key.</param>
        /// <param name="endKey">The end key.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="includeKeys">if set to <c>true</c> the query will return the starKey and endKey inside the range to search.</param>
        /// <returns></returns>
        public IEnumerable<string> GetKeysByKeyPattern(string startKey, string endKey = "", int? limit = null, bool includeKeys = false)
        {
            if (string.IsNullOrWhiteSpace(endKey))
            {
                endKey = startKey + LastCharacter;
            }

            string moreThan = includeKeys ? ">=" : ">";
            string lessThan = includeKeys ? "<=" : "<";

            string query = "SELECT ";
            if (limit != null) query = query + S.Invariant($"TOP {limit.Value}");
            query = query + @" DocumentKey
                from  [EventSourcing].[EventsTableCompressed]
                where DocumentKey" + moreThan + "@param1 and DocumentKey" + lessThan + "@param2";

            Func<SqlConnection, IEnumerable<string>> func = (conn) =>
            {
                var result = conn.Query<string>(query, new { param1 = startKey, param2 = endKey }, this.transaction);
                return result;
            };

            return this.RunQuery(func);
        }

        /// <summary>
        /// Gets the values by key pattern.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="startKey">The start key.</param>
        /// <param name="endKey">The end key.</param>
        /// <param name="includeKeys">if set to <c>true</c> the query will return the starKey and endKey inside the range to search.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<T> GetValuesByKeyPattern<T>(string startKey, string endKey = "", bool includeKeys = false)
        {
            if (string.IsNullOrWhiteSpace(endKey))
            {
                endKey = startKey + LastCharacter;
            }

            string moreThan = includeKeys ? ">=" : ">";
            string lessThan = includeKeys ? "<=" : "<";

            string query = "SELECT ";
            query = query + @" JsonDecompressed from  [EventSourcing].[EventsTableCompressed]
                               where DocumentKey" + moreThan + @"@param1
                               and DocumentKey" + lessThan + "@param2";

            Func<SqlConnection, IEnumerable<string>> func = (conn) =>
            {
                return conn.Query<string>(query, new { param1 = startKey, param2 = endKey }, this.transaction);
            };

            IEnumerable<string> raws = this.RunQuery(func);

            List<T> result = new List<T>();
            foreach (var raw in raws)
            {
                T item = this.serializer.Deserialize<T>(raw);
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Gets the values by key pattern.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="limit">The limit.</param>
        /// <param name="startKey">The start key.</param>
        /// <param name="endKey">The end key.</param>
        /// <param name="includeKeys">if set to <c>true</c> the query will return the starKey and endKey inside the range to search.</param>
        /// <returns></returns>
        public BatchQuery<T> GetValuesByKeyPattern<T>(int limit, string startKey, string endKey = "", bool includeKeys = false) where T : IPersistable
        {
            if (string.IsNullOrWhiteSpace(endKey))
            {
                endKey = startKey + LastCharacter;
            }

            string moreThan = includeKeys ? ">=" : ">";
            string lessThan = includeKeys ? "<=" : "<";

            string query = "SELECT ";
            query = query + S.Invariant($"TOP {limit}");
            query = query + @" JsonDecompressed from  [EventSourcing].[EventsTableCompressed]
                where DocumentKey" + moreThan + "@param1 and DocumentKey" + lessThan + "@param2";

            Func<SqlConnection, IEnumerable<string>> func = (conn) =>
            {
                return conn.Query<string>(query, new { param1 = startKey, param2 = endKey }, this.transaction);
            };

            IEnumerable<string> raws = this.RunQuery(func);

            List<T> result = new List<T>();
            foreach (var raw in raws)
            {
                T item = this.serializer.Deserialize<T>(raw);
                result.Add(item);
            }

            return new BatchQuery<T>
            {
                NextStartKey = result.LastOrDefault()?.DocumentKey,
                EndKey = endKey,
                Items = result
            };
        }

        /// <summary>
        /// Inserts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public PersistenceResultEnum Insert(IPersistable item)
        {
            Check.NotNull(() => item);

            var newCas = (long)this.random.Next(0, 9999999);
            item.Cas = (ulong)newCas;
            var raw = this.serializer.Serialize(item);
            try
            {
                Func<SqlConnection, PersistenceResultEnum> func = (conn) =>
                 {
                     conn.Execute(@"INSERT INTO [EventSourcing].[EventsTableCompressed]
                   ([DocumentKey]
                   ,[CAS]
                   ,[CreatedDate]
                   ,[LastDateModified]
                   ,[JsonCompressed]) VALUES (
                    @param1,
                    @param2,
                    GETUTCDATE(),
                    GETUTCDATE(),
                    COMPRESS(@param3))",
                       new { param1 = item.DocumentKey, param2 = newCas, param3 = raw },
                       transaction: this.transaction);

                     return PersistenceResultEnum.Success;
                 };

                return this.RunQuery(func);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return PersistenceResultEnum.KeyAlreadyExist;
                }

                throw;
            }
        }

        /// <summary>
        /// Reserves a number taking it from the AllEventsCounter
        /// </summary>
        /// <returns>
        /// A number not used before
        /// </returns>
        public ulong ReserveCounter()
        {
            return this.ReserveCounter(SystemSequenceName);
        }

        /// <summary>
        /// Reserves the counter.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public ulong ReserveCounter(string key)
        {
            this.CreateSequenceIfNotExists(key);

            Func<SqlConnection, ulong> func = (conn) =>
            {
                return conn.ExecuteScalar<ulong>(S.Invariant($"SELECT NEXT VALUE FOR EventSourcing.{key};"), null, this.transaction);
            };

            return this.RunQuery(func);
        }

        /// <summary>
        /// Update or insert the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public PersistenceResultEnum UpdateOrInsert(IPersistable item)
        {
            Check.NotNull(() => item);

            Func<SqlConnection, PersistenceResultEnum> func;
            if (item.Cas == 0)
            {
                var newCas = (long)this.random.Next(0, 9999999);
                item.Cas = (ulong)newCas;
                var raw = this.serializer.Serialize(item);
                var commandWithoutCas = @"
                merge [EventSourcing].[EventsTableCompressed] as target
                using (select @param1 as DocumentKey,@param2 as NewCas, COMPRESS(@param3) as JsonCompressed) as source
                on (target.DocumentKey=Source.DocumentKey)
                When Matched Then
	                Update Set target.JsonCompressed=source.JsonCompressed, target.CAS=source.NewCas, target.LastDateModified=GETUTCDATE()
                When not matched by target then
	                insert (DocumentKey, CAS, CreatedDate, LastDateModified, JsonCompressed) values (source.DocumentKey, NewCas, GETUTCDATE(), GETUTCDATE(),JsonCompressed)
                output
	                $action, inserted.DocumentKey;
                ";

                func = (conn) =>
                  {
                      var queryResult = conn.Query(commandWithoutCas,
                   new
                   {
                       param1 = item.DocumentKey,
                       param2 = newCas,
                       param3 = raw
                   },
                   this.transaction
                   );

                      if (queryResult.Count() == 1) return PersistenceResultEnum.Success;
                      else if (queryResult.Count() == 0) return PersistenceResultEnum.DocumentOutOfDate;
                      else return PersistenceResultEnum.NonDefinedError;
                  };
            }
            else
            {
                var newCas = (long)this.random.Next(0, 9999999);
                var oldCas = (long)item.Cas;
                item.Cas = (ulong)newCas;
                var raw = this.serializer.Serialize(item);
                var command = @"
                merge [EventSourcing].[EventsTableCompressed] as target
                using (select @param1 as DocumentKey,@param2 as CAS,@param3 as NewCas, COMPRESS(@param4) as JsonCompressed) as source
                on (target.DocumentKey=Source.DocumentKey)
                When Matched and target.CAS=source.CAS Then
	                Update Set target.JsonCompressed=source.JsonCompressed, target.CAS=source.NewCas, target.LastDateModified=GETUTCDATE()
                When not matched by target then
	                insert (DocumentKey, CAS, CreatedDate, LastDateModified, JsonCompressed) values (source.DocumentKey, NewCas, GETUTCDATE(), GETUTCDATE(),JsonCompressed)
                output
	                $action, inserted.DocumentKey;
                ";

                func = (conn) =>
                {
                    var queryResult = conn.Query(command,
                                       new
                                       {
                                           param1 = item.DocumentKey,
                                           param2 = oldCas,
                                           param3 = newCas,
                                           param4 = raw
                                       },
                                       this.transaction
                                       );

                    if (queryResult.Count() == 1) return PersistenceResultEnum.Success;
                    else if (queryResult.Count() == 0) return PersistenceResultEnum.DocumentOutOfDate;
                    else return PersistenceResultEnum.NonDefinedError;
                };
            }

            try
            {
                return this.RunQuery(func);
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return PersistenceResultEnum.KeyAlreadyExist;
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the lock.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="maxExecutingInSeconds">The maximum executing in seconds.</param>
        /// <returns></returns>
        /// <exception cref="EventSourcingException">The key already exists</exception>
        public LockerInfo GetLock(string itemName, int maxExecutingInSeconds)
        {
            var mergeCommand = @"
                merge [EventSourcing].[Lockers] as target
                using (select @param1 as ItemName, @param2 as MaxSeconds) as source
                on (target.ItemName=Source.ItemName)
                When matched and (target.InProgress = 0 or DATEADD(s,MaxSeconds,target.LockInitDate) < GETUTCDATE() )  then
    				Update Set
                    target.LockInitDate= GETUTCDATE(),
					InProgress = 1
                When not matched by target then
	                insert (ItemName, LockInitDate, InProgress) values (source.ItemName, GETUTCDATE(), 1)
                output
	                $action, inserted.ItemName, inserted.InProgress;
                ";
            Func<SqlConnection, LockerInfo> func;
            func = (conn) =>
            {
                var queryResult = conn.Query(mergeCommand,
                                   new
                                   {
                                       param1 = itemName,
                                       param2 = maxExecutingInSeconds
                                   },
                                   this.transaction
                                   );

                if (queryResult.Count() == 1)
                {
                    var itemOutput = queryResult.First();

                    return new LockerInfo()
                    {
                        ItemName = itemOutput.ItemName,
                        InProgress = itemOutput.InProgress
                    };
                }
                else if (queryResult.Count() == 0)
                {
                    return null;
                }
                else
                {
                    throw new EventSourcingException("Can not be different number of actions than 0 or 1");
                }
            };
            try
            {
                var lockerInfo = this.RunQuery(func);
                return lockerInfo;
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    return null;
                }

                throw;
            }
        }

        /// <summary>
        /// Release the lock
        /// </summary>
        /// <param name="itemName">The name of the locker.</param>
        public void ReleaseLock(string itemName)
        {
            var mergeCommand = @"
                merge [EventSourcing].[Lockers] as target
                using (select @param1 as ItemName) as source
                on (target.ItemName=Source.ItemName)
                When matched and target.InProgress = 1 then
                    Update Set
					InProgress = 0
                output
	                $action, inserted.ItemName;
                ";
            Func<SqlConnection, PersistenceResultEnum> func;
            func = (conn) =>
            {
                var queryResult = conn.Query(mergeCommand,
                                   new
                                   {
                                       param1 = itemName
                                   },
                                   this.transaction
                                   );

                if (queryResult.Count() == 1) return PersistenceResultEnum.Success;
                else if (queryResult.Count() == 0) return PersistenceResultEnum.DocumentOutOfDate;
                else return PersistenceResultEnum.NonDefinedError;
            };
            try
            {
                this.RunQuery(func);
            }
            catch (SqlException)
            {
                throw;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void InitDatabase()
        {
            if (initialized != 0) return;
            if (Interlocked.Exchange(ref initialized, 1) != 0) return;

            using (var conn = this.GetNewConnection())
            {
                var tableCount = conn.Query<int>("select count(TABLE_NAME) from information_schema.tables where TABLE_NAME='EventsTableCompressed'");
                if (tableCount.Count() == 0 || tableCount.First() == 0)
                {
                    try
                    {
                        conn.ExecuteScalar(createScheme);
                    }
                    catch { }

                    try
                    {
                        conn.ExecuteScalar(createTable);
                    }
                    catch { }
                }

                tableCount = conn.Query<int>("select count(TABLE_NAME) from information_schema.tables where TABLE_NAME='Lockers'");
                if (tableCount.Count() == 0 || tableCount.First() == 0)
                {
                    try
                    {
                        conn.ExecuteScalar(createLockerTable);
                    }
                    catch { }
                }

                this.CreateSequenceIfNotExists(SystemSequenceName);
            }
        }

        private TResult RunQuery<TResult>(Func<SqlConnection, TResult> func)
        {
            if (this.transaction != null)
            {
                return func(this.transaction.Connection);
            }
            else
            {
                using (var conn = this.GetNewConnection())
                {
                    return func(conn);
                }
            }
        }

        private IEnumerable<string> DetectSequences(string name)
        {
            using (var conn = this.GetNewConnection())
            {
                this.sequencesDetected = new List<string>();
                var sequences = conn.Query<string>("SELECT seq.name as name from sys.sequences as seq where name=@param1", new { param1 = name });
                foreach (var sequence in sequences)
                {
                    this.sequencesDetected.AddIfNotExist(sequence, (x, y) => x.Equals(y));
                }

                return sequences;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The connection must be kept alive")]
        private SqlConnection GetNewConnection()
        {
            var connection = new SqlConnection(this.connectionString);
            connection.Open();
            return connection;
        }

        private void CreateSequenceIfNotExists(string name, int tries = 0)
        {
            if (this.sequencesDetected.Contains(name)) return;
            this.DetectSequences(name);
            if (this.sequencesDetected.Contains(name)) return;

            try
            {
                using (var conn = this.GetNewConnection())
                {
                    conn.ExecuteScalar(createSequence.Replace("%name%", name));
                }
            }
            catch
            {
                if (tries <= 10)
                {
                    CreateSequenceIfNotExists(name, tries + 1);
                }
                else
                {
                    throw;
                }
            }

            this.sequencesDetected.AddIfNotExist(name, (x, y) => x == y);
        }

        /// <summary>
        /// Begins the transaction scope, use the returned object to perfom operations over the database.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The dispose must be called outside this class, in the using for example")]
        public IPersistenceProvider BeginScope()
        {
            var result = new SqlServerProvider(this.serializer, this.configuration);
            var connection = this.GetNewConnection();
            result.transaction = connection.BeginTransaction();
            return result;
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public void Commit()
        {
            if (this.transaction == null) throw new EventSourcingException("The transaction is not set");
            this.transaction.Commit();
            this.transaction?.Connection?.Dispose();
            this.transaction = null;
        }

        /// <summary>
        /// Rollbacks this instance.
        /// </summary>
        public void Rollback()
        {
            if (this.transaction == null) throw new EventSourcingException("The transaction is not set");
            this.transaction.Rollback();
            this.transaction.Connection?.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.transaction != null)
                {
                    this.transaction?.Connection?.Dispose();
                    this.transaction.Dispose();
                }
            }
        }

        /// <summary>
        /// This method is just a mechanism protection to avoid delete everything if the vpn is on and
        /// the host file points to production.
        /// </summary>
        private void NotInProductionPlease()
        {
            var productionIps = new string[] { "10.185.186.9", "10.0.1.4" };

            if (productionIps.Any(x => this.connectionString.Contains(x)))
            {
                throw new EventSourcingException("Estas intentado borrar todo de produccion, comenta este codigo si realmente quieres hacerlo y luego dejalo descomentado");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "maybe will be cached in the future")]
        private string connectionString
        {
            get
            {
                var database=this.configuration.Value.Values.Where(x => x.Name == "EventsDatabase").FirstOrDefault();
                if (database == null) throw new EventSourcingException("The connection is not set");
                return database.Value;
            }
        }

        private SqlTransaction transaction { get; set; }

        private string createScheme = @"
            CREATE SCHEMA EventSourcing;";

        private string createTable = @"
            CREATE TABLE [EventSourcing].[EventsTableCompressed](
	        [DocumentKey] [nvarchar](200) NOT NULL,
            [CreatedDate] [datetime] NOT NULL,
            [LastDateModified] [datetime] NOT NULL,
	        [CAS] [bigint] NOT NULL,
	        [JsonCompressed] [varbinary](max) NOT NULL,
            [JsonDecompressed] AS CAST(DECOMPRESS(JsonCompressed) AS nvarchar(max))
            CONSTRAINT [PK_EventsTableCompressed] PRIMARY KEY CLUSTERED
            (
	            [DocumentKey] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];
            ";

        private string createLockerTable = @"
            CREATE TABLE [EventSourcing].[Lockers](
	        [ItemName] [nvarchar](200) NOT NULL,
            [LockInitDate][datetime] NOT NULL,
	        [InProgress]  [bit] NOT NULL

            CONSTRAINT [PK_Lockers] PRIMARY KEY CLUSTERED
            (
	            [ItemName] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
            ) ON [PRIMARY];
            ";

        private string createSequence = @"
            CREATE SEQUENCE EventSourcing.%name% start with 1 increment by 1;";

        private Random random;
    }
}