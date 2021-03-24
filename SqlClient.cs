// SQL Client
using System;
using System.Security;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient; // install-package system.data.sqlclient
using System.Linq;

namespace PlayingAround
{
    class Program
    {
        private readonly static string _server = "devsql";
        private readonly static string _catalog = "Activebank1";
        private readonly static string _username = "esb";

        static void Main(string[] args)
        {
            var program = new Program();

            if (1 == 0)
            {
                program.SimpleConnection();
            }

            program.Run();
        }

        public void SimpleConnection()
        {
            var connectionString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password = {3}", _server, _catalog, _username, "foo");
            var connection = new SqlConnection(connectionString);
            connection.Open();
            connection.Close();
        }

        public void Run() // using System.Data.SqlClient;
        {
            var connectionString = string.Format("Data Source={0};Initial Catalog={1};", _server, _catalog);

            using var password = GetPassword(string.Format("Enter the password for '{0}.{1}' user '{2}': ", _server, _catalog, _username)); // Will automatically call IDisposable.Dispose() on completion

            using (var connection = new SqlConnection(connectionString, new SqlCredential(_username, password))) // Will automatically call IDisposable.Dispose() on completion
            {
                connection.Open();

                // CREATE - Really any raw SQL
                ExecuteQuery(connection, @"create table dbo.Users (Identifier int identity(1, 1), Firstname varchar(50) not null, Surname varchar(50) not null)");

                // INSERT
                InsertUsers(connection, User.GetUsers());

                // BULK INSERT
                BulkInsert(connection, User.GetUsers());

                // SELECT
                var users = GetUsersFromDatabase(connection);
                Console.WriteLine("Users:");
                foreach (var user in users)
                    Console.WriteLine(user.ToString);

                // UPDATE
                users[0].Firstname = "Jon";
                users[1].Firstname = "NULL";

                foreach (var user in users.OrderBy(u => u.Identifier).Take(2)) // using System.Linq;
                    UpdateUser(connection, user);

                users = GetUsersFromDatabase(connection);
                Console.WriteLine("Users:");
                foreach (var user in users)
                    Console.WriteLine(user.ToString);

                ExecuteQuery(connection, @"drop table dbo.Users");
            }
        }

        public int ExecuteQuery(SqlConnection connection, string sql)
        {
            using var command = new SqlCommand(sql, connection);
            return command.ExecuteNonQuery();
        }

        public void InsertUsers(SqlConnection connection, IEnumerable<User> users)
        {
            var sql = @"insert into dbo.Users (Firstname, Surname) values (@firstname, @surname)";
            using var insert = new SqlCommand(sql, connection);

            foreach (var user in users)
            {
                insert.Parameters.AddWithValue("@firstname", user.Firstname);
                insert.Parameters.AddWithValue("@surname", user.Surname);
                insert.ExecuteNonQuery();
                insert.Parameters.Clear();
            }
        }

        public void BulkInsert(SqlConnection connection, IEnumerable<User> users)
        {
            var table = new DataTable();
            table.Columns.Add(new DataColumn("Firstname", typeof(string)));
            table.Columns.Add(new DataColumn("Lastname", typeof(string)));

            foreach (var user in users)
            {
                var row = table.NewRow();
                row["Firstname"] = user.Firstname;
                row["Lastname"] = user.Surname;
                table.Rows.Add(row);
            }

            using var bulk = new SqlBulkCopy(connection) { DestinationTableName = "dbo.Users" };
            // Mapping is not required if the names are consistent but if you provide one column you have to provide them all
            bulk.ColumnMappings.Add("Firstname", "Firstname"); 
            bulk.ColumnMappings.Add("Lastname", "Surname");
            bulk.WriteToServer(table);
            table.Clear(); 
        }

        public List<User> GetUsersFromDatabase(SqlConnection connection)
        {
            var sql = @"select Identifier, Firstname, Surname from dbo.Users";
            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            var users = new List<User>();
            while (reader.Read())
                users.Add(new User { Identifier = reader.GetInt32(0), Firstname = reader.GetString(1), Surname = reader.GetString(2) });

            return users;
        }

        public void UpdateUser(SqlConnection connection, User userToUpdate)
        {
            if (userToUpdate.Identifier == null)
                throw new ArgumentNullException("User.Identifier must be populated");

            var sql = @"update dbo.Users set Firstname = @firstname, Surname = @surname where Identifier = @identifier";

            var transactionName = "UpdateUser";
            using var transaction = connection.BeginTransaction(transactionName);

            using var command = new SqlCommand(sql, connection, transaction);
            command.Parameters.AddWithValue("@identifier", userToUpdate.Identifier);
            command.Parameters.AddWithValue("@firstname", userToUpdate.Firstname);
            command.Parameters.AddWithValue("@surname", userToUpdate.Surname);
            
            var rowsUpdated = command.ExecuteNonQuery();
            Console.Write("{0} row updated. Transaction ", rowsUpdated);

            if (!"null".Equals(userToUpdate.Firstname, StringComparison.OrdinalIgnoreCase))
            {
                transaction.Commit();
                Console.WriteLine("committed");
            }
            else
            {
                transaction.Rollback(transactionName);
                Console.WriteLine("rolled back");
            }
        }

        public SecureString GetPassword(string prompt) // using System.Security;
        {
            Console.Write(prompt);

            ConsoleKeyInfo key;
            var password = new SecureString();
            do
            {
                key = Console.ReadKey(true);

                var consoleKeyValue = (int)key.Key;

                if ((consoleKeyValue >= (int)ConsoleKey.D0 && consoleKeyValue <= (int)ConsoleKey.D9)
                    || (consoleKeyValue >= (int)ConsoleKey.A && consoleKeyValue <= (int)ConsoleKey.Z))
                {
                    password.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.RemoveAt(password.Length - 1);
                }
            } while (key.Key != ConsoleKey.Enter);
            password.MakeReadOnly();
            Console.WriteLine("\n");
            return password;
        }
    }

    public class User
    {
        public int? Identifier { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }

        public new string ToString => string.Format("Identifier: {0}, Firstname: {1}, Surname: {2}", Identifier != null ? Identifier.ToString() : "None", Firstname, Surname);

        public static List<User> GetUsers() // using System.Collections.Generic;
        {
            return new List<User> { new User { Firstname = "Joe", Surname = "Bloggs" }, new User { Firstname = "Foo", Surname = "Bar" } };
        }
    }
}
