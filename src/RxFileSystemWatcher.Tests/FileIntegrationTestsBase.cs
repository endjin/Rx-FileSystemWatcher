namespace Tests
{
	using System;
	using System.IO;
	using NUnit.Framework;

	public class FileIntegrationTestsBase
	{
		protected string TempPath;

		[SetUp]
		public void BeforeEachTest()
		{
			this.TempPath = Guid.NewGuid().ToString();
			Directory.CreateDirectory(this.TempPath);
		}

		[TearDown]
		public void AfterEachTest()
		{
			if (!Directory.Exists(this.TempPath))
			{
				return;
			}

			Directory.Delete(this.TempPath, true);
		}
	}
}