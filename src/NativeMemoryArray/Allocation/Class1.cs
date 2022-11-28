using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Cysharp.Collections.Allocation
{
    public interface INativeAllocator
    {
        IntPtr Allocate(nuint size, bool clear = false);
        void Free(IntPtr data);
    }

    public abstract class MemoryAllocator : INativeAllocator
    {
        public virtual bool CanHandleZero => false;

        public IntPtr Allocate(nuint size, bool clear = false)
        {
            if (size < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size), size, "Allocation size must be positive");
            }

            if (size == 0)
            {
                return IntPtr.Zero;
            }

            IntPtr ptr = AllocateCore(size);
            if (ptr == IntPtr.Zero)
            {
                throw new NullReferenceException();
            }

            if (clear && CanHandleZero)
            {
                unsafe
                {
                    var  chunks = 
                    for (int i = 0; i < length; i++)
                    {

                    }
                    new Span<byte>(ptr.ToPointer(), (int)size).Fill(0);
                }
            }

            return ptr;
        }



        public void Free(IntPtr data)
        {
            if (data == IntPtr.Zero)
            {
                return;
            }
            FreeCore(data);
        }

        public abstract IntPtr AllocateCore(nuint size, bool clear = false);
        public abstract void FreeCore(IntPtr data);
    }

    public sealed class HGlobalMemAllocator : MemoryAllocator
    {
        public override IntPtr AllocateCore(nuint size, bool clear)
            => Marshal.AllocHGlobal(checked((IntPtr)(long)size));

        public override void FreeCore(IntPtr data)
            => Marshal.FreeHGlobal(data);
    }
    
#if NET6_0_OR_GREATER
    public sealed class NativeMemoryMemAllocator : MemoryAllocator
    {
        public override bool CanHandleZero => true;
        public override unsafe IntPtr AllocateCore(nuint size, bool clear)
           => clear 
            ? (IntPtr)NativeMemory.AllocZeroed(size)
            : (IntPtr)NativeMemory.Alloc(size);

        public override unsafe void FreeCore(IntPtr data)
            => NativeMemory.Free(data.ToPointer());
    }
#endif
}
