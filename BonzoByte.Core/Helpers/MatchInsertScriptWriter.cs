using System.Text;

namespace BonzoByte.Core.Helpers
{
    public sealed class MatchInsertScriptWriter : IDisposable
    {
        private readonly string _basePath;
        private readonly int _rowsPerInsert;
        private readonly int _rowsPerFile;     // npr. 200_000
        private readonly long _maxFileBytes;   // npr. 200L * 1024 * 1024
        private readonly MatchRowMapper _mapper;

        private StreamWriter? _writer;
        private int _fileIndex = 0;
        private int _rowsInCurrentFile = 0;
        private int _rowsInCurrentInsert = 0;
        private bool _insertOpen = false;
        private string _columnList;

        public MatchInsertScriptWriter(string basePath,
                                       MatchRowMapper mapper,
                                       int rowsPerInsert = 1000,
                                       int rowsPerFile = 200_000,
                                       long maxFileBytes = 200L * 1024 * 1024)
        {
            _basePath = basePath;
            _mapper = mapper;
            _rowsPerInsert = rowsPerInsert;
            _rowsPerFile = rowsPerFile;
            _maxFileBytes = maxFileBytes;
            _columnList = _mapper.BuildColumnList();
        }

        public void Append(Models.Match m)
        {
            EnsureFile();
            EnsureInsert();

            var line = _mapper.BuildValuesTuple(m);
            if (_rowsInCurrentInsert == 1)
                _writer!.WriteLine(line);
            else
                _writer!.WriteLine("," + line);

            _rowsInCurrentInsert++;
            _rowsInCurrentFile++;

            if (_rowsInCurrentInsert >= _rowsPerInsert)
                CloseInsert();

            if (_rowsInCurrentFile >= _rowsPerFile || (_writer!.BaseStream.Position >= _maxFileBytes))
                RotateFile();
        }

        private void EnsureFile()
        {
            if (_writer != null) return;
            var path = Path.Combine(_basePath, $"Match_Insert_Part_{++_fileIndex:D4}.sql");
            _writer = new StreamWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read), new UTF8Encoding(false));
            WriteHeader();
        }

        private void WriteHeader()
        {
            _writer!.WriteLine("USE [BonzoByte];");
            _writer!.WriteLine("SET NOCOUNT ON;");
            _writer!.WriteLine("SET XACT_ABORT ON;");
            _writer!.WriteLine();
            _writer!.WriteLine("BEGIN TRAN;");
        }

        private void EnsureInsert()
        {
            if (_insertOpen) return;
            _writer!.WriteLine();
            _writer!.WriteLine($"INSERT INTO dbo.[Match] ({_columnList}) VALUES");
            _insertOpen = true;
            _rowsInCurrentInsert = 0;
        }

        private void CloseInsert()
        {
            if (!_insertOpen) return;
            _writer!.WriteLine(";");
            _insertOpen = false;
        }

        private void RotateFile()
        {
            // zatvori otvoreni INSERT i transakciju
            CloseInsert();
            _writer!.WriteLine("COMMIT;");
            _writer!.WriteLine("GO");
            _writer!.Dispose();
            _writer = null;
            _rowsInCurrentFile = 0;
        }

        public void Dispose()
        {
            if (_writer == null) return;
            CloseInsert();
            _writer!.WriteLine("COMMIT;");
            _writer!.WriteLine("GO");
            _writer.Dispose();
            _writer = null;
        }
    }
}
