using System.Data;
using System.Data.SQLite;
using System.IO;
using NUnit.Framework;
using System.Linq;
using Dapper;

namespace AspNet.Identity.Dapper.Test.nUnit {
	[TestFixture]
	public class DatabaseTest {

		[Test]
		public void CanCreateInMemoryDatabaseConnection() {
			using ( var connection = DbHelper.GetInMemoryDbConnection() ) {
				connection.Open();
				Assert.IsTrue( connection.State == ConnectionState.Open );
			}
		}

		[Test]
		public void CanCreateDatabaseTables() {
			using ( var connection = DbHelper.GetInMemoryDbConnection() ) {
				DbHelper.CreateTables( connection );
				var bla = DbHelper.GetTables( connection );
				Assert.IsTrue( bla.Count == 2 );
			}
		}

		[Test]
		public void CanCreateFileDatabaseConnection() {
			using ( var connection = DbHelper.GetDbConnection( DbHelper.FileConnectionString ) ) {
				if ( File.Exists( DbHelper.DbFileName ) ) File.Delete( DbHelper.DbFileName );
				SQLiteConnection.CreateFile( DbHelper.DbFileName );
				connection.Open();
				Assert.IsTrue( connection.State == ConnectionState.Open );

				DbHelper.CreateTables( connection );
				var bla = DbHelper.GetTables( connection );
				Assert.IsTrue( bla.Count == 2 );
			}
		}

		[Test]
		public void DbMemory_CanUserInsertSql() {
			using ( var connection = DbHelper.GetTestDatabaseConnection() ) {
				DbHelper.UserInsert( connection );
				var result = connection.Query<DapperUser<string>>( "select * from Users where UserName = '_test_'" );
				Assert.IsTrue( result.Any() );
			}
		}

		[Test]
		public void DbFile_CanUserInsertSql() {
			using ( var connection = DbHelper.GetDbConnection( DbHelper.FileConnectionString ) ) {
				if ( File.Exists( DbHelper.DbFileName ) ) File.Delete( DbHelper.DbFileName );
				SQLiteConnection.CreateFile( DbHelper.DbFileName );
				connection.Open();
				DbHelper.CreateTables( connection );
				Assert.IsTrue( connection.State == ConnectionState.Open );

				DbHelper.UserInsert( connection );

				var result = connection.Query<DapperUser<string>>( "select * from Users where UserName = '_test_'" );
				Assert.IsTrue( result.Any() );
			}
		}


	}
}

