## Format Structure

**Important:** The byte values would have to be read in Little Endian.

### Header Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4  | String | ZPAC, header |
| 0x4 | 0x4 | UInt32 | Unknown, always 0x1 and same value in all .pack files |
| 0x8 | 0x4 | UInt32 | HashEntry Table section offset, always 0x10 and same value in all pack files |
| 0xC | 0x4 | UInt32 | FileEntry Table section offset |


### Hash Entry Table Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x00 | 0x4  | UInt32 | Hash Entry Count, dividing this value by 2 should also return the File Count |
| 0x4 | 0xC | UInt32[3] | Reserved, always null |
| Hash Entry Table | 0x8 * Hash Entry Count | (#Hash Entry) | Hash Entries |

#### Hash Entry

| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | StrCode32 hash of FilePath, if null treat this offset and the next 4 bytes as null |
| 0x4 | 0x1 | UInt8 | Unknown flag, always 0x10 |
| 0x5 | 0x1 | UInt16 | File Index |
| 0x6 | 0x1 | UInt8 | Reserved, always null |

<br> The File Entry Table section begins immediately after the last Hash Entry and the position should match with the FileEntry Table section offset in the header.


### File Entry Table Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | File entry count, also number of files in the .pack file |
| 0x4 | 0xC | UInt32[3] | Reserved, always null |

#### File Entry
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | Compressed data size |
| 0x4 | 0x4 | UInt32 | Unknown |
| 0x8 | 0x4 | UInt32 | Uncompressed data size |
| 0xC | 0x4 | UInt32 | Data offset, relative from first file's data |
| 0x10 | 0x4 | UInt32 | Unknown2, value is 1 in all .pack files |
| 0x14 | 0xE0 | UInt32[56] | Hash or encrypted filename, same size in all .pack files |
| 0xE0 | 0xC | UInt32[3] | Reserved, always null |

#### Notes
- The compression used is raw lz4. use the library linked in the main readme document to decompress and compress the data.