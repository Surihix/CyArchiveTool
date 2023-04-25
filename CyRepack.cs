using K4os.Compression.LZ4;
using System.Buffers.Binary;

namespace CyArchiveTool
{
    internal class CyRepack
    {
        public static void CompressArchive(string inFolderVar, string inPackFileVar)
        {
            var extractedDir = Path.GetFileNameWithoutExtension(inFolderVar);
            string[] extractedDirToCheck = Directory.GetFiles(inFolderVar, "*.*", SearchOption.TopDirectoryOnly);

            CmnMethods.FileExistsDel("ProcessLog.txt");
            
            using (StreamWriter logProcess = new("ProcessLog.txt", append: true))
            {
                Console.WriteLine("Repacking....");
                Console.WriteLine("");

                var finalPackedFile = inPackFileVar + ".new";

                CmnMethods.FileExistsDel(finalPackedFile);
                CmnMethods.FileExistsDel(extractedDir + "\\" + "_tempPacked.bin");

                using (FileStream oldPackFileStream = new(inPackFileVar, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader oldPackFileReader = new(oldPackFileStream))
                    {
                        CmnMethods.ReadBytesUInt32(oldPackFileReader, 12, out uint fileCountPos);
                        CmnMethods.ReadBytesUInt32(oldPackFileReader, fileCountPos, out uint totalFileCount);
                        var fileInfoSize = totalFileCount * 256;


                        Console.WriteLine("Checking extracted files....");
                        var checkCount = 0;
                        var checkFCount = 1;
                        for (int c = 0; c < totalFileCount; c++)
                        {
                            foreach (var ef in extractedDirToCheck)
                            {
                                var getFileName = Path.GetFileNameWithoutExtension(ef);
                                if (getFileName.Equals("FILE_" + checkFCount))
                                {
                                    checkCount++;
                                }
                            }

                            checkFCount++;
                        }

                        if (checkCount < totalFileCount)
                        {
                            logProcess.WriteLine("One or more files missing in the extracted folder");
                            logProcess.Close();
                            CmnMethods.ErrorExit("One or more files missing in the extracted folder");
                        }

                        Console.WriteLine("Finished checking extracted files");
                        Console.WriteLine("");
                        Thread.Sleep(1000);


                        using (MemoryStream fileInfoStream = new())
                        {
                            oldPackFileStream.Seek(fileCountPos + 16, SeekOrigin.Begin);
                            byte[] fileInfoBuffer = new byte[fileInfoSize];
                            var bytesToRead = oldPackFileStream.Read(fileInfoBuffer, 0, fileInfoBuffer.Length);
                            fileInfoStream.Write(fileInfoBuffer, 0, bytesToRead);

                            using (BinaryWriter fileInfoWriter = new(fileInfoStream))
                            {
                                using (FileStream newPackFile = new(finalPackedFile, FileMode.OpenOrCreate, FileAccess.Write))
                                {
                                    using (FileStream tempDataPackFile = new(extractedDir + "\\" + "_tempPacked.bin", FileMode.Append, FileAccess.Write))
                                    {

                                        var fCount = 1;
                                        uint fPos = 0;
                                        uint fCmpSize = 0;
                                        uint fActualSize = 0;
                                        uint fileInfoWriterPos = 0;
                                        for (int i = 0; i < totalFileCount; i++)
                                        {
                                            foreach (var f in extractedDirToCheck)
                                            {
                                                var currentFile = Path.GetFileNameWithoutExtension(f);

                                                if (currentFile.Equals("FILE_" + fCount))
                                                {
                                                    File.Copy(f, extractedDir + "\\" + currentFile);

                                                    fPos = (uint)tempDataPackFile.Length;
                                                    fActualSize = (uint)new FileInfo(extractedDir + "\\" + currentFile).Length;

                                                    var source = File.ReadAllBytes(extractedDir + "\\FILE_" + fCount);
                                                    var cmpTarget = new byte[fActualSize];
                                                    var decompress = LZ4Codec.Encode(source, cmpTarget, LZ4Level.L00_FAST);

                                                    fCmpSize = (uint)decompress;
                                                    tempDataPackFile.Write(cmpTarget, 0, decompress);

                                                    AdjustBytesUInt32(fileInfoWriter, fileInfoWriterPos, fCmpSize);
                                                    AdjustBytesUInt32(fileInfoWriter, fileInfoWriterPos + 8, fActualSize);
                                                    AdjustBytesUInt32(fileInfoWriter, fileInfoWriterPos + 12, fPos);

                                                    File.Delete(extractedDir + "\\" + currentFile);
                                                    Console.WriteLine("Repacked " + currentFile);
                                                    logProcess.WriteLine("Repacked " + currentFile);

                                                    Console.WriteLine("Compressed size: " + fCmpSize);
                                                    Console.WriteLine("UnCompressed size: " + fActualSize);
                                                    logProcess.WriteLine("Compressed size: " + fCmpSize);
                                                    logProcess.WriteLine("UnCompressed size: " + fActualSize);

                                                    var filePosInArchive = fileCountPos + 16 + fileInfoSize + fPos;
                                                    Console.WriteLine("Position: " + filePosInArchive);
                                                    Console.WriteLine("");
                                                    logProcess.WriteLine("Position: " + filePosInArchive + "\n");
                                                }
                                            }

                                            fCount++;
                                            fileInfoWriterPos += 256;
                                        }
                                    }

                                    oldPackFileStream.Seek(0, SeekOrigin.Begin);
                                    byte[] fileHashBuffer = new byte[fileCountPos + 16];
                                    var HashbytesToRead = oldPackFileStream.Read(fileHashBuffer, 0, fileHashBuffer.Length);
                                    newPackFile.Write(fileHashBuffer, 0, HashbytesToRead);

                                    fileInfoStream.Seek(0, SeekOrigin.Begin);
                                    fileInfoStream.CopyTo(newPackFile);

                                    using (FileStream repackedData = new(extractedDir + "\\" + "_tempPacked.bin", FileMode.Open, FileAccess.Read))
                                    {
                                        repackedData.Seek(0, SeekOrigin.Begin);
                                        repackedData.CopyTo(newPackFile);
                                    }
                                }
                            }
                        }
                    }
                }

                File.Delete(extractedDir + "\\" + "_tempPacked.bin");

                Console.WriteLine("");
                Console.WriteLine("Finished repacking files to " + finalPackedFile);
                logProcess.WriteLine("\nFinished repacking files to " + finalPackedFile);

                Console.ReadLine();
            }
        }


        static void AdjustBytesUInt32(BinaryWriter writerNameVar, uint writerPosVar, uint valueToAdjustVar)
        {
            writerNameVar.BaseStream.Position = writerPosVar;
            byte[] adjustValue = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(adjustValue, valueToAdjustVar);
            writerNameVar.Write(adjustValue);
        }
    }
}