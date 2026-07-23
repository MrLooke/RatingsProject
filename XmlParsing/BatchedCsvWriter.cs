using System.Text;

namespace XmlParsing
{
    /// <summary>
    /// Writes CSV rows across a series of numbered files ("{basePath}_1.csv", "{basePath}_2.csv", ...),
    /// starting a new file with a fresh header row every <c>batchSize</c> rows.
    /// </summary>
    internal sealed class BatchedCsvWriter : IDisposable
    {
        private readonly string _basePath;
        private readonly string _headers;
        private readonly int _batchSize;

        private StreamWriter _writer;
        private int _fileNumber = 1;
        private int _rowCount;

        internal BatchedCsvWriter(string basePath, string headers, int batchSize = 100_000)
        {
            _basePath = basePath;
            _headers = headers;
            _batchSize = batchSize;
            _writer = OpenFile();
        }

        internal void WriteLine(string row)
        {
            _writer.WriteLine(row);

            if (++_rowCount >= _batchSize)
            {
                _writer.Dispose();
                _fileNumber++;
                _writer = OpenFile();
                _rowCount = 0;
            }
        }

        private StreamWriter OpenFile()
        {
            var writer = new StreamWriter($"{_basePath}_{_fileNumber}.csv", false, Encoding.UTF8);
            writer.WriteLine(_headers);
            return writer;
        }

        public void Dispose() => _writer.Dispose();
    }
}
