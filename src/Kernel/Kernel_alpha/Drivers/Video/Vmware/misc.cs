using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel_alpha.Drivers.Video.Vmware
{
    public enum IOPortOffset : ushort
    {
        Index   = 0x0,
        Value   = 0x1,
        Bios    = 0x2,
        IRQ     = 0x8
    };

    public enum Versions : uint
    {
        SVGA_ID_0 = (UInt32)(0x900000UL << 8 | 0x0),
        SVGA_ID_1 = (UInt32)(0x900000UL << 8 | 0x1),
        SVGA_ID_2 = (UInt32)(0x900000UL << 8 | 0x2)
    };

    public enum Registers : uint
    {
        SVGA_REG_ID = 0,
        SVGA_REG_ENABLE = 1,
        SVGA_REG_WIDTH = 2,
        SVGA_REG_HEIGHT = 3,
        SVGA_REG_MAX_WIDTH = 4,
        SVGA_REG_MAX_HEIGHT = 5,
        SVGA_REG_DEPTH = 6,
        SVGA_REG_BITS_PER_PIXEL = 7,       /* Current bpp in the guest */
        SVGA_REG_PSEUDOCOLOR = 8,
        SVGA_REG_RED_MASK = 9,
        SVGA_REG_GREEN_MASK = 10,
        SVGA_REG_BLUE_MASK = 11,
        SVGA_REG_BYTES_PER_LINE = 12,
        SVGA_REG_FB_START = 13,            /* (Deprecated) */
        SVGA_REG_FB_OFFSET = 14,
        SVGA_REG_VRAM_SIZE = 15,
        SVGA_REG_FB_SIZE = 16,

        /* ID 0 implementation only had the above registers, then the palette */

        SVGA_REG_CAPABILITIES = 17,
        SVGA_REG_MEM_START = 18,           /* (Deprecated) */
        SVGA_REG_MEM_SIZE = 19,
        SVGA_REG_CONFIG_DONE = 20,         /* Set when memory area configured */
        SVGA_REG_SYNC = 21,                /* See "FIFO Synchronization Registers" */
        SVGA_REG_BUSY = 22,                /* See "FIFO Synchronization Registers" */
        SVGA_REG_GUEST_ID = 23,            /* Set guest OS identifier */
        SVGA_REG_CURSOR_ID = 24,           /* (Deprecated) */
        SVGA_REG_CURSOR_X = 25,            /* (Deprecated) */
        SVGA_REG_CURSOR_Y = 26,            /* (Deprecated) */
        SVGA_REG_CURSOR_ON = 27,           /* (Deprecated) */
        SVGA_REG_HOST_BITS_PER_PIXEL = 28, /* (Deprecated) */
        SVGA_REG_SCRATCH_SIZE = 29,        /* Number of scratch registers */
        SVGA_REG_MEM_REGS = 30,            /* Number of FIFO registers */
        SVGA_REG_NUM_DISPLAYS = 31,        /* (Deprecated) */
        SVGA_REG_PITCHLOCK = 32,           /* Fixed pitch for all modes */
        SVGA_REG_IRQMASK = 33,             /* Interrupt mask */

        /* Legacy multi-monitor support */
        SVGA_REG_NUM_GUEST_DISPLAYS = 34,/* Number of guest displays in X/Y direction */
        SVGA_REG_DISPLAY_ID = 35,        /* Display ID for the following display attributes */
        SVGA_REG_DISPLAY_IS_PRIMARY = 36,/* Whether this is a primary display */
        SVGA_REG_DISPLAY_POSITION_X = 37,/* The display position x */
        SVGA_REG_DISPLAY_POSITION_Y = 38,/* The display position y */
        SVGA_REG_DISPLAY_WIDTH = 39,     /* The display's width */
        SVGA_REG_DISPLAY_HEIGHT = 40,    /* The display's height */

        /* See "Guest memory regions" below. */
        SVGA_REG_GMR_ID = 41,
        SVGA_REG_GMR_DESCRIPTOR = 42,
        SVGA_REG_GMR_MAX_IDS = 43,
        SVGA_REG_GMR_MAX_DESCRIPTOR_LENGTH = 44,

        SVGA_REG_TRACES = 45,            /* Enable trace-based updates even when FIFO is on */
        SVGA_REG_TOP = 46,               /* Must be 1 more than the last register */

        SVGA_PALETTE_BASE = 1024,        /* Base of SVGA color map */
        SVGA_FIFO_NUM_REGS = 293
    };

    public enum FIFO : ushort
    {
        SVGA_FIFO_MIN = 0,
        SVGA_FIFO_MAX = 4,
        SVGA_FIFO_NEXT_CMD = 8,
        SVGA_FIFO_STOP = 12,
        SVGA_FIFO_CAPABILITIES = 4,
        SVGA_FIFO_GUEST_3D_HWVERSION = 0,
        Update = 1,
    };

    public static class misc
    {
        public const ushort PCI_VENDOR_ID_VMWARE = 0x15AD;
        public const ushort PCI_DEVICE_ID_VMWARE_SVGA2 = 0x0405;
    }
}
