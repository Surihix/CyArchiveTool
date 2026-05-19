using CyArchiveTool.Support;
using CyArchiveTool.Support.Structures;
using System.Text;

namespace CyArchiveTool.Repack
{
    internal class ZPACRepackCsvHelpers
    {
        public static HashEntry[] GetHashEntriesFromCSV(string hashEntryTableCsvFile)
        {
            Console.WriteLine($"Reading data from '{Path.GetFileName(hashEntryTableCsvFile)}' file....");
            Console.WriteLine("");

            var hashEntryList = new List<HashEntry>();

            using (var hashEntryTableCsvReader = new StreamReader(hashEntryTableCsvFile, Encoding.UTF8))
            {
                _ = hashEntryTableCsvReader.ReadLine();

                string readLine;
                int lineCounter = 1;

                while ((readLine = hashEntryTableCsvReader.ReadLine()) != default)
                {
                    var readLineData = readLine.Split(',');

                    if (readLineData.Length < 3)
                    {
                        SharedFunctions.ErrorExit($"Error: Not enough data specified for entry at line_{lineCounter}!");
                    }

                    var pathHashVal = readLineData[0];
                    var flagVal = readLineData[1];
                    var fileIndexVal = readLineData[2];

                    if (uint.TryParse(pathHashVal, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out uint strCode32Hash) == false)
                    {
                        SharedFunctions.ErrorExit($"Error: Invalid PathHash value is specified for entry at line_{lineCounter}!");
                    }

                    if (byte.TryParse(flagVal, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out byte unkFlag) == false)
                    {
                        SharedFunctions.ErrorExit($"Error: Invalid Flag value is specified for entry at line_{lineCounter}!");
                    }

                    if (ushort.TryParse(fileIndexVal, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out ushort fileIndex) == false)
                    {
                        SharedFunctions.ErrorExit($"Error: Invalid FileIndex value is specified for entry at line_{lineCounter}!");
                    }

                    hashEntryList.Add(new HashEntry() { StrCode32Hash = strCode32Hash, UnkFlag = unkFlag, FileIndex = fileIndex, Reserved = 0 });
                    lineCounter++;
                }
            }

            return hashEntryList.ToArray();
        }

        public static string[] GetFilePathsFromCSV(string pathTableCsvFile)
        {
            Console.WriteLine($"Reading data from '{Path.GetFileName(pathTableCsvFile)}' file....");
            Console.WriteLine("");

            var filePathsList = new List<string>();

            using (var pathTableCsvReader = new StreamReader(pathTableCsvFile, Encoding.UTF8))
            {
                _ = pathTableCsvReader.ReadLine();

                string readLine;
                int lineCounter = 1;

                while ((readLine = pathTableCsvReader.ReadLine()) != default)
                {
                    var readLineData = readLine.Split(',');

                    if (readLineData.Length < 2)
                    {
                        SharedFunctions.ErrorExit($"Error: Not enough data specified for path at line_{lineCounter}!");
                    }

                    if (SharedFunctions.ShiftJISEncoding.GetBytes(readLineData[1]).Length > 224)
                    {
                        SharedFunctions.ErrorExit($"Error: Path specified at line_{lineCounter} is too large!");
                    }

                    filePathsList.Add(readLineData[1]);
                }
            }

            return filePathsList.ToArray();
        }
    }
}