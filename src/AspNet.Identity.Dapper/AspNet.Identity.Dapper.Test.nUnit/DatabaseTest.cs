using System;
using System.Data;
using NUnit.Framework;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;


namespace AspNet.Identity.Dapper.Test.nUnit {
	[TestFixture]
	public class DatabaseTest {

		[Test]
		public void CanCreateInMemoryDatabaseConnection ( ) {
			using ( var connection = DbHelper.GetInMemoryDbConnection ( ) ) {
				connection.Open ( );
				Assert.IsTrue ( connection.State == ConnectionState.Open );
			}

		}

		[Test]
		public void CanCreateDatabaseTables ( ) {
			using ( var connection = DbHelper.GetInMemoryDbConnection ( ) ) {

				DbHelper.CreateTables ( connection );
				var bla = DbHelper.GetTables ( connection );
				Assert.IsTrue ( bla.Count == 2 );
			}

		}
	}

}

