#if !UNITY_EDITOR
namespace BitField
{
    [TestClass]
    public class BitFieldTests
    {
        [TestMethod]
        public void BitFieldConstructor_0Constructor_Valid()
        {
            _ = new BitField(5, 5);
        }

        [TestMethod]
        public void BitFieldConstructor_DefaultConstructor_Valid()
        {
            _ = new BitField(5, 5, 1);
        }

        [TestMethod]
        public void BitFieldConstructor_2Border_Invalid()
        {
            Assert.Throws<ArgumentException>(() => new BitField(5, 5, 2));
        }

        [TestMethod]
        public void BitFieldConstructor_NoSize_Invalid()
        {
            Assert.Throws<Exception>(() => new BitField(0,0));
        }

        [TestMethod]
        public void BitFieldConstructor_100x100_ValidFullSize()
        {
            _ = new BitField(100, 100);
        }

        [TestMethod]
        public void BitFieldConstructor_0Rows_Invalid()
        {
            Assert.Throws<Exception>(() => new BitField(0,5));
        }

        [TestMethod]
        public void BitFieldConstructor_0Cols_Invalid()
        {
            Assert.Throws<Exception>(() => new BitField(5,0));
        }

        [TestMethod]
        public void BitFieldChangeBorders_1SameOnEmpty_ValidNoChange()
        {
            BitField field = new BitField(5, 5);
            field.ChangeBorders(1);
        }

        [TestMethod]
        public void BitFieldChangeBorders_0BorderOnEmpty_ValidAllZeroes()
        {
            BitField field = new BitField(5, 5);
            field.ChangeBorders(0);
        }

        [TestMethod]
        public void BitFieldChangeBorders_2Border_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<ArgumentException>(() => field.ChangeBorders(2));
        }

        [TestMethod]
        public void BitFieldChangeBorders_1SameFullGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field.SetSlice(1, 1, 1, 5, 1);
            field.SetSlice(2, 1, 2, 5, 1);
            field.SetSlice(3, 1, 3, 5, 1);
            field.SetSlice(4, 1, 4, 5, 1);
            field.SetSlice(5, 1, 5, 5, 1);
            field.ChangeBorders(1);
            BitField testField = new BitField(5, 5);
            testField._field = ToBinaryNumber(0b1_111_111_111_111_111_111_111_111_111_111_111_111_111_111_111_111);
            Assert.AreEqual(testField, field, $"test field: {BinaryNumberToLong(testField._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");;
        }

        [TestMethod]
        public void BitFieldChangeBorders_0FullGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field.SetSlice(1, 1, 1, 5, 1);
            field.SetSlice(2, 1, 2, 5, 1);
            field.SetSlice(3, 1, 3, 5, 1);
            field.SetSlice(4, 1, 4, 5, 1);
            field.SetSlice(5, 1, 5, 5, 1);
            field.ChangeBorders(0);
            BitField testField = new BitField(5, 5);
            testField._field = ToBinaryNumber(0b0_000_000_011_111_001_111_100_111_110_011_111_001_111_100_000_000);
            Assert.AreEqual(testField, field, $"test field: {BinaryNumberToLong(testField._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");;
        }

        [TestMethod]
        public void BitFieldChangeBorders_2FullGraph_Invalid()
        {
            BitField field = new BitField(5, 5);
            field.SetSlice(1, 1, 1, 5, 1);
            field.SetSlice(2, 1, 2, 5, 1);
            field.SetSlice(3, 1, 3, 5, 1);
            field.SetSlice(4, 1, 4, 5, 1);
            field.SetSlice(5, 1, 5, 5, 1);
            Assert.Throws<ArgumentException>(()=>field.ChangeBorders(2));
        }

        [TestMethod]
        public void BitFieldGetSetCell_0MiddleCellEmptyGraph_0Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000101_1000001_1111111);
            field.SetCell(2, 2, 0);
            Assert.AreEqual<uint>(0, field.GetCell(2, 2));
        }

        [TestMethod]
        public void BitFieldGetSetCell_1MiddleCellEmptyGraph_1Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.SetCell(2, 2, 1);
            Assert.AreEqual<uint>(1, field.GetCell(2, 2));
        }

        [TestMethod]
        public void BitFieldSetCell_2MiddleCellEmptyGraph_Invalid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000101_1111111);
            Assert.Throws<ArgumentException>(()=>field.SetCell(2, 2, 2));
        }

        [TestMethod]
        public void BitFieldGetSetCell_0MiddleCellFullGraph_0Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.SetCell(2, 2, 0);
            Assert.AreEqual<uint>(0, field.GetCell(2, 2));
        }

        [TestMethod]
        public void BitFieldGetSetCell_1MiddleCellFullGraph_1Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.SetCell(2, 2, 1);
            Assert.AreEqual<uint>(1, field.GetCell(2, 2));
        }

        [TestMethod]
        public void BitFieldSetCell_2MiddleCellFullGraph_Invalid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            Assert.Throws<ArgumentException>(()=>field.SetCell(2, 2, 2));
        }

        [TestMethod]
        public void BitFieldGetSetCell_0CornerCellEmptyGraph_0Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000011_1111111);
            field.SetCell(1, 1, 0);
            Assert.AreEqual<uint>(0, field.GetCell(1, 1));
        }

        [TestMethod]
        public void BitFieldGetSetCell_1CornerCellEmptyGraph_1Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.SetCell(1, 1, 1);
            Assert.AreEqual<uint>(1, field.GetCell(1, 1));
        }

        [TestMethod]
        public void BitFieldSetCell_2CornerCellEmptyGraph_Invalid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.Throws<ArgumentException>(()=>field.SetCell(1, 1, 2));
        }

        [TestMethod]
        public void BitFieldGetCellNeighbors_All0NeighborsMiddle_0b00000000()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual<uint>(0b000_000_000, field.GetCellNeighbors(2, 2));
        }

        [TestMethod]
        public void BitFieldGetCellNeighbors_EachDirectionAs1Middle_All0sExcept1Binary()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000101_1000001_1111111);
            Assert.AreEqual<uint>(0b100_000_000, field.GetCellNeighbors(2, 2), "self");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1001001_1000001_1111111);
            Assert.AreEqual<uint>(0b010_000_000, field.GetCellNeighbors(2, 2), "right");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1001001_1111111);
            Assert.AreEqual<uint>(0b001_000_000, field.GetCellNeighbors(2, 2), "upper right");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000101_1111111);
            Assert.AreEqual<uint>(0b000_100_000, field.GetCellNeighbors(2, 2), "up");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000011_1111111);
            Assert.AreEqual<uint>(0b000_010_000, field.GetCellNeighbors(2, 2), "upper left");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000011_1000001_1111111);
            Assert.AreEqual<uint>(0b000_001_000, field.GetCellNeighbors(2, 2), "left");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000011_1000001_1000001_1111111);
            Assert.AreEqual<uint>(0b000_000_100, field.GetCellNeighbors(2, 2), "lower left");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000101_1000001_1000000_1111111);
            Assert.AreEqual<uint>(0b000_000_010, field.GetCellNeighbors(2, 2), "down");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1001001_1000001_1000001_1111111);
            Assert.AreEqual<uint>(0b000_000_001, field.GetCellNeighbors(2, 2), "lower right");
        }

        [TestMethod]
        public void BitFieldGetCellNeighbors_All1NeighborsMiddle_0b111111111()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1001111_1001111_1001111_1111111);
            Assert.AreEqual<uint>(0b111_111_111, field.GetCellNeighbors(2, 2));
        }

        [TestMethod]
        public void BitFieldGetCellNeighbors_All0NeighborsCorners_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual<uint>(0b001_111_100, field.GetCellNeighbors(1, 1), "upper left corner");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual<uint>(0b011_110_001, field.GetCellNeighbors(1, 5), "upper right corner");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual<uint>(0b000_011_111, field.GetCellNeighbors(5, 1), "lower left corner");

            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual<uint>(0b011_000_111, field.GetCellNeighbors(5, 5), "lower right corner");
        }

        [TestMethod]
        public void BitFieldInsertRow_NormalSizeEmptyGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.InsertRow(2);
            BitField test_field = new BitField(6, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertRow_NormalSizeAlternatingGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1010101_1111111);
            field.InsertRow(2);
            BitField test_field = new BitField(6, 5);
            test_field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1000001_1010101_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertRow_NormalSizeFullGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.InsertRow(2);
            BitField test_field = new BitField(6, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1000001_1111111_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertRow_NormalSizeEmptyGraphSides_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.InsertRow(1);
            BitField test_field = new BitField(6, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertRow_NormalSizeFullGraphSides_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.InsertRow(1);
            BitField test_field = new BitField(6, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertRow_NormalSizeBadIndexes_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(0));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(6));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(7));
        } 

        [TestMethod]
        public void BitFieldInsertRow_1WideField_Valid()
        {
            BitField field = new BitField(5, 1);
            field.InsertRow(1);
            BitField test_field = new BitField(6, 1);
            test_field._field = ToBinaryNumber(0b111_101_101_101_101_101_101_111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        } 

        [TestMethod]
        public void BitFieldInsertRow_1HighField_Valid()
        {
            BitField field = new BitField(1, 5);
            field.InsertRow(1);
            BitField test_field = new BitField(2, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        } 

        [TestMethod]
        public void BitFieldInsertRow_1WideField_Invalid()
        {
            BitField field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(6));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(0));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(7));
        } 

        [TestMethod]
        public void BitFieldInsertRow_1HighField_Invalid()
        {
            BitField field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(0));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(2));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertRow(3));
        } 


        [TestMethod]
        public void BitFieldInsertCol_NormalSizeEmptyGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.InsertCol(2);
            BitField test_field = new BitField(5, 6);
            test_field._field = ToBinaryNumber(0b11111111_10000001_10000001_10000001_10000001_10000001_11111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertCol_NormalSizeAlternatingGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1010101_1111111);
            field.InsertCol(2);
            BitField test_field = new BitField(5, 6);
            test_field._field = ToBinaryNumber(0b11111111_10101001_10101001_10101001_10101001_10101001_11111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }


        [TestMethod]
        public void BitFieldInsertCol_NormalSizeFullGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.InsertCol(2);
            BitField test_field = new BitField(5, 6);
            test_field._field = ToBinaryNumber(0b11111111_11111011_11111011_11111011_11111011_11111011_11111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertCol_NormalSizeEmptyGraphSides_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.InsertCol(1);
            BitField test_field = new BitField(5, 6);
            test_field._field = ToBinaryNumber(0b11111111_10000001_10000001_10000001_10000001_10000001_11111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertCol_NormalSizeFullGraphSides_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.InsertCol(1);
            BitField test_field = new BitField(5, 6);
            test_field._field = ToBinaryNumber(0b11111111_11111101_11111101_11111101_11111101_11111101_11111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldInsertCol_NormalSizeBadIndexes_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(0));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(6));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(7));
        }

        [TestMethod]
        public void BitFieldInsertCol_1WideField_Valid()
        {
            BitField field = new BitField(5, 1);
            field.InsertCol(1);
            BitField test_field = new BitField(5, 2);
            test_field._field = ToBinaryNumber(0b1111_1001_1001_1001_1001_1001_1111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        } 

        [TestMethod]
        public void BitFieldInsertCol_1HighField_Valid()
        {
            BitField field = new BitField(1, 5);
            field.InsertCol(1);
            BitField test_field = new BitField(1, 6);
            test_field._field = ToBinaryNumber(0b11111111_10000001_11111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        } 

        [TestMethod]
        public void BitFieldInsertCol_1WideField_Invalid()
        {
            BitField field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(0));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(2));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(3));
        } 

        [TestMethod]
        public void BitFieldInsertCol_1HighField_Invalid()
        {
            BitField field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(0));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(6));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.InsertCol(7));
        }  

        [TestMethod]
        public void BitFieldDeleteRow_NormalSizeEmptyGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.DeleteRow(2);
            BitField test_field = new BitField(4, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldDeleteRow_NormalSizeAlternatingGraph_Valid()
        {
            BitField field = new BitField(4, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1010101__1000001_1010101_1111111);
            field.DeleteRow(2);
            BitField test_field = new BitField(3, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1010101__1010101_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldDeleteRow_NormalSizeFullGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.DeleteRow(2);
            BitField test_field = new BitField(4, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldDeleteRow_NormalSizeGraphSides_Valid()
        {
            BitField field = new BitField(4, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1010101__1000001_1010101_1111111);
            field.DeleteRow(1);
            BitField test_field = new BitField(3, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1010101__1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldDeleteRow_NormalSizeBadIndexes_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(0));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(6));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(7));
        } 
    
        [TestMethod]
        public void BitFieldDeleteRow_1WideField_Valid()
        {
            BitField field = new BitField(5, 1);
            field.DeleteRow(1);
            BitField test_field = new BitField(4, 1);
            test_field._field = ToBinaryNumber(0b111_101_101_101_101_111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        } 

        [TestMethod]
        public void BitFieldDeleteRow_1WideField_Invalid()
        {
            BitField field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(6));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(0));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(7));
        } 

        [TestMethod]
        public void BitFieldDeleteRow_1HighField_Invalid()
        {
            BitField field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(0));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(1));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(2));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteRow(3));
        } 

        [TestMethod]
        public void BitFieldDeleteCol_NormalSizeEmptyGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.DeleteCol(2);
            BitField test_field = new BitField(5, 4);
            test_field._field = ToBinaryNumber(0b111111_100001_100001_100001_100001_100001_111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldDeleteCol_NormalSizeAlternatingGraph_Valid()
        {
            BitField field = new BitField(4, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101__1010101_1010101_1111111);
            field.DeleteCol(2);
            BitField test_field = new BitField(4, 4);
            test_field._field = ToBinaryNumber(0b111111_101001_101001__101001_101001_111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }


        [TestMethod]
        public void BitFieldDeleteCol_NormalSizeFullGraph_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.DeleteCol(2);
            BitField test_field = new BitField(5, 4);
            test_field._field = ToBinaryNumber(0b111111_111111_111111_111111_111111_111111_111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldDeleteCol_NormalSizeGraphSides_Valid()
        {
            BitField field = new BitField(4, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1010101__1000001_1010101_1111111);
            field.DeleteCol(1);
            BitField test_field = new BitField(4, 4);
            test_field._field = ToBinaryNumber(0b111111_100001_101011__100001_101011_111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }


        [TestMethod]
        public void BitFieldDeleteCol_NormalSizeBadIndexes_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(0));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(6));
            field = new BitField(5, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(7));
        } 

        [TestMethod]
        public void BitFieldDeleteCol_1HighField_Valid()
        {
            BitField field = new BitField(1, 5);
            field.DeleteCol(1);
            BitField test_field = new BitField(1, 4);
            test_field._field = ToBinaryNumber(0b111111_100001_111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        } 

        [TestMethod]
        public void BitFieldDeleteCol_1WideField_Invalid()
        {
            BitField field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(0));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(1));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(2));
            field = new BitField(5, 1);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(3));
        } 

        [TestMethod]
        public void BitFieldDeleteCol_1HighField_Invalid()
        {
            BitField field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(0));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(6));
            field = new BitField(1, 5);
            Assert.Throws<IndexOutOfRangeException>(()=>field.DeleteCol(7));
        } 

        [TestMethod]
        public void BitFieldSetSlice_0s_Valid()
        {
            BitField field = new BitField(5, 5);
            field.SetSlice(1, 1, 5, 1, 1);
            BitField test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000011_1000011_1000011_1000011_1000011_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field.SetSlice(2, 2, 4, 2, 1);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000101_1000101_1000101_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field.SetSlice(1, 1, 1, 5, 1);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1111111_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field.SetSlice(2, 2, 2, 4, 1);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1011101_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
    
            field = new BitField(5, 5);
            field.SetSlice(4, 2, 2, 2, 1);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000101_1000101_1000101_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

        }

        [TestMethod]
        public void BitFieldSetSlice_1s_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.SetSlice(1, 1, 5, 1, 0);
            BitField test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111101_1111101_1111101_1111101_1111101_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.SetSlice(2, 2, 4, 2, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111111_1111011_1111011_1111011_1111111_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.SetSlice(1, 1, 1, 5, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.SetSlice(2, 2, 2, 4, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1100011_1111111_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
    
            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.SetSlice(4, 2, 2, 2, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1111111_1111011_1111011_1111011_1111111_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

        }

        [TestMethod]
        public void BitFieldSetSlice_Mixed_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1010101_1111111);
            field.SetSlice(1, 1, 5, 1, 1);
            BitField test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1010111_1010111_1010111_1010111_1010111_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1010101_1111111);
            field.SetSlice(2, 2, 4, 2, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1010101_1010001_1010001_1010001_1010101_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1010101_1111111);
            field.SetSlice(1, 1, 1, 5, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");

            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1010101_1111111);
            field.SetSlice(2, 2, 2, 4, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1000001_1010101_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        
            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1010101_1111111);
            field.SetSlice(2, 4, 2, 2, 0);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1000001_1010101_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
        }

        [TestMethod]
        public void BitFieldSetGetSlice_DiagonalIndices_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.SetSlice(1, 1, 5, 5, 1), "Field should not accept diagonal slicing");

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSlice(1, 1, 5, 5), "Field should not accept diagonal slicing");
        }

        [TestMethod]
        public void BitFieldSetGetSlice_SameStartEnd_Valid()
        {
            BitField field = new BitField(5, 5);
            field.SetSlice(1, 1, 1, 1, 1);
            BitField test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000011_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
            Assert.AreEqual(0b1U, field.GetSlice(1, 1, 1, 1));

            field = new BitField(5, 5);
            field.SetSlice(3, 4, 3, 4, 1);
            test_field = new BitField(5, 5);
            test_field._field = ToBinaryNumber(0b1111111_1000001_1000001_1010001_1000001_1000001_1111111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
            Assert.AreEqual(0b1U, field.GetSlice(3, 4, 3, 4));
        }

        [TestMethod]
        public void BitFieldSetGetSlice_OutofBoundsIndices_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.SetSlice(0, 1, 4, 1, 1));
            Assert.Throws<Exception>(()=>field.GetSlice(0, 1, 4, 1));
            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.SetSlice(4, 0, 4, 1, 1));
            Assert.Throws<Exception>(()=>field.GetSlice(4, 0, 4, 1));
            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.SetSlice(4, 1, 0, 1, 1));
            Assert.Throws<Exception>(()=>field.GetSlice(4, 1, 0, 1));
            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.SetSlice(0, 0, 4, 0, 1));
            Assert.Throws<Exception>(()=>field.GetSlice(0, 0, 4, 0));
            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.SetSlice(6, 1, 6, 3, 1));
            Assert.Throws<Exception>(()=>field.GetSlice(6, 1, 6, 3));
            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.SetSlice(4, 7, 4, 3, 1));
            Assert.Throws<Exception>(()=>field.GetSlice(4, 7, 4, 3));
            }

        [TestMethod]
        public void BitFieldSetGetSlice_1by1Field_Valid()
        {
            BitField field = new BitField(1, 1);
            field.SetSlice(1, 1, 1, 1, 1);
            BitField test_field = new BitField(1, 1);
            test_field._field = ToBinaryNumber(0b111_111_111);
            Assert.AreEqual(test_field, field, $"test field: {BinaryNumberToLong(test_field._field).ToString("b")} field: {BinaryNumberToLong(field._field ).ToString("b")}");
            Assert.AreEqual(0b1U, field.GetSlice(1, 1, 1, 1));
        }

        [TestMethod]
        public void BitFieldGetSliceOR_No1s_Valid0()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1000001_1111111);
            Assert.AreEqual<uint>(0, field.GetSliceOR(1, 1, 1, 5));
            Assert.AreEqual<uint>(0, field.GetSliceOR(1, 1, 5, 1));
        }

        [TestMethod]
        public void BitFieldGetSliceOR_1_1s_Valid1()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1001001_1111111);
            Assert.AreEqual<uint>(1, field.GetSliceOR(1, 1, 1, 5));
            Assert.AreEqual<uint>(1, field.GetSliceOR(1, 3, 5, 3));
        }

        [TestMethod]
        public void BitFieldGetSliceOR_No0s_Valid1()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1111111_1111111);
            Assert.AreEqual<uint>(1, field.GetSliceOR(1, 1, 1, 5));
            Assert.AreEqual<uint>(1, field.GetSliceOR(1, 2, 5, 2));
        }

        [TestMethod]
        public void BitFieldGetSliceOR_Diagonal_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(1, 1, 5, 5));
            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(5, 1, 1, 5));
        }

        [TestMethod]
        public void BitFieldGetSliceOR_BadIndices_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(1, 1, 0, 1));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(1, 1, 6, 1));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(1, 1, 7, 1));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(1, 1, 1, 0));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(1, 1, 1, 6));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceOR(1, 1, 1, 7));
        }

        [TestMethod]
        public void BitFieldGetSliceOR_SameStartEnd_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1111111_1111111);
            Assert.AreEqual<uint>(1, field.GetSliceOR(1, 1, 1, 1));
            Assert.AreEqual<uint>(0, field.GetSliceOR(3, 3, 3, 3));
        }

        [TestMethod]
        public void BitFieldGetSliceOR_1by1_Valid()
        {
            BitField field = new BitField(1, 1);
            Assert.AreEqual<uint>(0, field.GetSliceOR(1, 1, 1, 1));
        }

        [TestMethod]
        public void BitFieldGetSliceAND_No1s_Valid0()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1000001_1111111);
            Assert.AreEqual<uint>(0, field.GetSliceAND(1, 1, 1, 5));
            Assert.AreEqual<uint>(0, field.GetSliceAND(1, 1, 5, 1));
        }

        [TestMethod]
        public void BitFieldGetSliceAND_1_1s_Valid0()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1001001_1111111);
            Assert.AreEqual<uint>(0, field.GetSliceAND(1, 1, 1, 5));
            Assert.AreEqual<uint>(0, field.GetSliceAND(1, 3, 5, 3));
        }

        [TestMethod]
        public void BitFieldGetSliceAND_No0s_Valid1()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1111111_1111111);
            Assert.AreEqual<uint>(1, field.GetSliceAND(1, 1, 1, 5));
            Assert.AreEqual<uint>(1, field.GetSliceAND(1, 2, 5, 2));
        }

        [TestMethod]
        public void BitFieldGetSliceAND_Diagonal_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(1, 1, 5, 5));
            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(5, 1, 1, 5));
        }

        [TestMethod]
        public void BitFieldGetSliceAND_BadIndices_Invalid()
        {
            BitField field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(1, 1, 0, 1));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(1, 1, 6, 1));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(1, 1, 7, 1));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(1, 1, 1, 0));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(1, 1, 1, 6));

            field = new BitField(5, 5);
            Assert.Throws<Exception>(()=>field.GetSliceAND(1, 1, 1, 7));
        }

        [TestMethod]
        public void BitFieldGetSliceAND_SameStartEnd_Valid()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1111111_1111111);
            Assert.AreEqual<uint>(1, field.GetSliceAND(1, 1, 1, 1));
            Assert.AreEqual<uint>(0, field.GetSliceAND(3, 3, 3, 3));
        }

        [TestMethod]
        public void BitFieldGetSliceAND_1by1_Valid()
        {
            BitField field = new BitField(1, 1);
            Assert.AreEqual<uint>(0, field.GetSliceAND(1, 1, 1, 1));
        }

        private BinaryNumber ToBinaryNumber(long num)
        {
            return new BinaryNumber((ulong) num);
        }

        private long BinaryNumberToLong(BinaryNumber binNum)
        {
            return (long) binNum.ToULong();
        }


        [TestMethod]
        public void BitFieldToBMP_ByHand()
        {
            BitField field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1001111_1001111_1001111_1111111);
            field.ToBMP();
            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1010101_1010101_1010101_1010101_1111111_1111111);
            field.ToBMP();
            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1000001_1000001_1000001_1000001_1000001_1111111);
            field.ToBMP();
            field = new BitField(5, 5);
            field._field = ToBinaryNumber(0b1111111_1111111_1111111_1111111_1111111_1111111_1111111);
            field.ToBMP();
        }
    
    }


}
#endif