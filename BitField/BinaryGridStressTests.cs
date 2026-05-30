namespace BitField
{
#pragma warning disable MSTEST0045 // Use 'CooperativeCancellation = true' with '[Timeout]'

    [TestClass]
    public class BitFieldStressTests
    {


        [Timeout(1000)]
        [TestMethod]
        public void BitFieldConstructor_200_Valid()
        {
            _ = new BitField(200, 200);
        }

        [Timeout(1000)]
        [TestMethod]
        public void BitFieldConstructor_400_Valid()
        {
            _ = new BitField(400, 400);
        }

        [Timeout(1000)]
        [TestMethod]
        public void BitFieldConstructor_800_Valid()
        {
            _ = new BitField(800, 800);
        }

        [Timeout(1000)]
        [TestMethod]
        public void BitFieldConstructor_1600_Valid()
        {
            _ = new BitField(1600, 1600);
        }

        [Timeout(1000)]
        [TestMethod]
        public void BitFieldConstructor_3200_Valid()
        {
            _ = new BitField(3200, 3200);
        }

        [Timeout(10000)]
        [TestMethod]
        public void BitFieldGetSlice_Huge_Valid()
        {
            BitField grid = new BitField(800, 800);
            for (uint i = 0; i < 1000; i++)
            {
                _ = grid.GetSlice((i%800)+1, (i%800)+1, (i%800)+1, (uint) ((i*.5)%800)+1);
            }
        }

        [Timeout(10000)]
        [TestMethod]
        public void BitFieldSetSlice_Huge_Valid()
        {
            BitField grid = new BitField(800, 800);
            for (uint i = 0; i < 1000; i++)
            {
                grid.SetSlice((i%800)+1, (i%800)+1, (i%800)+1, (uint) ((i*.25)%800)+1, 1);
            }
        }

        [Timeout(10000)]
        [TestMethod]
        public void BitFieldInsertRowCol_Huge_Valid()
        {
            BitField grid = new BitField(800, 800);
            for (uint i = 0; i < 1000; i++)
            {
                grid.InsertRow((i % 800) + 1);
                grid.InsertRow((i % 800) + 1);
            }
        }

        [Timeout(10000)]
        [TestMethod]
        public void BitFieldGetSliceORAND_Huge_Valid()
        {
            BitField grid = new BitField(800, 800);
            for (uint i = 0; i < 1000; i++)
            {
                _ = grid.GetSliceOR((i%800)+1, (i%800)+1, (i%800)+1, (uint) ((i*.5)%800)+1);
                _ = grid.GetSliceAND((i%800)+1, (i%800)+1, (i%800)+1, (uint) ((i*.5)%800)+1);
            }
        }

        [Timeout(10000)]
        [TestMethod]
        public void BitFieldDeleteRowCol_Huge_Valid()
        {
            BitField grid = new BitField(800, 800);
            for (uint i = 0; i < 799; i++)
            {
                grid.DeleteRow(800 - i);
                grid.DeleteCol(800 - i);
            }
        }
    
    }
}