# BitField

`BitField` is a high-performant C# library for managing 2D bitfields. Engineered for speed and memory efficiency, it is designed for high-frequency applications like real-time cellular automata, large-scale procedural generation, and complex 2D boolean grid analysis.

The library uses **1-based indexing** for public API calls and maintains an internal, inaccessible 1-cell border to optimize neighbor-checking algorithms.

## Table of Contents
- Key Features
- Properties
- Constructors
- Indexers
- Core Methods
- Mutation Methods
- Slicing Operations
- Neighborhood Analysis
- Batch Processing
- Debug Helpers

---

## Key Features
- **Efficient Storage**: Uses a custom `BinaryNumber` wrapper around `BitArray` for bitwise operations.
- **Flexible Indexing**: Supports 1D indices, 2D coordinates (x, y), `Vector2`, and `SadRogue.Primitives.Point`.
- **Dynamic Resizing**: Insert or delete rows and columns on the fly.
- **Neighborhood Logic**: Built-in support for Moore (3x3) and von Neumann (Cartesian) neighborhood calculations.
- **Slice Operations**: High-performance bitwise operations on horizontal and vertical segments.

## Properties

| Property | Type | Description |
| :--- | :--- | :--- |
| `RowSize` | `uint` | Number of rows (y-dimension) excluding borders. |
| `ColSize` | `uint` | Number of columns (x-dimension) excluding borders. |
| `Height` | `int` | Integer representation of `RowSize`. |
| `Width` | `int` | Integer representation of `ColSize`. |
| `Count` | `int` | Total number of cells in the field, including internal borders. |

## Constructors

### `BitField(uint rows, uint columns, uint borders = 1)`
Initializes a new field with the specified dimensions.
- `rows`: Number of rows.
- `columns`: Number of columns.
- `borders`: Initial bit value (0 or 1) for the internal border.

### `BitField(BitField old_field)`
Creates a deep copy of an existing `BitField`.

## Indexers

`BitField` supports multiple ways to access cell states (returns `bool`):
- `field[int index1D]`: Access via a flat 1D index.
- `field[SadRogue.Primitives.Point pos]`: Access via a GoRogue/SadConsole compatible Point.
- `field[int x, int y]`: Access via x (Row) and y (Col) coordinates.

## Core Methods

### `Clear()`
Resets the entire field to its initial state based on the current size and border value.

### `ChangeBorders(uint borders)`
Updates all internal border cells to the specified bit value (0 or 1).

### `GetCell(uint row, uint col)` / `GetCell(Vector2 coords)`
Returns the bit value (`uint`: 0 or 1) at the specified 1-indexed coordinates.

### `SetCell(uint row, uint col, uint val)` / `SetCell(Vector2 coords, uint val)`
Sets the bit value at the specified 1-indexed coordinates.

### `InBounds(uint row, uint col)` / `InBounds(Vector2 coords)`
Returns `true` if the coordinates are within the playable field (excluding borders).

### `CombineFields(IEnumerable<BitField> other_fields)`
Performs a bitwise **OR** operation between this field and a collection of other fields.

### `DifferenceFields(IEnumerable<BitField> other_fields)`
Subtracts (bitwise) the bits of the provided fields from this field.

## Mutation Methods

### `InsertRow(uint index)` / `InsertCol(uint index)`
Inserts a new empty row or column at the specified index. Existing data is shifted accordingly, and borders are recalculated.

### `DeleteRow(uint index)` / `DeleteCol(uint index)`
Removes the row or column at the specified index.

## Slicing Operations

Slices must be either horizontal (`row1 == row2`) or vertical (`col1 == col2`).

### `SetSlice(uint row1, uint col1, uint row2, uint col2, uint val)`
Sets an entire horizontal or vertical segment to the specified value. Horizontal slices use optimized bit-masking.

### `GetSlice(uint row1, uint col1, uint row2, uint col2)`
Returns the bit values of the slice packed into a `ulong`.

### `GetSliceOR(uint row1, uint col1, uint row2, uint col2)`
Returns `1` if **any** cell in the slice is `1`.

### `GetSliceAND(uint row1, uint col1, uint row2, uint col2)`
Returns `1` if **all** cells in the slice are `1`.

## Neighborhood Analysis

### `GetCellNeighbors(uint row, uint col)`
Returns a 9-bit `uint` mask representing the cell and its 8 neighbors. The bits are packed based on relative position.

### `GetAllSetCellNeighbors(uint row, uint col)`
Returns the count of set bits (1s) in the 3x3 neighborhood (Moore neighborhood), including the center cell.

### `GetAllSetCartesianNeighbors(uint row, uint col)`
Returns the count of set bits (1s) in the von Neumann neighborhood (the center cell + its 4 cardinal neighbors).

## Batch Processing

For performance-critical updates (like parallel cellular automata), use the queuing system to avoid modifying the field while it is being read.

### `QueueFillCell(uint row, uint col)`
Queues a cell to be set to `1`. Thread-safe.

### `QueueEmptyCell(uint row, uint col)`
Queues a cell to be set to `0`. Thread-safe.

### `RunQueue()`
Processes all queued operations in bulk using optimized bitwise operations. Queued fills are processed before queued empties.

## Debug Helpers

### `ToBMP(string name, string color, bool overlay)`
*(Available in DEBUG builds)* 
Exports the current state of the BitField to a BMP file.
- `name`: Filename prefix (saved as `../../{name}Field.bmp`).
- `color`: Hex color string for set bits (e.g., `"0xFF000000"`).
- `overlay`: If `true`, draws onto the existing BMP of the same name instead of clearing the canvas.

---

## Internal Structure: BinaryNumber
The `BinaryNumber` struct is an internal wrapper for `BitArray` that enables:
- Bitwise operators: `<<`, `>>`, `&`, `|`, `~`.
- Conversion to `uint`, `ulong`, and `BigInteger`.
- Dynamic resizing during shifts.

## Dependencies
- `System.Numerics`
- `System.Drawing` (for BMP export)
- `Medallion.Bits`
- `SadRogue.Primitives`

## License
This project is licensed under the MIT License.
