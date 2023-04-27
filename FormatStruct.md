## Format Structure

**Important:** The format is not fully parsed and the byte values would have to be read in Little Endian.

#### Header Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4  | String | ZPAC, header |
| 0x4 | 0x4 | UInt32 | Unknown, same value as other .pack files too |
| 0x8 | 0x4 | UInt32 | Unknown, same value as other .pack files too |
| 0xC | 0x4 | UInt32 | FileCount offset |
| 0x10 | 0x4  | UInt32 | total offsets that are populated between 0x20 till the FileCount offset |
| 0x14 | 0xC | UInt32[3] | Reserved, always null |

The offset value at 0x10 can be divided by 2 to get the file count.
<br>The section between 0x20 till the file count offset most probably contains hashed filenames.
<br>The file table begins 16 bytes after the file count offset.


#### File table section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | Compressed data size |
| 0x4 | 0x4 | UInt32 | Unknown |
| 0x8 | 0x4 | UInt32 | Uncompressed data size |
| 0xC | 0x4 | UInt32 | Data start position, relative |
| 0x10 | 0x4 | UInt32 | Unknown, value is always 1 |
| 0x14 | 0xE0 | Unknown | Unknown, bytes are always the same length |
| 0xE0 | 0xC | UInt32[3] | Reserved, always null |

The Compression used is raw lz4. use the decompression library linked in the main readme document to decompress the data.
