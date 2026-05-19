# CyArchiveTool

This small program allows you to unpack and repack the .pack archive files that are present inside the cygames folder in the PC version of Zone of Enders 2 MARS. the program should be launched from command prompt with a few argument switches to perform a function. the list of valid argument switches are given below:

<br>**Tool actions:**
- ``-u`` Unpacks all files stored in a pack file
- ``-uaf`` Unpacks a specific file stored in a pack file
- ``-uad`` Unpacks a specific directory along with sub directories, stored in the pack file. specify the `*` character, after the last path separator in the path.
- ``-r`` Repacks a folder containing valid extracted files to a pack file. files are packed uncompressed.
- ``-rc`` Same as ``-r`` swtich, but the files are packed compressed
- ``-raf`` Repack a single file into a pack file. file will be appended at the end of the pack file and will be packed, depending on how the original file is packed.
- ``-rad`` Repacks a specific directory along with sub directories containing files, into a pack file. files will be packed, just like how its done in the ``-raf`` swtich. specify the `*` character, after the last path separator in the path.
- ``-?`` or ``-h`` Display the help page

## Important notes
- Make sure you have .net 6.0 runtime installed to use this program. you can get the setup file from [here](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.23-windows-x64-installer).
- To display the help page, run the program with either the `-?` or `-h` switches. for example `CyArchiveTool -?`
- Unless you know what you are doing, do not edit the `#hash-entry-table.csv` or the `#path-table.csv` files from the unpacked folder. these files are required when using either the `-r` or `-rc` swtiches, that creates a pack file from scratch. so if any sort of invalid data is present in either one of these csv files, then the repacking functions would either fail or create a corrupted pack file. 
- Once repacking is done, if a .pack file with the same name as the unpacked folder exists, then that file will be renamed with a .old extension. the newly created/repacked pack file, will be named with the name of the unpacked folder, with a .pack extension.

## For Developers:
- The following package was used for lz4 compression and decompression:
<br>**K4os.Compression.LZ4** - https://www.nuget.org/packages/K4os.Compression.LZ4
- Refer to the format structure of the .pack file from [here](FormatStruct.md). a 010 bt template has also been provided in this repo.
