// Excel
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using ClosedXML.Excel; // https://github.com/ClosedXML/ClosedXML/wiki

namespace PlayingAround
{
    class Program
    {
        private readonly static string _root = @"C:\temp\C# Training";
        private readonly static string _excel = Path.Join(_root, "Excel");
        private readonly static string _firstWorkbook = Path.Join(_excel, "FirstWorkbook.xlsx");
        private readonly static string _secondWorkbook = Path.Join(_excel, "SecondWorkbook.xlsx");
        private readonly static Random _random = new Random();

        static void Main(string[] args)
        {
            var program = new Program();

            program.Setup();

            program.WriteFirstWorkBook();
            program.WriteSecondWorkBook();
            program.ReadSecondWorkBook();

            program.CleanUp();
        }

        public void WriteFirstWorkBook()
        {
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.AddWorksheet("Foo");
                var colA = sheet.Columns("A");
                colA.Width = 50;

                var cell = new CellPosition();

                var headers = new string[] { "Identifier", "Firstname", "Surname", "Age", "Gender" };
                foreach (var header in headers)
                {
                    sheet.Cell(cell.Reference).Value = header;
                    cell.IncrementColumn();
                }
                cell.ResetColumn();
                cell.IncrementRow();

                var id = 0;
                var rnd = new Random();
                foreach (var _ in Enumerable.Range(1, 100))
                {
                    sheet.Cell(cell.Reference).Value = ++id;
                    sheet.Cell(cell.IncrementColumn()).Value = id <= 50 ? "Foo" : "Bar";
                    sheet.Cell(cell.IncrementColumn()).Value = rnd.Next(0, 2) == 0 ? "Brown" : "Bloggs";
                    sheet.Cell(cell.IncrementColumn()).Value = rnd.Next(25, 61); ;
                    sheet.Cell(cell.IncrementColumn()).Value = "M";
                    cell.ResetColumn();
                    cell.IncrementRow();
                }

                sheet.RangeUsed().SetAutoFilter().Column(2).AddFilter("Foo"); // Filters "Firstname" to "Foo"
                sheet.AutoFilter.Sort(4, XLSortOrder.Descending); // Sorts "Age" descending

                workbook.SaveAs(_firstWorkbook); //using System.IO;
            }
        }

        public void WriteSecondWorkBook()
        {
            using var workbook = new XLWorkbook();
            var sheet = workbook.AddWorksheet("Values");

            // Heading section
            var cell = new CellPosition("B2");
            sheet.Cell(cell.Reference).Value = "User ID:";
            sheet.Cell(cell.IncrementColumn()).Value = "03459ABCIJ";

            var idRange = sheet.Range(cell.SetCellPosition("B2") + ":" + cell.IncrementColumn());
            idRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            cell.ResetColumn();
            cell.IncrementRow(2);

            // Data
            var headers = new string[] { "Identifier", "Firstname", "Surname", "Date of Birth", "Nationality", "Foo", "Bar" };
            foreach (var header in headers)
            {
                sheet.Cell(cell.Reference).Value = header;
                cell.IncrementColumn();
            }

            cell.ResetColumn();
            cell.IncrementRow();

            var id = 0;
            var nationalities = new string[] { "GB", "DE", "FR", "IR" };
            var foos = new string[] { "f1", "f2", "f3" };
            var dateStart = DateTime.Now.AddYears(-18);
            var dateRange = new TimeSpan(365 * 10, 0, 0, 0, 0); // 10 years
            foreach (var _ in Enumerable.Range(1, 100))
            {
                sheet.Cell(cell.Reference).Value = ++id;
                sheet.Cell(cell.IncrementColumn()).Value = RandomHelper.GenerateName(RandomHelper.GenerateNumber(3, 7));
                sheet.Cell(cell.IncrementColumn()).Value = RandomHelper.GenerateName(RandomHelper.GenerateNumber(4, 10));
                sheet.Cell(cell.IncrementColumn()).Value = RandomHelper.GenerateDate(dateStart, dateRange, "B");
                sheet.Cell(cell.Reference).Style.NumberFormat.Format = "dd/MM/yyyy";
                sheet.Cell(cell.IncrementColumn()).Value = RandomHelper.GetString(nationalities);
                sheet.Cell(cell.IncrementColumn()).Value = RandomHelper.GetString(foos);
                sheet.Cell(cell.IncrementColumn()).Value = RandomHelper.GenerateNumber(length: 10);
                sheet.Cell(cell.Reference).Style.NumberFormat.Format = new string('0', 10); // Prevents leading zeros being omitted
                cell.ResetColumn();
                cell.IncrementRow();
            }

            foreach (var header in headers)
            {
                sheet.Column(cell.Column).AdjustToContents();
                cell.IncrementColumn();
            }

            workbook.SaveAs(_secondWorkbook); //using System.IO;
        }

        public void ReadSecondWorkBook()
        {
            var wb = new XLWorkbook(_secondWorkbook);
            var ws = wb.Worksheet("Values");

            var userId = string.Empty;

            var row = ws.FirstRowUsed();

            var cell = row.FirstCellUsed();
            if (!cell.Value.ToString().Contains("user id", StringComparison.OrdinalIgnoreCase))
                throw new FormatException("Could not find user id");

            cell = cell.CellRight();
            userId = cell.Value.ToString();
            Console.WriteLine("Debug - User ID found at {0}, value {1}", cell.Address, userId);
            if (userId.Trim().Length != 20)
                throw new FormatException("User ID missing or incomplete");

            // Find table
            var searchCount = 0;
            do
            {
                if (++searchCount >= 100)
                    throw new FormatException("Could not find table data");

                row = row.RowBelow();
            } while (row.IsEmpty());

            if (!row.FirstCell().Value.ToString().Contains("identifier", StringComparison.OrdinalIgnoreCase))
                throw new FormatException("Could not find table data");

            var firstAddress = row.RowBelow().FirstCellUsed().Address;
            var lastAddress = ws.LastCellUsed().Address;
            Console.WriteLine("Debug - Data found at {0}:{1}", firstAddress, lastAddress);

            var npiRange = ws.Range(row.FirstCell().Address, ws.LastCellUsed().Address);

            foreach (var r in npiRange.Rows())
            {
                int i = 0;
                Console.WriteLine("Identifier: {0}", r.Cell(++i).Value.ToString());
                Console.WriteLine("Firstname: {0}", r.Cell(++i).Value.ToString());
                Console.WriteLine("Surname: {0}", r.Cell(++i).Value.ToString());
                Console.WriteLine("Date of Birth: {0}", r.Cell(++i).Value.ToString());
                Console.WriteLine("Nationality: {0}", r.Cell(++i).Value.ToString());
                Console.WriteLine("Identification Type: {0}", r.Cell(++i).Value.ToString());
                Console.WriteLine("Identification Value: {0}", r.Cell(++i).Value.ToString());
                Console.WriteLine();
            }
        }

        public void Setup()
        {
            if (!Directory.Exists(_excel))
                Directory.CreateDirectory(_excel);
        }

        public void CleanUp()
        {
            if (File.Exists(_firstWorkbook))
                File.Delete(_firstWorkbook);
            if (File.Exists(_secondWorkbook))
                File.Delete(_secondWorkbook);
            if (Directory.GetFiles(_excel).Length == 0)
                Directory.Delete(_excel);
        }
    }

    public class RandomHelper
    {
        private static readonly RandomHelper _instance = new RandomHelper();
        private static readonly Random _random = new Random();
        private static readonly string _allowedChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static RandomHelper GetInstance()
        {
            return _instance;
        }

        public static string GetString(string[] items)
        {
            return items[GenerateNumber(0, items.Length - 1)];
        }

        public static int GenerateNumber(int start, int end)
        {
            return _random.Next(start, end + 1);
        }

        public static string GenerateNumber(int length, bool canStartWithZero = true)
        {
            string output = GetNumber(canStartWithZero);
            for (var i = 0; i < length - 1; i++)
                output += GetNumber();

            return output;
        }

        public static string GenerateName(int length)
        {
            var output = GetUpper();
            for (var i = 0; i < length - 1; i++)
                output += GetLower();
            return output;
        }

        public static DateTime GenerateDate(DateTime start, TimeSpan range, string direction = "F") // Todo; move to enum
        {
            var days = (int)range.TotalDays;

            return start.AddDays(_random.Next(direction.Equals("F") ? 0 : -days, direction.Equals("B") ? 0 : days));
        }

        private static string GetNumber(bool canBeZero = true)
        {
            if (canBeZero)
                return (_allowedChars[0..10])[_random.Next(0, 10)].ToString();

            return (_allowedChars[1..10])[_random.Next(0, 9)].ToString();
        }

        private static string GetUpper()
        {
            return (_allowedChars[10..36])[_random.Next(0, 26)].ToString();
        }

        private static string GetLower()
        {
            return (_allowedChars[36..62])[_random.Next(0, 26)].ToString();
        }
    }
	
    public class CellPosition
    {
        public string Column { get; private set; }
        public int Row { get; private set; }

        public string Reference { get { return Column + Row; } }

        private static readonly Regex _regex = new Regex("^([a-zA-Z]{1,3})([1-9][0-9]*)$", RegexOptions.IgnoreCase);
        private static readonly string _maxColumn = "XFD";

        public CellPosition(string position = "A1")
        {
            SetCellPosition(position);
        }

        public string SetCellPosition(string position = "A1")
        {
            var match = _regex.Match(position);
            if (!match.Success)
                throw new ArgumentOutOfRangeException();

            // Todo; validate range
            Column = match.Groups[1].Value.ToUpper();
            Row = int.Parse(match.Groups[2].Value);

            return Reference;
        }

        public string Increment(int columnTimes, int rowTimes)
        {
            IncrementColumn(columnTimes);
            IncrementRow(rowTimes);
            return Reference;
        }

        public string IncrementColumn(int times)
        {
            foreach (var _ in Enumerable.Range(1, times))
            {
                IncrementColumn();
            }
            return Reference;
        }

        public string IncrementColumn()
        {
            if (Column.Equals(_maxColumn))
                throw new InvalidOperationException("Max column range reached: " + _maxColumn);

            var chars = Column.ToCharArray();
            if (!Column.EndsWith('Z'))
                chars[^1] = ++chars[^1];
            else
            {
                if (Column.Distinct().Count() == 1)
                {
                    chars = new char[Column.Length + 1];
                    Array.Fill(chars, 'A');
                }
                else
                {
                    var current = Column.Length - 1;
                    while (current > 0)
                    {
                        if (chars[current] != 'Z')
                        {
                            chars[current] = chars[current]--;
                            break;
                        }

                        chars[current] = 'A';
                        current--;
                    }
                }
            }
            Column = new string(chars);
            return Reference;
        }

        public string IncrementRow(int times)
        {
            if (times < 1)
                throw new ArgumentException("Must be greated than zero");

            Row += times;
            return Reference;
        }

        public string IncrementRow()
        {
            Row++;
            return Reference;
        }

        public string ResetColumn()
        {
            Column = "A";
            return Reference;
        }

        public string ResetRow()
        {
            Row = 1;
            return Reference;
        }

        public string ResetCell()
        {
            ResetColumn();
            ResetRow();
            return Reference;
        }
    }
}
