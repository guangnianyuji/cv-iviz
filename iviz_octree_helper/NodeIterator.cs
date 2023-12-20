namespace Iviz.Octree
{
    internal struct NodeIterator
    {
        public readonly OcTreeKey key;
        public readonly int depth;
        int position;
        readonly sbyte bitset;

        NodeIterator(int depth, in OcTreeKey key, sbyte bitset)
        {
            this.depth = depth;
            position = -1;
            this.key = key;
            this.bitset = bitset;
        }
        
        public NodeIterator(sbyte bitset)
        {
            depth = 0;
            position = -1;
            key = new OcTreeKey(OctreeHelper.TreeMaxVal);
            this.bitset = bitset;
        }        

        public bool MoveNext()
        {
            if (position >= 7)
            {
                return false;
            }

            position++;
            return true;
        }

        public bool HasChild => (bitset & (1 << position)) != 0;
        public bool IsLeaf => bitset == 0;

        public void CreateChild(sbyte childBitset, out NodeIterator childIt)
        {
            childIt = new NodeIterator(depth + 1,
                key.ComputeChildKey(position, OctreeHelper.TreeMaxVal >> (depth + 1)),
                childBitset);
        }
    }
}