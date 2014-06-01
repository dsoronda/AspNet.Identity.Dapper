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

		static readonly string creteTableseSql = @"
CREATE TABLE  Users  (
    UserId  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    UserName TEXT NOT NULL,
    PasswordHash TEXT,
    SecurityStamp TEXT
);

CREATE TABLE ExternalLogins (
    ExternalLoginId TEXT NOT NULL,
    UserId TEXT NOT NULL,
    LoginProvider TEXT NOT NULL,
    ProviderKey TEXT NOT NULL,
    rowId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL
);
";
		static readonly string dropTablesSql = @"
drop table users;
drop table ExternalLogins;
 ";

		public static void CreateTables ( IDbConnection connection ) {
			if ( connection.State != ConnectionState.Open ) {
				connection.Open ( );
			}

			using ( var command = connection.CreateCommand ( ) ) {
				command.CommandText = creteTableseSql;
				command.ExecuteNonQuery ( );
			}

		}

		public static void DeleteTables ( IDbConnection connection ) {
			if ( connection.State != ConnectionState.Open ) {
				connection.Open ( );
			}

			using ( var command = connection.CreateCommand ( ) ) {
				command.CommandText = dropTablesSql;
				command.ExecuteNonQuery ( );
			}

		}

		public static List<string> GetTables ( DbConnection connection ) {
			if ( connection.State != ConnectionState.Open ) {
				connection.Open ( );
			}
			var list = new List<string> ( );

			using ( var command = connection.CreateCommand ( ) ) {
				var tables = connection.GetSchema ( "Tables" );
				foreach ( DataRow item in tables.Rows ) {
					if ( item [ 2 ].ToString ( ).Contains ( "sqlite" ) )
						continue;
					list.Add ( item [ 2 ].ToString ( ) );
				}
				
				return list;
			}
		}

		public static DbConnection GetInMemoryDbConnection ( ) {
			return GetDbConnection ( InMemoryConnectionString ) as DbConnection;
		}

		public static DbConnection GetDbConnection ( string connectionString ) {
			return new SQLiteConnection ( connectionString ) as DbConnection;
		}
	}
}
