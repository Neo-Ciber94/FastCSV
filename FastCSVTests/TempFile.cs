using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace FastCSV.Tests
{
    public sealed class TempFile : IDisposable
    {
        private FileInfo _fileInfo;

        public TempFile(string extension = null)
        {
            string fileName = Path.GetTempFileName() + (extension ?? string.Empty);
            _fileInfo = new FileInfo(fileName);

            //// Create the file and close the used FileStream
            using var _ = _fileInfo.Create();
        }        

        public string FullName
        {
            get
            {
                ThrowIfDisposed(); 
                return _fileInfo.FullName;
            }
        }

        public string Name
        {
            get
            {
                ThrowIfDisposed();
                return _fileInfo.Name;
            }
        }

        public void Dispose()
        {
            ThrowIfDisposed();
            _fileInfo.Delete();
            _fileInfo = null;

            GC.SuppressFinalize(this);
        }

        ~TempFile()
        {
            Dispose();
        }

        private void ThrowIfDisposed()
        {
            if (_fileInfo == null)
            {
                throw new ObjectDisposedException("TempFile was closed");
            }
        }
    }

    [TestFixture()]
    public class TempFileTest
    {
        [Test]
        public void CreateTempFileTest()
        {
            string fileName = null;

            using (var tempFile = new TempFile())
            {
                fileName = tempFile.FullName;

                Assert.IsTrue(File.Exists(fileName));

                using (var writer = new StreamWriter(fileName))
                {
                    writer.Write("Hello World");
                    writer.Flush();
                }

                using (var reader = new StreamReader(fileName))
                {
                    Assert.AreEqual("Hello World", reader.ReadToEnd());
                }
            }

            Assert.IsFalse(File.Exists(fileName));
        }
    }
}
