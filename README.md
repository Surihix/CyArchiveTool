# CyArchiveTool

This small program allows you to unpack and repack the .pack archive files that are present inside the cygames folder in the PC version of Zone of Enders 2 MARS. the program should be launched from command prompt with a few argument switches to perform a function. the list of valid argument switches are given below:

<br>**Tool actions:**
<br>``-u`` Unpack all files stored in a pack file
<br>``-uaf`` Unpacks a specific file stored in a pack file
<br>``-uad`` Unpacks a specific directory along with sub directories, stored in the pack file
<br>``-r`` Repack a folder containing valid extracted files into a pack file
<br>``-?`` or ``-h`` Display the help page
<br>

## Important notes
- Make sure you have .net 6.0 runtime installed to use this tool. you can get the setup file from [here](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.23-windows-x64-installer);

- To display the help page, run the program with either the `-?` or `-h` switches. for example `CyArchiveTool -?`

- When repacking a file, you would have to provide the old .pack file along with the extracted folder path.

- Once repacking is done, the old .pack file will be renamed with a .old extension, while the new pack file will be renamed to the same name as the old pack file without the .old extension.

## For Developers:
The following package was used for lz4 compression and decompression:
<br>**K4os.Compression.LZ4** - https://www.nuget.org/packages/K4os.Compression.LZ4
<br><br>Refer to the format structure of the .pack file from [here](FormatStruct.md). a 010 bt template has also been provided in this repo.
