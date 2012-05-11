using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace AniSharp.Hash
{
    /// <summary>
    /// Ed2kHashGenerator
    /// </summary>
    class Ed2kHashGenerator
    {
        private String _sFile;
        private System.IO.FileStream _FileStream;
        
        private Int32 _length;

        private Thread mFileReader;
        private Thread[] mHashCalculators;
        private Thread mFinalCalculator;

        private BlockingCollection<Job> taskQueue = new BlockingCollection<Job>(20);

        private byte[][] preHashes;

        public const int chunkSize = 9728000;

        //private static int workerThreads = AniSharp.Properties.Settings.Default.HashWorkingThreads;
        private static int workerThreads = Environment.ProcessorCount;

        private string ed2kLink = null;

        class Job
        {
            public Job(int pos, byte[] chunk) { this.pos = pos; this.chunk = chunk; }
            public readonly int pos;
            public readonly byte[] chunk;
        }

        /// <summary>
        /// gives the resulting Ed2k-Link, or null, if not finished.
        /// Use waitForIt() for blocking.
        /// </summary>
        public String Ed2kLink
        {
            get
            {
                return ed2kLink;
            }
        }

        /// <summary>
        /// Hashes the given file. Hashing starts immediateley after construction
        /// </summary>
        /// <param name="sFile">The path to the file</param>
        public Ed2kHashGenerator(String sFile)
        {
            _sFile = sFile;
            _FileStream = new System.IO.FileStream(sFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
           
            _length = (Int32)(new System.IO.FileInfo(sFile).Length);

             preHashes = new byte[_length / chunkSize + 1][];

            
            
            mFileReader = new Thread(threadFileReader);
            if (_length > chunkSize)
            {
                mHashCalculators = new Thread[workerThreads];
                for (int i = 0; i < workerThreads; i++)
                {
                    mHashCalculators[i] = new Thread(threadFileHasherBig);
                    mHashCalculators[i].Start();
                }
            }
            else
            {
                mHashCalculators = new Thread[1];

                    mHashCalculators[0] = new Thread(threadFileHasherSmall);
                    mHashCalculators[0].Start();
                
            }

            mFileReader = new Thread(threadFileReader);

            mFileReader.Start();


            mFinalCalculator = new Thread(threadFinalCalculations);
            mFinalCalculator.Start();
        }

        /// <summary>
        /// blocks, until the hash is available
        /// </summary>
        public void waitForIt()
        {
            lock (this)
            {
                while (ed2kLink == null)
                    Monitor.Wait(this);
            }
        }

        #region " Threads "
        private void threadFileReader()
        {
            int chunkId = 0;
            byte[] buffer = new byte[chunkSize];
            int readNow = 0;
            while ((readNow = _FileStream.Read(buffer, 0, chunkSize)) > 0)
            {
                byte[] thisBuffer = new byte[readNow];
                Buffer.BlockCopy(buffer, 0, thisBuffer, 0, readNow);

                taskQueue.Add(new Job(chunkId, thisBuffer));

                chunkId++;
            }
            foreach (Thread t in mHashCalculators)
            {
                taskQueue.Add(null);
            }
        }

        private void threadFileHasherBig()
        {
            Job job;
            MD4 md4 = new MD4();

            while (true)
            {
                
                    job = taskQueue.Take();
                    if (job == null) return;
               

                md4.Reset(); md4.Append(job.chunk);
                preHashes[job.pos] = md4.Finish();
            }
        }

        private void threadFileHasherSmall()
        {
            preHashes[0] = taskQueue.Take().chunk;
        }

        private void threadFinalCalculations()
        {
            foreach (Thread t in mHashCalculators)
            {
                t.Join();
            }

            MD4 md4 = new MD4();

            foreach (byte[] x in preHashes)
            {
                md4.Append(x);
            }

            byte[] hash = md4.Finish();

            StringBuilder sb = new StringBuilder();

            sb.Append("ed2k://|file|");
            sb.Append(_sFile.Substring(_sFile.LastIndexOf(@"\")+1));
            sb.Append('|');
            sb.Append(_length);
            sb.Append('|');
   
            foreach(byte b in hash)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            lock (this)
            {
                ed2kLink = sb.ToString();
                Monitor.PulseAll(this);
            }

        }

        #endregion

        public override string ToString()
        {
            waitForIt();
            return Ed2kLink;
        }
    }
}
