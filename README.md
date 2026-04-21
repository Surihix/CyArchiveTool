# CyArchiveTool

This small program allows you to unpack and repack the .pack archive files that are present inside the cygames folder in the PC version of Zone of Enders 2 MARS. the program should be launched from command prompt with a few argument switches to perform a function. the list of valid argument switches are given below:

<br>**Tool actions:**
<br>``-u`` Unpack all files stored in a pack file
<br> ``-uwp`` Unpacks all files stored in a pack file with filepaths.
<br>``-r`` Repack a folder containing valid extracted files into a pack file
<br>``-?`` or ``-h`` Display the help page
<br>

## Important notes
- To display the help page, run the program with either the `-?` or `-h` switches. for example `CyArchiveTool -?`

- Due to the pack archive file containing only hashes for file paths, the ``-u`` function will unpack files, with the name `FILE_` which will then be suffixed with the file's index number. for example `FILE_56.dds`.

- The ``-uwp`` function can unpack files from the pack archive with proper file paths, by making use of a text file containing filepaths for each file that is stored in the pack archive.
<br>This function checks each and every provided file path in the text file by hashing them and then comparing the hash with the hashes that exists in the pack archive. if the hash exists, then the file path is used for the unpacked file. if the hash does not exist, then the file is unpacked with the `FILE_` and index number as the filename. 

- You can use the text files given in the [pack_paths_E.F.G.I.J.S](pack_paths_E.F.G.I.J.S) folder in this repo, to quickly unpack files with the correct file paths with the ``-uwp``.
<br>Do note that all of these paths were tested only for the full version game files and it may or may not be valid for the orange case demo version pack files.

- You can add a ` ?` characters at the end of a file path to make the tool skip checking the hash and use the path as is.
<br>For example ``cygames/check_points/cpひな形.csv ?``

- If you want to try generating file paths for a pack file, then you can do so with the [CyArchivePathGenerator](https://github.com/Surihix/CyArchivePathGenerator) tool.

- When repacking a file, you would have to provide the old .pack file along with the extracted folder path. the extracted folder should contain all the files and the `##path_mappings.txt` file.

- Once repacking is done, the old .pack file will be renamed with a .old extension, while the new pack file will be renamed to the same name as the old pack file without the .old extension.

## For Developers:
The following package was used for lz4 compression and decompression:
<br>**K4os.Compression.LZ4** - https://www.nuget.org/packages/K4os.Compression.LZ4
<br><br>Refer to the format structure of the .pack file from [here](FormatStruct.md). a 010 bt template has also been provided in this repo.
