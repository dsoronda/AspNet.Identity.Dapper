using System;
using System.Data;
using NUnit.Framework;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Data.SQLite;

namespace AspNet.Identity.Dapper.Test.nUnit {

	public static class DbHelper {
		public static readonly string InMemoryConnectionString = "Data Source=:memory:;Version=3;New=True;";
		public static string FileConnectionString { get { return string.Format( @"Data Source={0} ;Version=3;New=True;", DbFileName ); } }
		public static readonly string DbFileName = @"d:\identity_test.sqlite";

		private const string creteTableseSql = @"
CREATE TABLE  Users  (
    UserId  TEXT NOT NULL,
    UserName TEXT NOT NULL,
    PasswordHash TEXT,
    SecurityStamp TEXT,
	rowId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
);

CREATE TABLE ExternalLogins (
    ExternalLoginId TEXT NOT NULL,
    UserId TEXT NOT NULL,
    LoginProvider TEXT NOT NULL,
    ProviderKey TEXT NOT NULL,
    rowId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
);
";
		private const string dropTablesSql = @"
drop table users;
drop table ExternalLogins;
 ";

		public static void CreateTables( IDbConnection connection ) {
			if ( connection.State != ConnectionState.Open ) {
				connection.Open();
			}

			using ( var command = connection.CreateCommand() ) {
				command.CommandText = creteTableseSql;
				command.ExecuteNonQuery();
			}

		}

		public static void DeleteTables( IDbConnection connection ) {
			if ( connection.State != ConnectionState.Open )
				connection.Open();

			using ( var command = connection.CreateCommand() ) {
				command.CommandText = dropTablesSql;
				command.ExecuteNonQuery();
			}

		}

		#region User CRUD sql
		public static void UserInsert( IDbConnection connection ) {
			if ( connection.State != ConnectionState.Open )
				connection.Open();

			using ( var command = connection.CreateCommand() ) {
				command.CommandText = "insert into Users( UserId, UserName, PasswordHash, SecurityStamp) values ( '_test_', '_test_', '_test_', '_test_' )";
				command.ExecuteNonQuery();
			}
		}
		public static void UserDelete( IDbConnection connection ) {
			if ( connection.State != ConnectionState.Open )
				connection.Open();

			using ( var command = connection.CreateCommand() ) {
				command.CommandText = "delete from Users where UserId = '_test_' )";
				command.ExecuteNonQuery();
			}
		}
		#endregion

		public static List<string> GetTables( DbConnection connection ) {
			if ( connection.State != ConnectionState.Open ) {
				connection.Open();
			}
			var list = new List<string>();

			using ( var command = connection.CreateCommand() ) {
				var tables = connection.GetSchema( "Tables" );
				foreach ( DataRow item in tables.Rows ) {
					if ( item[2].ToString().Contains( "sqlite" ) )
						continue;
					list.Add( item[2].ToString() );
				}

				return list;
			}
		}

		public static DbConnection GetInMemoryDbConnection() {
			return GetDbConnection( InMemoryConnectionString ) as DbConnection;
		}

		public static DbConnection GetTestDatabaseConnection() {
			var connection = GetInMemoryDbConnection();
			CreateTables( connection );
			return connection;
		}

		public static DbConnection GetDbConnection( string connectionString ) {
			return new SQLiteConnection( connectionString ) as DbConnection;
		}

	}
}
