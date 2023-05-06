using K4os.Compression.LZ4;

namespace CyArchiveTool
{
    internal class CyUnpack
    {
        public static void DecompressArchive(string inFileVar)
        {
            CmnMethods.FileExistsDel("ProcessLog.txt");

            using (StreamWriter logProcess = new("ProcessLog.txt", append: true))
            {
                Console.WriteLine("Unpacking....");
                Console.WriteLine("");

                using (FileStream packFileStream = new(inFileVar, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader packFileReader = new(packFileStream))
                    {
                        var packFilePath = Path.GetFullPath(inFileVar);
                        var packFileFolder = Path.GetDirectoryName(packFilePath);
                        var extractDir = packFileFolder + "\\" + Path.GetFileNameWithoutExtension(inFileVar);

                        if (Directory.Exists(extractDir))
                        {
                            Directory.Delete(extractDir, true);
                        }
                        Directory.CreateDirectory(extractDir);

                        CmnMethods.ReadBytesUInt32(packFileReader, 12, out uint fileCountPos);
                        CmnMethods.ReadBytesUInt32(packFileReader, fileCountPos, out uint totalFileCount);


                        var readValueStartPos = fileCountPos + 16;
                        var dataSectionStartPos = (totalFileCount * 256) + fileCountPos + 16;
                        uint fCount = 1;
                        for (int i = 0; i < totalFileCount; i++)
                        {
                            CmnMethods.ReadBytesUInt32(packFileReader, readValueStartPos, out uint cmpFileSize);
                            CmnMethods.ReadBytesUInt32(packFileReader, readValueStartPos + 8, out uint uncmpFileSize);
                            CmnMethods.ReadBytesUInt32(packFileReader, readValueStartPos + 12, out uint filePosInDataSection);

                            packFileStream.Seek(dataSectionStartPos + filePosInDataSection, SeekOrigin.Begin);
                            byte[] cmpBuffer = new byte[cmpFileSize];
                            packFileStream.Read(cmpBuffer, 0, cmpBuffer.Length);

                            var dcmpBuffer = new byte[uncmpFileSize];
                            LZ4Codec.Decode(cmpBuffer, 0, cmpBuffer.Length, dcmpBuffer, 0, dcmpBuffer.Length);

                            var fExtn = "";
                            using (FileStream dcmpOutFile = new(extractDir + "\\FILE_" + fCount, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                            {
                                dcmpOutFile.Write(dcmpBuffer, 0, dcmpBuffer.Length);
                                ExtensionCheck(dcmpOutFile, ref fExtn);
                            }

                            Console.WriteLine("FILE_" + fCount + fExtn);
                            logProcess.WriteLine("FILE_" + fCount + fExtn);

                            Console.WriteLine("compressed size: " + cmpFileSize);
                            logProcess.WriteLine("compressed size: " + cmpFileSize);

                            Console.WriteLine("uncompressed size: " + uncmpFileSize);
                            logProcess.WriteLine("uncompressed size: " + uncmpFileSize + "\n");

                            File.Move(extractDir + "\\FILE_" + fCount, extractDir + "\\FILE_" + fCount + fExtn);
                            Console.WriteLine("");

                            readValueStartPos += 256;
                            fCount++;
                        }

                        Console.WriteLine("");
                        Console.WriteLine("Finished extracting " + Path.GetFileName(inFileVar) + " file");
                        logProcess.WriteLine("\nFinished extracting " + Path.GetFileName(inFileVar) + " file");

                        Console.ReadLine();
                    }
                }
            }          
        }


        static void ExtensionCheck(FileStream streamNameVar, ref string fExtnVar)
        {
            using (BinaryReader dcmpOutFileReader = new(streamNameVar))
            {
                dcmpOutFileReader.BaseStream.Position = 0;
                var headerChars = dcmpOutFileReader.ReadChars(4);
                var readHeader = string.Join("", headerChars).Replace("\0", "");

                switch (readHeader)
                {
                    case "DDS ":
                        fExtnVar = ".dds";
                        break;
                    case "DXBC":
                        fExtnVar = ".dxb";
                        break;
                    case "mdzx":
                        fExtnVar = ".mdzx";
                        break;
                    case "mdl0":
                        fExtnVar = ".mdlbin";
                        break;
                    default:
                        fExtnVar = ".bin";
                        break;
                }

                if (readHeader.StartsWith("{"))
                {
                    fExtnVar = ".json";
                }
            }
        }
    }
}