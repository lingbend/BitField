﻿#nullable enable
using System.Numerics;
using System.Collections;
using System.Drawing;

using static Medallion.Bits;
using System.IO.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BitField
{
    /// <summary>
    /// Represents a 2D field of bits, using 1-based indexing for rows and columns.
    /// The field includes an inaccessible, internal 1-cell border to simplify neighbor-checking algorithms.
    /// </summary>
    public class BitField
    {
        internal BinaryNumber _field;
        private (uint, uint) _size;

        /// <summary>
        /// Gets the number of rows (y-dimension) in the field (excluding borders).
        /// </summary>
        public uint RowSize{get{return _size.Item1;}}

        /// <summary>
        /// Gets the number of columns (x-dimension) in the field (excluding borders).
        /// </summary>
        public uint ColSize{get{return _size.Item2;}}

        /// <summary>
        /// Gets the height (y-dimension / rows) of the field.
        /// </summary>
        public int Height => (int) RowSize;

        /// <summary>
        /// Gets the width (x-dimension / columns) of the field.
        /// </summary>
        public int Width => (int) ColSize;

        /// <summary>
        /// Gets the total number of cells in the BitField (including borders).
        /// </summary>
        public int Count => _field.Count;

        /// <summary>
        /// Gets or sets the state of a cell using a flat 1D index.
        /// </summary>
        public bool this[int index1D] {
            get
            {
                return GetCell(GetPositionFromIndex((uint) index1D)) == 1;
            }
            set
            {
                SetCell(GetPositionFromIndex((uint) index1D), value ? 1u : 0);
            }
        }

        /// <summary>
        /// Gets or sets the state of a cell using a SadRogue.Primitives.Point coordinate.
        /// For compatibility with SadRogue.Primitives and GoRogue.
        /// </summary>
        public bool this[SadRogue.Primitives.Point pos]  {
            get
            {
                return GetCell(new Vector2(pos.X, pos.Y)) == 1;
            }
            set
            {
                SetCell(new Vector2(pos.X, pos.Y), value ? 1u : 0u);
            }
        }

        /// <summary>
        /// Gets or sets the state of a cell using x (Row) and y (Col) coordinates.
        /// </summary>
        public bool this[int x, int y]  {
            get
            {
                return GetCell((uint) x, (uint) y) == 1u;
            }
            set
            {
                SetCell((uint) x, (uint) y, value ? 1u : 0u);
            }
        }

        /// <summary>
        /// The current value (0 or 1) assigned to the internal inaccessible border.
        /// </summary>
        public uint BorderNum{get {return _border_num;}}

        /// <summary>
        /// The current value (0 or 1) assigned to the internal inaccessible border.
        /// </summary>
        private uint _border_num;

        /// <summary>
        /// Buffer used to track cells to be filled during parallel processing.
        /// </summary>
        private bool[] _queue_fill;

        /// <summary>
        /// Buffer used to track cells to be emptied during parallel processing.
        /// </summary>
        private bool[] _queue_empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitField"/> class.
        /// </summary>
        /// <param name="rows">Number of rows (y-dimension).</param>
        /// <param name="columns">Number of columns (x-dimension).</param>
        /// <param name="borders">Default value for the border (0 or 1).</param>
        public BitField(uint rows, uint columns, uint borders = 1)
        {
            _border_num = borders;
            if (rows == 0 || columns == 0)
            {
                throw new IndexOutOfRangeException();
            }
            _field = new BinaryNumber((int)((rows + 2) * (columns + 2)), 0);
            _queue_fill = new bool[(rows + 2) * (columns + 2)];
            _queue_empty = new bool[(rows + 2) * (columns + 2)];

            _size = (rows, columns);
            if (borders == 1)
            {
                ChangeBorders(borders);
            }
            else if (borders != 0)
            {
                throw new ArgumentException("Value must be 0 or 1. Value was: " + borders);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BitField"/> class as a copy of an existing one.
        /// </summary>
        /// <param name="old_field">The BitField to copy.</param>
        public BitField(BitField old_field)
        {
            _border_num = old_field._border_num;
            if (old_field._size.Item1 == 0 || old_field._size.Item2 == 0)
            {
                throw new IndexOutOfRangeException();
            }
            _field = new BinaryNumber((int)((old_field._size.Item1 + 2) * (old_field._size.Item2 + 2)), 0);
            _queue_fill = new bool[(old_field._size.Item1 + 2) * (old_field._size.Item2 + 2)];
            _queue_empty = new bool[(old_field._size.Item1 + 2) * (old_field._size.Item2 + 2)];

            _size = (old_field._size.Item1, old_field._size.Item2);
            if (old_field._border_num == 1)
            {
                ChangeBorders(old_field._border_num);
            }
            else if (old_field._border_num != 0)
            {
                throw new ArgumentException("Value must be 0 or 1. Value was: " + old_field._border_num);
            }
            _field = new BinaryNumber(old_field._field);
        }

        /// <summary>
        /// Resets the field to its initial state based on the current size and border value.
        /// </summary>
        public void Clear()
        {
            uint rows = _size.Item1; uint columns = _size.Item2;

            if (rows == 0 || columns == 0)
            {
                throw new IndexOutOfRangeException();
            }

            _field = new BinaryNumber((int)((rows + 2) * (columns + 2)), 0);
            _queue_fill = new bool[(rows + 2) * (columns + 2)];
            _queue_empty = new bool[(rows + 2) * (columns + 2)];

            _size = (rows, columns);
            if (_border_num == 1)
            {
                ChangeBorders(_border_num);
            }
            else if (_border_num != 0)
            {
                throw new ArgumentException("Value must be 0 or 1. Value was: " + _border_num);
            }
        }

        /// <summary>
        /// Updates the value of all border cells.
        /// </summary>
        /// <param name="borders">The bit value (0 or 1) to set the borders to.</param>
        public void ChangeBorders(uint borders)
        {
            if (borders != 1 && borders != 0)
            {
                throw new ArgumentException("Value must be 0 or 1, not: " + borders);
            }
            SetRowInternal(0, _size.Item2+1, borders, _size.Item2+1 + 1);
            SetRowInternal(_size.Item1+1, _size.Item2+1, borders, _size.Item2+1 + 1);
            for (uint row = 0; row < _size.Item1+2; row++)
            {
                SetCellInternal(row, 0, borders);
                SetCellInternal(row, _size.Item2+1, borders);
            }
            _border_num = borders;
        }

        /// <summary>
        /// Calculates the flat 1D index for the underlying BitArray based on 2D coordinates,
        /// accounting for the internal 1-cell inaccessible border.
        /// </summary>
        /// <param name="row">The 0-based or 1-based row index (depending on context).</param>
        /// <param name="col">The 0-based or 1-based column index.</param>
        /// <returns>A 1D coordinate for the 2D coordinates input.</returns>
        private uint GetCellIndex(uint row, uint col){
            return row * (_size.Item2+2) + col;
        }

        /// <summary>
        /// Calculates the 2D coordinates from a flat 1D coordinate.
        /// </summary>
        /// <param name="index">The flat 1D coordinate.</param>
        /// <returns>A vector representing the 2D coordinates.</returns>
        private Vector2 GetPositionFromIndex(uint index)
        {
            return new Vector2(index / _size.Item2, index % _size.Item2);
        }

        /// <summary>
        /// Sets a specific cell to the given value.
        /// </summary>
        /// <param name="row">1-indexed row (y).</param>
        /// <param name="col">1-indexed column (x).</param>
        /// <param name="val">Bit value (0 or 1).</param>
        public void SetCell(uint row, uint col, uint val)
        {
            if (val != 0 && val != 1)
            {
                throw new ArgumentException("Value must be 0 or 1, not: " + val);
            } 
            else if (row < 1 || col < 1 || row > _size.Item1 || col > _size.Item2)
            {
                throw new IndexOutOfRangeException("Bad Cell Index: (" + row + ", " + col + ")");
            }

            SetCellInternal(row, col, val);
        }

        /// <summary>
        /// Sets a specific cell to the given value.
        /// </summary>
        /// <param name="coords">2D vector coordinates</param>
        /// <param name="val">Bit value (0 or 1).</param>
        public void SetCell(Vector2 coords, uint val)
        {
            SetCell((uint) coords.X, (uint) coords.Y, val);
        }

        /// <summary>
        /// Queues a cell to be set to 1 during the next <see cref="RunQueue"/> call.
        /// </summary>
        public void QueueFillCell(uint row, uint col)
        {
            if (row < 1 || col < 1 || row > _size.Item1 || col > _size.Item2)
            {
                throw new IndexOutOfRangeException("Bad Cell Index: (" + row + ", " + col + ")");
            }
            _queue_fill[GetCellIndex(row, col)] = true;
        }

        /// <summary>
        /// Queues a cell to be set to 0 during the next <see cref="RunQueue"/> call. Runs after <see cref="QueueFillCell"/> if both are queued.
        /// </summary>
        public void QueueEmptyCell(uint row, uint col)
        {
            if (row < 1 || col < 1 || row > _size.Item1 || col > _size.Item2)
            {
                throw new IndexOutOfRangeException("Bad Cell Index: (" + row + ", " + col + ")");
            }
            _queue_empty[GetCellIndex(row, col)] = true;
        }

        /// <summary>
        /// Processes all queued <see cref="QueueFillCell"/> and <see cref="QueueEmptyCell"/> operations in batch.
        /// Processes queued fill operations before empty operations.
        /// </summary>
        public void RunQueue()
        {
            _field |= new BinaryNumber(new BitArray(_queue_fill));
            _queue_fill = new bool[_queue_fill.Length];
            _field &= (~new BinaryNumber(new BitArray(_queue_empty))) &_field;
            _queue_empty = new bool[_queue_empty.Length];
        }

        /// <summary>
        /// Performs a bitwise OR operation with other fields.
        /// </summary>
        public void CombineFields(IEnumerable<BitField> other_fields)
        {
            foreach (var o_field in other_fields)
            {
                _field |= o_field._field;
            }
        }

        /// <summary>
        /// Subtracts the bits of other fields from this one.
        /// </summary>
        public void DifferenceFields(IEnumerable<BitField> other_fields)
        {
            _field |= other_fields.Select(g=>g._field).Aggregate((g1, g2)=>g1 | g2);
            _field &= (~other_fields.Select(g=>g._field).Aggregate((g1, g2)=>g1 | g2)) &_field;
        }

        /// <summary>
        /// Updates a bit in the field without performing boundary checks.
        /// </summary>
        /// <param name="row">Row (y) index.</param>
        /// <param name="col">Column (x) index.</param>
        /// <param name="val">Bit value (0 or 1).</param>
        private void SetCellInternal(uint row, uint col, uint val)
        {
            if (val == 1){
                _field.SetBit((int) GetCellIndex(row, col), true);
            }
            else
            {
                _field.SetBit((int) GetCellIndex(row, col), false);
            }
        }

        /// <summary>
        /// Updates a horizontal segment of the field using bitwise shifts and masks for performance.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The right-most column index of the segment.</param>
        /// <param name="val">Bit value (0 or 1).</param>
        /// <param name="length">The number of bits to set.</param>
        private void SetRowInternal(uint row, uint col, uint val, uint length)
        {        
            if (val == 1){
                _field |= new BinaryNumber((int) length, 1) <<  (int) (GetCellIndex(row, col)+1-length);
            }
            else
            {
                _field &= ~((new BinaryNumber((int) length, 1) <<  (int) (GetCellIndex(row, col)+1-length))&_field);
            }
        }

        /// <summary>
        /// Gets the value of a specific cell.
        /// </summary>
        /// <param name="row">1-indexed row (y).</param>
        /// <param name="col">1-indexed column (x).</param>
        public uint GetCell(uint row, uint col)
        {
            if (row < 1 || col < 1 || row > _size.Item1 || col > _size.Item2)
            {
                throw new IndexOutOfRangeException("Bad Cell Index: (" + row + ", " + col + ")");
            }

            return GetCellInternal(row, col);
        }

        /// <summary>
        /// Gets the value of a specific cell.
        /// </summary>
        /// <param name="coords">2D vector coordinates</param>
        public uint GetCell(Vector2 coords)
        {
            return GetCell((uint) coords.X, (uint) coords.Y);
        }

        /// <summary>
        /// Retrieves a bit value from the field without performing boundary checks.
        /// </summary>
        /// <param name="row">Row (y) index.</param>
        /// <param name="col">Column (x) index.</param>
        /// <returns>The bit value (0 or 1).</returns>
        private uint GetCellInternal(uint row, uint col)
        {
            return _field.GetBit((int) GetCellIndex(row, col));
        }

        /// <summary>
        /// Inserts a single bit at the specified index and shifts subsequent bits right.
        /// </summary>
        /// <param name="row">Target row (y).</param>
        /// <param name="col">Target column (x).</param>
        private void InsertEmptyCellInternal(uint row, uint col)
        {
            int cell_index = (int) GetCellIndex(row, col);
            BinaryNumber first_half = _field & new BinaryNumber(cell_index, 1);
            BinaryNumber second_half = (_field << 1) & (new BinaryNumber(_field.Count - cell_index, 1) << (cell_index+1));
            _field = first_half | second_half;
        }

        /// <summary>
        /// Inserts a block of bits (representing a row) and shifts subsequent bits.
        /// </summary>
        /// <param name="row">Target row (y).</param>
        /// <param name="col">Start column (x).</param>
        /// <param name="length">Number of bits to insert.</param>
        private void InsertEmptyRowInternal(uint row, uint col, uint length)
        {
            int cell_index = (int) GetCellIndex(row, col);
            BinaryNumber first_half = _field & new BinaryNumber(cell_index, 1);
            BinaryNumber second_half = (_field << (int) length+1) & (new BinaryNumber(_field.Count - cell_index, 1) << (int) (cell_index+length+1));
            _field = first_half | second_half;
        }

        /// <summary>
        /// Removes a single bit at the specified index and shifts subsequent bits left.
        /// </summary>
        /// <param name="row">Target row (y).</param>
        /// <param name="col">Target column (x).</param>
        private void DeleteCellInternal(uint row, uint col)
        {
            int cell_index = (int) GetCellIndex(row, col);
            BinaryNumber first_half = _field & new BinaryNumber(cell_index, 1);
            BinaryNumber second_half = (_field >> 1) & (new BinaryNumber(_field.Count - cell_index, 1) << cell_index);
            _field = first_half | second_half;
        }

        /// <summary>
        /// Removes a block of bits (representing a row) and shifts subsequent bits.
        /// </summary>
        /// <param name="row">Target row (y).</param>
        /// <param name="col">Start column (x).</param>
        /// <param name="length">Number of bits to remove.</param>
        private void DeleteEmptyRowInternal(uint row, uint col, uint length)
        {
            int cell_index = (int) GetCellIndex(row, col);
            BinaryNumber first_half = _field & new BinaryNumber(cell_index, 1);
            BinaryNumber second_half = (_field >> (int) length+1) & (new BinaryNumber(_field.Count - cell_index, 1) << (int) (cell_index-length+1));
            _field = first_half | second_half;
        }

        /// <summary>
        /// Returns a bit-mask representing the cell and its 8 neighbors.
        /// The bits are packed based on relative position.
        /// <param name="row">Row (y) index.</param>
        /// <param name="col">Column (x) index.</param>
        /// </summary>
        public uint GetCellNeighbors(uint row, uint col)
        {
            uint neighbors = 0;
            neighbors = neighbors << 1 | GetCellInternal(row, col);
            neighbors = neighbors << 1 | GetCellInternal(row, col+1);
            neighbors = neighbors << 1 | GetCellInternal(row-1, col+1);
            neighbors = neighbors << 1 | GetCellInternal(row-1, col);
            neighbors = neighbors << 1 | GetCellInternal(row-1, col-1);
            neighbors = neighbors << 1 | GetCellInternal(row, col-1);
            neighbors = neighbors << 1 | GetCellInternal(row+1, col-1);
            neighbors = neighbors << 1 | GetCellInternal(row+1, col);
            neighbors = neighbors << 1 | GetCellInternal(row+1, col+1);
            return neighbors;
        }

        /// <summary>
        /// Returns the count of set bits (1s) in the 3x3 neighborhood, including the center cell.
        /// <param name="row">Row (y) index.</param>
        /// <param name="col">Column (x) index.</param>
        /// </summary>
        public uint GetAllSetCellNeighbors(uint row, uint col)
        {
            return (uint) BitCount(GetCellNeighbors(row, col));
        }

        /// <summary>
        /// Returns the count of set bits (1s) in the von Neumann neighborhood (self + 4 adjacent).
        /// <param name="row">Row (y) index.</param>
        /// <param name="col">Column (x) index.</param>
        /// </summary>
        public uint GetAllSetCartesianNeighbors(uint row, uint col)
        {
            uint neighbors = 0;
            neighbors += GetCellInternal(row, col);
            neighbors += GetCellInternal(row, col+1);
            neighbors += GetCellInternal(row-1, col);
            neighbors += GetCellInternal(row, col-1);
            neighbors += GetCellInternal(row+1, col);
            return neighbors;
        }

        /// <summary>
        /// Validates that the provided coordinates are within bounds, including not within the borders.
        /// </summary>
        /// <param name="row">Row (y) index.</param>
        /// <param name="col">Column (x) index.</param>
        private void CheckIndexValidity(uint row, uint col)
        {
            if (row < 1 || col < 1 || row > _size.Item1 || col > _size.Item2)
            {
                throw new IndexOutOfRangeException("Bad Cell Index: (" + row + ", " + col + ")");
            }
        }

        /// <summary>
        /// Ensures that the field size remains valid after a mutation.
        /// </summary>
        /// <param name="size_change_x">Change in row count.</param>
        /// <param name="size_change_y">Change in column count.</param>
        private void ValidateNewSize(int size_change_x, int size_change_y)
        {
            if (_size.Item1 + size_change_x <= 0 || _size.Item2 + size_change_y <= 0)
            {
                throw new IndexOutOfRangeException("Graph cannot be smaller than 1 row or column.");
            }
        }

        /// <summary>
        /// Inserts a new empty row at the specified index, shifting existing rows.
        /// </summary>
        /// <param name="index">The row (y) index where the row should be inserted.</param>
        public void InsertRow(uint index)
        {
            CheckIndexValidity(index, 1);
            InsertEmptyRowInternal(index, 0, _size.Item2+1);
            _size.Item1++;
            ChangeBorders(_border_num);
        }

        /// <summary>
        /// Inserts a new empty column at the specified index, shifting existing columns.
        /// </summary>
        /// <param name="index">The column (x) index where the column should be inserted.</param>
        public void InsertCol(uint index)
        {
            CheckIndexValidity(1, index);
            for (int row = (int) _size.Item1 + 1; row >= 0; row--)
            {
                InsertEmptyCellInternal((uint) row, index);
            }
            _size.Item2++;
            ChangeBorders(_border_num);
        }

        /// <summary>
        /// Deletes the row at the specified y index.
        /// </summary>
        /// <param name="index">row (y) index.</param>
        public void DeleteRow(uint index)
        {
            CheckIndexValidity(index, 1);
            ValidateNewSize(-1, 0);
            DeleteEmptyRowInternal(index, 0,  _size.Item2+1);
            _size.Item1--;
        }

        /// <summary>
        /// Deletes the column at the specified index.
        /// </summary>
        /// <param name="index">column (x) index.</param>
        public void DeleteCol(uint index)
        {
            CheckIndexValidity(1, index);
            ValidateNewSize(0, -1);
            for (int row = (int) _size.Item1+1 ; row >= 0; row--)
            {
                DeleteCellInternal((uint) row, index);
            }
            _size.Item2--;
        }

        /// <summary>
        /// Sets a horizontal or vertical slice of cells to a specific value.
        /// </summary>
        /// <param name="row1">row (y) index of slice start</param>
        /// <param name="col1">column (x) index of slice start</param>
        /// <param name="row2">row (y) index of slice end</param>
        /// <param name="col2">column (x) index of slice end</param>
        /// <param name="val">bit value to set entire slice to</param>
        public void SetSlice(uint row1, uint col1, uint row2, uint col2, uint val)
        {
            if (val != 0 && val != 1)
            {
                throw new ArgumentException("Value must be 0 or 1, not: " + val);
            } 
            CheckIndexValidity(row1, col1);
            CheckIndexValidity(row2, col2);

            if (row1 == row2)
            {
                uint min_col = Math.Min(col1, col2);
                uint max_col = Math.Max(col1, col2);

                SetRowInternal(row1, max_col, val, max_col - min_col + 1);

            }
            else if (col1 == col2)
            {
                uint min_row = Math.Min(row1, row2);
                uint max_row = Math.Max(row1, row2);

                for (uint row = min_row; row <= max_row; row++)
                {
                    SetCell(row, col1, val);
                } 
            }
            else
            {
                throw new IndexOutOfRangeException("Slices must be horizontal or vertical");
            }
        }

        /// <summary>
        /// Returns the bit values of a horizontal / vertical slice packed into a ulong.
        /// </summary>
        /// <param name="row1">row (y) index of slice start</param>
        /// <param name="col1">column (x) index of slice start</param>
        /// <param name="row2">row (y) index of slice end</param>
        /// <param name="col2">column (x) index of slice end</param>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public ulong GetSlice(uint row1, uint col1, uint row2, uint col2)
        {
            ulong result = 0;
            if (row1 == row2)
            {
                uint min_col = Math.Min(col1, col2);
                uint max_col = Math.Max(col1, col2);

                for (uint col = min_col; col <= max_col; col++)
                {
                    if (col == min_col)
                    {
                        result = GetCell(row1, min_col);
                    }
                    else
                    {
                        result = (result << 1) | GetCell(row1, col);
                    }
                }
            }
            else if (col1 == col2)
            {
                uint min_row = Math.Min(row1, row2);
                uint max_row = Math.Max(row1, row2);

                for (uint row = min_row; row <= max_row; row++)
                {
                    if (row == min_row)
                    {
                        result = GetCell( row, col1);
                    }
                    else
                    {
                        result = (result << 1) | GetCell(row, col1);
                    }
                } 
            }
            else
            {
                throw new IndexOutOfRangeException("Slices must be horizontal or vertical");
            }
            return result;
        }

        /// <summary>
        /// Returns 1 if any cell in the horizontal / vertical slice is 1, otherwise 0.
        /// </summary>
        /// <param name="row1">row (y) index of slice start</param>
        /// <param name="col1">column (x) index of slice start</param>
        /// <param name="row2">row (y) index of slice end</param>
        /// <param name="col2">column (x) index of slice end</param>
        public uint GetSliceOR(uint row1, uint col1, uint row2, uint col2)
        {
            CheckIndexValidity(row1, col1);
            CheckIndexValidity(row2, col2);
            uint result = 0;
            if (row1 == row2)
            {
                uint min_col = Math.Min(col1, col2);
                uint max_col = Math.Max(col1, col2);

                for (uint col = min_col; col <= max_col; col++)
                {
                    result |= GetCell(row1, col);
                    if (result > 0)
                    {
                        break;
                    }
                }
            }
            else if (col1 == col2)
            {
                uint min_row = Math.Min(row1, row2);
                uint max_row = Math.Max(row1, row2);

                for (uint row =  min_row; row <= max_row; row++)
                {
                    result |=  GetCell(row, col1);
                    if (result > 0)
                    {
                        break;
                    }
                } 
            }
            else
            {
                throw new IndexOutOfRangeException("Slices must be horizontal or vertical");
            }
            return result;
        }

        /// <summary>
        /// Returns 1 if all cells in the horizontal / vertical slice are 1, otherwise 0.
        /// </summary>
        /// <param name="row1">row (y) index of slice start</param>
        /// <param name="col1">column (x) index of slice start</param>
        /// <param name="row2">row (y) index of slice end</param>
        /// <param name="col2">column (x) index of slice end</param>
        public uint GetSliceAND(uint row1, uint col1, uint row2, uint col2)
        {
            CheckIndexValidity(row1, col1);
            CheckIndexValidity(row2, col2);
            uint result = 1;
            if (row1 == row2)
            {
                uint min_col = Math.Min(col1, col2);
                uint max_col = Math.Max(col1, col2);

                for (uint col = min_col; col <= max_col; col++)
                {
                    result &= GetCell(row1, col);
                    if (result == 0)
                    {
                        break;
                    }
                }
            }
            else if (col1 == col2)
            {
                uint min_row = Math.Min(row1, row2);
                uint max_row = Math.Max(row1, row2);

                for (uint row =  min_row; row <= max_row; row++)
                {
                    result &=  GetCell(row, col1);
                    if (result == 0)
                    {
                        break;
                    }
                } 
            }
            else
            {
                throw new IndexOutOfRangeException("Slices must be horizontal or vertical");
            }
            return result;
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is BitField)
            {
                return ((BitField) obj)._field.Equals(_field);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _field.GetHashCode();
        }

        /// <summary>
        /// Determines if the given coordinates are within the field bounds (excluding borders).
        /// </summary>
        /// <param name="row">row (y) index</param>
        /// <param name="col">column (x) index</param>
        public bool InBounds(uint row, uint col)
        {
            if (row < 1 || col < 1 || row > _size.Item1 || col > _size.Item2)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if the given coordinates are within the field bounds (excluding borders).
        /// </summary>
        /// <param name="coords">2D vector coordinates</param>
        public bool InBounds(Vector2 coords)
        {
            if (coords.X < 0 || coords.Y < 0)
            {
                return false;
            }
            return InBounds((uint) coords.X, (uint) coords.Y);
        }

        #if DEBUG
        #pragma warning disable CA1416 // Validate platform compatibility
        /// <summary>
        /// Debug helper to export the current field state as a BMP image.
        /// </summary>
        /// <param name="name">Filename prefix.</param>
        /// <param name="color">Hex color for set bits. Defaults to black</param>
        /// <param name="overlay">Whether to overlay on an existing BMP of the same name.</param>
        public void ToBMP(string name = "", string color = "0xFF000000", bool overlay = false)
        {
            Image image;
            Graphics graphic;
            if (!overlay)
            {

                image = new Bitmap((int) (30*_size.Item2), (int) (30*_size.Item1));
                graphic = Graphics.FromImage(image);
            }
            else
            {
                File.Copy($"../../{name}Field.bmp", $"../../__temp__{name}Field.bmp");
                image = Image.FromFile($"../../__temp__{name}Field.bmp");
                graphic = Graphics.FromImage(image);
            }
        
            using var brush = new SolidBrush(Color.FromArgb(Convert.ToInt32(color, 16)));

            if (!overlay)
            {
                graphic.Clear(Color.White);
            }
        
            for(int row = 1; row <= _size.Item1; row++)
            {
                for(int col = 1; col <= _size.Item2; col++)
                {
                    if (GetCell((uint) row, (uint) col) == 1)
                    {
                        graphic.FillRectangle(brush, new RectangleF(30*(col-1), 30*(row-1), 30f, 30f));
                    }
                }
            }
            if (overlay)
            {
                File.Delete($"../../{name}Field.bmp");
            }
            image.Save($"../../{name}Field.bmp");
            graphic.Dispose();
            image.Dispose();
            if (overlay)
            {
                File.Delete($"../../__temp__{name}Field.bmp");
            }     
        }
        #endif
    }
    

    /// <summary>
    /// Internal wrapper for BitArray to provide bitwise operator support.
    /// </summary>
    internal struct BinaryNumber
        {
            /// <summary>
            /// The underlying BitArray storing the binary data.
            /// </summary>
            private BitArray _backing_array;

            /// <summary>
            /// Initializes a new instance from a 64-bit unsigned integer.
            /// </summary>
            public BinaryNumber(ulong value)
            {
                _backing_array = ToBitArray(value);
            } 

            /// <summary>
            /// Initializes a new instance as a deep copy of an existing BinaryNumber.
            /// </summary>
            public BinaryNumber(BinaryNumber old_number)
            {
                _backing_array = new BitArray(old_number._backing_array);
            }

            /// <summary>
            /// Represents a BinaryNumber with a value of 0.
            /// </summary>
            public static BinaryNumber ZERO{get;} = new BinaryNumber(0);

            /// <summary>
            /// Represents a BinaryNumber with a value of 1.
            /// </summary>
            public static BinaryNumber ONE{get;} = new BinaryNumber(1);

            /// <summary>
            /// Gets the total number of bits in this binary number.
            /// </summary>
            public int Count{get {return _backing_array.Count;}}

            /// <summary>
            /// Initializes a new instance with a specific size and all bits set to a default bit value.
            /// </summary>
            /// <param name="bit_number">The size of the BitArray</param>
            /// <param name="default_value">The default bit value to start with (0 or 1)</param>
            public BinaryNumber(int bit_number, int default_value)
            {
                if (default_value != 1 && default_value != 0)
                {
                    throw new ArgumentException("number must be 0 or 1");
                }
                _backing_array = new BitArray(bit_number, default_value == 1);
            }

            /// <summary>
            /// Initializes a new instance from an existing BitArray.
            /// </summary>
            public BinaryNumber(BitArray bitArray)
            {
                _backing_array = new BitArray(bitArray);
            }

            /// <summary>
            /// Shifts the bits of the binary number to the left by the specified amount.
            /// The length of the underlying array is increased by the shift amount.
            /// </summary>
            /// <param name="n">The binary number to shift.</param>
            /// <param name="shift_num">The number of bits to shift.</param>
            /// <returns>A new BinaryNumber with shifted bits and an increased length.</returns>
            public static BinaryNumber operator <<(BinaryNumber n, int shift_num)
            {
                BinaryNumber copy = new BinaryNumber(n._backing_array);
                copy._backing_array.Length += shift_num;
                copy._backing_array.LeftShift(shift_num);
                return copy;
            }

            /// <summary>
            /// Shifts the bits of the binary number to the right by the specified amount.
            /// The length of the underlying array is decreased by the shift amount.
            /// </summary>
            /// <param name="n">The binary number to shift.</param>
            /// <param name="shift_num">The number of bits to shift.</param>
            /// <returns>A new BinaryNumber with shifted bits and a decreased length.</returns>
            public static BinaryNumber operator >>(BinaryNumber n, int shift_num)
            {
                BinaryNumber copy = new BinaryNumber(n._backing_array);
                copy._backing_array.RightShift(shift_num);
                copy._backing_array.Length -= shift_num;
                return copy;
            }

            /// <summary>
            /// Performs a bitwise OR operation between two binary numbers.
            /// The resulting binary number will have the length of the larger operand.
            /// </summary>
            /// <param name="n">The first binary number.</param>
            /// <param name="n2">The second binary number.</param>
            /// <returns>A new BinaryNumber representing the bitwise OR result.</returns>
            public static BinaryNumber operator |(BinaryNumber n, BinaryNumber n2)
            {
                if (n._backing_array.Count == n2._backing_array.Count){
                    BinaryNumber copy = new BinaryNumber(n._backing_array);
                    copy._backing_array.Or(n2._backing_array);
                    return copy;
                }
                else if (n._backing_array.Count > n2._backing_array.Count)
                {
                    BinaryNumber copy = new BinaryNumber(n2._backing_array);
                    copy._backing_array.Length += n._backing_array.Count - n2._backing_array.Count;
                    copy._backing_array.Or(n._backing_array);
                    return copy;
                }
                else
                {
                    BinaryNumber copy = new BinaryNumber(n._backing_array);
                    copy._backing_array.Length += n2._backing_array.Count - n._backing_array.Count;
                    copy._backing_array.Or(n2._backing_array);
                    return copy;
                }
            }
        
            /// <summary>
            /// Performs a bitwise AND operation. The result will have the length of the larger operand.
            /// </summary>
            /// <param name="n">The first binary number.</param>
            /// <param name="n2">The second binary number.</param>
            /// <returns>A new BinaryNumber representing the bitwise AND result.</returns>
            public static BinaryNumber operator &(BinaryNumber n, BinaryNumber n2)
            {
                if (n._backing_array.Count == n2._backing_array.Count){
                    BinaryNumber copy = new BinaryNumber(n._backing_array);
                    copy._backing_array.And(n2._backing_array);
                    return copy;
                }
                else if (n._backing_array.Count > n2._backing_array.Count)
                {
                    BinaryNumber copy = new BinaryNumber(n2._backing_array);
                    copy._backing_array.Length += n._backing_array.Count - n2._backing_array.Count;
                    copy._backing_array.And(n._backing_array);
                    return copy;
                }
                else
                {
                    BinaryNumber copy = new BinaryNumber(n._backing_array);
                    copy._backing_array.Length += n2._backing_array.Count - n._backing_array.Count;
                    copy._backing_array.And(n2._backing_array);
                    return copy;
                }
            }

            /// <summary>
            /// Performs a bitwise NOT operation (complement) on the binary number.
            /// </summary>
            /// <param name="n">The binary number to negate.</param>
            /// <returns>A new BinaryNumber with all bits flipped.</returns>
            public static BinaryNumber operator~(BinaryNumber n)
            {
                BinaryNumber copy = new BinaryNumber(n._backing_array);
                copy._backing_array.Not();
                return copy;
            }

            /// <summary>
            /// Sets the bit at the specified index.
            /// </summary>
            /// <param name="index">The 0-based index.</param>
            /// <param name="value">The boolean (True/1 vs False/0) state to set.</param>
            public void SetBit(int index, bool value)
            {
                _backing_array[index] = value;
            }

            /// <summary>
            /// Gets the bit value at the specified index.
            /// </summary>
            /// <param name="index">The 0-based index.</param>
            /// <returns>1 if true, 0 if false.</returns>
            public uint GetBit(int index)
            {
                if (_backing_array[index])
                {
                    return 1u;
                }
                else
                {
                    return 0u;
                }
            }

            /// <summary>
            /// Converts a ulong into a BitArray using its byte representation.
            /// </summary>
            /// <param name="num_obj">The ulong value.</param>
            private static BitArray ToBitArray(object num_obj)
            {
                ulong num = (ulong) num_obj;
                BitArray temp_array = new BitArray(BitConverter.GetBytes(num));
                return temp_array;
            }

            /// <summary>
            /// Attempts to convert the binary data into a 32-bit unsigned integer.
            /// </summary>
            public uint ToUint()
            {
                byte[] temp_array = new byte[_backing_array.Count];
                _backing_array.CopyTo(temp_array, 0);
                try
                {
                    return BitConverter.ToUInt32(temp_array);
                }
                catch (ArgumentOutOfRangeException)
                {
                    try
                    {
                        return BitConverter.ToUInt16(temp_array);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        try
                        {
                            return (uint) BitConverter.ToSingle(temp_array);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            return (uint) (BitConverter.ToBoolean(temp_array) ? 1 : 0);
                        }
                    }
                }

            }

            /// <summary>
            /// Converts the binary data into a 64-bit unsigned integer.
            /// </summary>
            public ulong ToULong()
            {
                byte[] temp_array = new byte[_backing_array.Count];
                _backing_array.CopyTo(temp_array, 0);
                return BitConverter.ToUInt64(temp_array);
            }

            /// <summary>
            /// Converts the binary data into a BigInteger.
            /// </summary>
            public BigInteger ToBigInteger()
            {
                byte[] temp_array = new byte[_backing_array.Count];
                _backing_array.CopyTo(temp_array, 0);
                return new BigInteger(temp_array);
            }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is BinaryNumber)
            {
                try
                {
                    return ((BinaryNumber)obj).ToULong() == ToULong();
                }
                catch (Exception)
                {
                    return ((BinaryNumber)obj).ToBigInteger() == ToBigInteger();
                }
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            try
                {
                    return ToULong().GetHashCode();
                }
            catch (Exception)
                {
                    return ToBigInteger().GetHashCode();
                }
        }
    }
}
