using ConsoleSignature.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleSignature.Extensions;


namespace ConsoleSignature.Services
{
    public class FileSignatureService : IDisposable
    {
        private readonly int _blockSize;
        private readonly string _filePath;

        private readonly object _readerLocker = new object();
        private readonly object _exceptionLocker = new object();

        private FileStream _fileStream;
        private uint _signatureNumber;
        private long _readedBytes;
        private long _streamLength;


        public FileSignatureService(int blockSize, string filePath)
        {
            _blockSize = blockSize;
            _filePath = filePath;

            //Разумные ограничения
            if (_blockSize <= 0)
                throw new ArgumentException("BlockSize has wrong value");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found");


        }

        public int ThreadCount { get; set; }

        public void PrintFileSignatures()
        {
            _fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _signatureNumber = 1;
            _readedBytes = 0;
            _streamLength = _fileStream.Length;

            if (ThreadCount > 0)
            {
                List<Thread> threadsList = new List<Thread>();
                for (int i = 0; i < ThreadCount; i++)
                {
                    threadsList.Add(new Thread(ReadSignature));
                }

                threadsList.StartAll();
                threadsList.WaitAll();
            }
            else
            {
                ReadSignature();
            }
        }


        private void ReadSignature()
        {
            try
            {
                while (_readedBytes < _streamLength)
                {

                    SignatureBlock block;
                    lock (_readerLocker)
                    {
                        if (_readedBytes >= _streamLength)
                            break;

                        block = new SignatureBlock();
                        block.BlockBytes = new byte[_blockSize];
                        block.BlockNumber = _signatureNumber;

                        _readedBytes += _fileStream.Read(block.BlockBytes, 0, _blockSize);

                        _signatureNumber++;
                    }

                    Console.WriteLine(block.GetBlockSHA256Str());
                }
            }
            catch (Exception exc)
            {
                lock (_exceptionLocker)
                {
                    Console.WriteLine(exc.Message);
                    Console.WriteLine(exc.StackTrace);
                }
            }
        }

        public void Dispose()
        {
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
            }

        }
    }
}
