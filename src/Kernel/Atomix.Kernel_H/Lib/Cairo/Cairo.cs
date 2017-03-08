/*
* PROJECT:          Atomix Development
* LICENSE:          BSD 3-Clause (LICENSE.md)
* PURPOSE:          Cairo Native Methods
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Runtime.InteropServices;

using Atomixilc.Attributes;

namespace Atomix.Kernel_H.Lib.Cairo
{
    internal static class Cairo
    {
        const string LIBRARY = "libcairo.a";

        /* Note: Because Arguments are pushed from left to right by the current compiler
         * we have to make it reverse for successful function calling
         */

        /// <summary>
        /// cairo_public cairo_t *
        /// cairo_create(cairo_surface_t* target);
        /// </summary>
        [NoException]
        [Plug("cairo_create")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint Create(uint target);

        /// <summary>
        /// cairo_public cairo_surface_t *
        /// cairo_image_surface_create(cairo_format_t format,
        ///                             int width,
        ///                             int height);
        /// </summary>
        [NoException]
        [Plug("cairo_image_surface_create")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint ImageSurfaceCreate(int height, int width, ColorFormat format);

        /// <summary>
        /// cairo_public cairo_surface_t *
        /// cairo_image_surface_create_for_data(unsigned char* data,
        ///                                     cairo_format_t format,
        ///                                     int width,
        ///                                     int height,
        ///                                     int stride);
        /// </summary>
        [NoException]
        [Plug("cairo_image_surface_create_for_data")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint ImageSurfaceCreateForData(int stride, int height, int width, ColorFormat format, uint data);
        
        /// <summary>
        /// cairo_public void
        /// cairo_set_source_rgb(cairo_t* cr, double red, double green, double blue);
        /// </summary>
        [NoException]
        [Plug("cairo_set_source_rgb")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetSourceRGB(double blue, double green, double red, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_source_rgba(cairo_t* cr, double red, double green, double blue,
        ///                        double alpha);
        /// </summary>
        [NoException]
        [Plug("cairo_set_source_rgba")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetSourceRGBA(double alpha, double blue, double green, double red, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_save (cairo_t *cr);
        /// </summary>
        [NoException]
        [Plug("cairo_save")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Save(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_restore (cairo_t *cr);
        /// </summary>
        [NoException]
        [Plug("cairo_restore")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Restore(uint cr);

        /// <summary>
        /// cairo_public unsigned char *
        /// cairo_image_surface_get_data(cairo_surface_t* surface);
        /// </summary>
        [NoException]
        [Plug("cairo_image_surface_get_data")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint ImageSurfaceGetData(uint surface);

        /// <summary>
        /// cairo_public void
        /// cairo_show_text(cairo_t* cr, const char* utf8);
        /// </summary>
        [NoException]
        [Plug("cairo_show_text")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe void ShowText(sbyte* utf8, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_rectangle(cairo_t* cr,
        ///                 double x, double y,
        ///                 double width, double height);
        /// </summary>
        [NoException]
        [Plug("cairo_rectangle")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Rectangle(double height, double width, double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_fill(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_fill")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Fill(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_stroke(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_stroke")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Stroke(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_destroy(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_destroy")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Destroy(uint cr);

        /// <summary>
        /// cairo_public int
        /// cairo_version(void);
        /// </summary>
        [NoException]
        [Plug("cairo_version")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int Version();

        /// <summary>
        /// cairo_public void
        /// cairo_surface_destroy(cairo_surface_t* surface);
        /// </summary>
        [NoException]
        [Plug("cairo_surface_destroy")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SurfaceDestroy(uint surface);

        /// <summary>
        /// cairo_public int
        /// cairo_format_stride_for_width(cairo_format_t format,
        ///                                int width);
        /// </summary>
        [NoException]
        [Plug("cairo_format_stride_for_width")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int FormatStrideForWidth(int width, ColorFormat format);


        /// <summary>
        /// cairo_public void
        /// cairo_surface_flush(cairo_surface_t* surface);
        /// </summary>
        [NoException]
        [Plug("cairo_surface_flush")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SurfaceFlush(uint surface);

        /// <summary>
        /// cairo_public void
        /// cairo_set_line_width(cairo_t* cr, double width);
        /// </summary>
        [NoException]
        [Plug("cairo_set_line_width")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetLineWidth(double width, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_source (cairo_t *cr, cairo_pattern_t *source);
        /// </summary>
        [NoException]
        [Plug("cairo_set_source")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetSource(uint source, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_paint(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_paint")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Paint(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_fill_preserve(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_fill_preserve")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void FillPreserve(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_dash(cairo_t* cr,
        ///                 const double* dashes,
        ///                 int num_dashes,
        ///                 double offset);
        /// </summary>
        [NoException]
        [Plug("cairo_set_dash")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetDash(double offset, int num_dashes, uint dashes, uint cr);

        /// <summary>
        /// cairo_public cairo_status_t
        /// cairo_status(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_status")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Status Status(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_clip(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_clip")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Clip(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_clip(cairo_t* cr);
        /// </summary>
        [NoException]
        [Plug("cairo_reset_clip")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void ResetClip(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_pattern_destroy (cairo_pattern_t *pattern);
        /// </summary>
        [NoException]
        [Plug("cairo_pattern_destroy")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PatternDestroy(uint pattern);

        /// <summary>
        /// cairo_public cairo_status_t
        /// cairo_surface_write_to_png(cairo_surface_t* surface,
        ///                             const char* filename);
        /// </summary>
        [NoException]
        [Plug("cairo_surface_write_to_png")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Status SurfaceWriteToPng(uint filename, uint surface);

        /// <summary>
        /// cairo_public void
        /// cairo_select_font_face(cairo_t* cr,
        ///                         const char* family,
        ///                         cairo_font_slant_t   slant,
		///                         cairo_font_weight_t weight);
        /// </summary>
        [NoException]
        [Plug("cairo_select_font_face")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe void SelectFontFace(FontWeight weight, FontSlant slant, sbyte* family, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_operator (cairo_t *cr, cairo_operator_t op);
        /// </summary>
        [NoException]
        [Plug("cairo_set_operator")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetOperator(Operator op, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_font_size(cairo_t* cr, double size);
        /// </summary>
        [NoException]
        [Plug("cairo_set_font_size")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetFontSize(double size, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_translate (cairo_t *cr, double tx, double ty);
        /// </summary>
        [NoException]
        [Plug("cairo_translate")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void Translate(double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_move_to(cairo_t* cr, double x, double y);
        /// </summary>
        [NoException]
        [Plug("cairo_move_to")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void MoveTo(double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_line_to(cairo_t* cr, double x, double y);
        /// </summary>
        [NoException]
        [Plug("cairo_line_to")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void LineTo(double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_rel_line_to(cairo_t* cr, double dx, double dy);
        /// </summary>
        [NoException]
        [Plug("cairo_rel_line_to")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void RelativeLineTo(double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_curve_to(cairo_t* cr,
        ///                 double x1, double y1,
        ///                 double x2, double y2,
        ///                 double x3, double y3);
        /// </summary>
        [NoException]
        [Plug("cairo_curve_to")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void CurveTo(double y3, double x3, double y2, double x2, double y1, double x1, uint cr);

        /// <summary>
        /// cairo_public cairo_surface_t *
        /// cairo_image_surface_create_from_png (const char	*filename);
        /// </summary>
        [NoException]
        [Plug("cairo_image_surface_create_from_png")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe uint ImageSurfaceFromPng(sbyte* filename);

        /// <summary>
        /// cairo_public void
        /// cairo_set_source_surface(cairo_t* cr,
        ///                           cairo_surface_t* surface,
        ///                           double x, double y);
        /// </summary>
        [NoException]
        [Plug("cairo_set_source_surface")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void SetSourceSurface(double y, double x, uint surface, uint cr);

        /// <summary>
        /// cairo_public cairo_pattern_t *
        /// cairo_pattern_create_linear (double x0, double y0,
        ///                                double x1, double y1);
        /// </summary>
        [NoException]
        [Plug("cairo_pattern_create_linear")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint PatternCreateLinear(double y1, double x1, double y0, double x0);

        /// <summary>
        /// cairo_public void
        /// cairo_pattern_add_color_stop_rgba (cairo_pattern_t *pattern,
        ///                                     double offset,
        ///                                     double red, double green, double blue,
        ///                                     double alpha);
        /// </summary>
        [NoException]
        [Plug("cairo_pattern_add_color_stop_rgba")]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PatternAddColorStopRgba(double alpha, double blue, double green, double red, double offset, uint pattern);
    }
}