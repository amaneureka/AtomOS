using System;
namespace Atomix.Graphics
{
    public static class Allocator
    {

        /// <summary>
        /// Alloc delegate.
        /// </summary>
        public unsafe delegate byte* AllocationDelegate(int size);

        /// <summary>
        /// The allocator.
        /// </summary>
        public static AllocationDelegate Alloc;

        /// <summary>
        /// Initializes the <see cref="T:Atomix.Graphics.Allocator"/> class.
        /// </summary>
        static Allocator()
        {

            // Set default allocator
            Alloc = size =>
            {
                throw new Exception("Please set Allocator.Alloc!");
            };
        }
    }
}


