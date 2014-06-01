using System;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace AspNet.Identity.Dapper.Test.nUnit {
	[TestFixture]
	public class DapperUserStoreTest {
		[Test]
		public void CanInjectConnectionString ( ) {
			using ( var dbConnection = DbHelper.GetInMemoryDbConnection ( ) ) {
				var store = new DapperUserStore<int> ( dbConnection );
				Assert.IsNotNull ( store.Connection );
				Assert.AreEqual ( store.Connection.ConnectionString, dbConnection.ConnectionString, "Connection string should be equal!" );
			}
		}
		 
	}
}

