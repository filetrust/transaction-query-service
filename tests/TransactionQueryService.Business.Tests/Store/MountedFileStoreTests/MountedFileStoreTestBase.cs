using Glasswall.Administration.K8.TransactionQueryService.Business.Store;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading;
using TestCommon;

namespace TransactionQueryService.Business.Tests.Store.MountedFileStoreTests
{
    public class MountedFileStoreTestBase : UnitTestBase<MountedFileStore>
    {
        protected string RootPath;
        protected Mock<ILogger<MountedFileStore>> Logger;
        protected CancellationToken CancellationToken;

        public void SharedSetup(string rootPath = null)
        {
            rootPath ??= $".{Path.DirectorySeparatorChar}{Guid.NewGuid()}";
            RootPath = rootPath;
            Logger = new Mock<ILogger<MountedFileStore>>();
            CancellationToken = new CancellationToken(false);
            ClassInTest = new MountedFileStore(Logger.Object, RootPath);
        }
    }
}
