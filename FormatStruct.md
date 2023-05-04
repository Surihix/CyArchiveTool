## Format Structure

**Important:** The format is not fully parsed and the byte values would have to be read in Little Endian.

#### Header Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4  | String | ZPAC, header |
| 0x4 | 0x4 | UInt32 | Unknown, same value in other pack files too |
| 0x8 | 0x4 | UInt32 | Unknown, same value in other pack files too |
| 0xC | 0x4 | UInt32 | FileCount offset |
| 0x10 | 0x4  | UInt32 | total offsets that are populated between 0x20 till the FileCount offset |
| 0x14 | 0xC | UInt32[3] | Reserved, always null |

The file table begins 12 bytes after the file count offset.
<br>

#### File table section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | Compressed data size |
| 0x4 | 0x4 | UInt32 | Unknown |
| 0x8 | 0x4 | UInt32 | Uncompressed data size |
| 0xC | 0x4 | UInt32 | Data start position, relative |
| 0x10 | 0x4 | UInt32 | Unknown, value is always 1 in other .pack files too |
| 0x14 | 0xE0 | Unknown | Unknown, bytes are always the same length in other pack files too |
| 0xE0 | 0xC | UInt32[3] | Reserved, always null |

### Notes
- The compression used is raw lz4. use the library linked in the main readme document to decompress and compress the data.
- The offset value at 0x10 can be divided by 2 to get the total filecount.
- The section between 0x20 till the file count offset most probably contains hashed filenames.
