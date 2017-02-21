using System;

namespace Atomix.Kernel_H.Lib.Cairo
{
    /* typedef enum _cairo_format {
     *      CAIRO_FORMAT_INVALID   = -1,
     *      CAIRO_FORMAT_ARGB32    = 0,
     *      CAIRO_FORMAT_RGB24     = 1,
     *      CAIRO_FORMAT_A8        = 2,
     *      CAIRO_FORMAT_A1        = 3,
     *      CAIRO_FORMAT_RGB16_565 = 4,
     *      CAIRO_FORMAT_RGB30     = 5
     * } cairo_format_t;
     */
    internal enum ColorFormat : int
    {
        Invalid     = -1,
        ARGB32      = 0,
        RGB24       = 1,
        A8          = 2,
        A1          = 3,
        RGB16_565   = 4,
        RGB30       = 5
    }
}
