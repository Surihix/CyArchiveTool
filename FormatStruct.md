## Format Structure

**Important:** The format is not fully parsed and the byte values would have to be read in Little Endian.

### Header Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4  | String | ZPAC, header |
| 0x4 | 0x4 | UInt32 | Unknown, always 0x1 and same value in all .pack files |
| 0x8 | 0x4 | UInt32 | Header size, always 0x10 and same value in all pack files |
| 0xC | 0x4 | UInt32 | FileTable section offset |


### UnkTable Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x00 | 0x4  | UInt32 | UnkTable Entry Count, dividing this value by 2 should return the File Count |
| 0x4 | 0xC | UInt32[3] | Reserved, always null |

#### UnkTable Entry
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | Some sort of hash, sometimes null |
| 0x4 | 0x1 | UInt8 | Unknown flag, always 0x10 and sometimes null |
| 0x5 | 0x1 | UInt8 | Unknown flag 2, sometimes null |
| 0x6 | 0x1 | UInt8 | Unknown flag 3, sometimes null |
| 0x7 | 0x1 | UInt8 | Unknown flag 4, sometimes null |

<br> The FileTable section begins immediately after the last UnkTable Entry and the position should match with the FileTable section offset in the header.


### FileTable Section
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | File count, number of files in the .pack file |
| 0x4 | 0xC | UInt32[3] | Reserved, always null |

#### File Entry
| Offset | Size | Type | Description |
| --- | --- | --- | --- |
| 0x0 | 0x4 | UInt32 | Compressed data size |
| 0x4 | 0x4 | UInt32 | Unknown |
| 0x8 | 0x4 | UInt32 | Uncompressed data size |
| 0xC | 0x4 | UInt32 | Data start position, relative |
| 0x10 | 0x4 | UInt32 | Unknown 2, value is 1 in all .pack files |
| 0x14 | 0xE0 | UInt32[56] | Hash or encrypted filename, same size in all .pack files |
| 0xE0 | 0xC | UInt32[3] | Reserved, always null |

#### Notes
- The compression used is raw lz4. use the library linked in the main readme document to decompress and compress the data.