using System;

namespace Atomix.Kernel_H.Lib.Cairo
{
    /* typedef enum _cairo_font_weight {
     *      CAIRO_FONT_WEIGHT_NORMAL,
     *      CAIRO_FONT_WEIGHT_BOLD,
     * } cairo_font_weight_t;
     */
    internal enum FontWeight : int
    {
        Normal = 0,
        Bold = 1
    }

    /* typedef enum _cairo_font_slant {
     *      CAIRO_FONT_SLANT_NORMAL,
     *      CAIRO_FONT_SLANT_ITALIC,
     *      CAIRO_FONT_SLANT_OBLIQUE
     * } cairo_font_slant_t;
     */
    internal enum FontSlant : int
    {
        Normal = 0,
        Italic = 1,
        Oblique = 2
    }
}
