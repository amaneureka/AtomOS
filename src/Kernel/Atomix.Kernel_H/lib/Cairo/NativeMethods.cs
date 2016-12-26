/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          Cairo Native Methods
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using System;
using System.Runtime.InteropServices;

using Atomixilc.Attributes;

namespace Atomix.Kernel_H.Lib.Cairo
{
    internal static class NativeMethods
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
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint cairo_create(uint target);

        /// <summary>
        /// cairo_public cairo_surface_t *
        /// cairo_image_surface_create(cairo_format_t format,
        ///                             int width,
        ///                             int height);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint cairo_image_surface_create(int height, int width, ColorFormat format);

        /// <summary>
        /// cairo_public cairo_surface_t *
        /// cairo_image_surface_create_for_data(unsigned char* data,
        ///                                     cairo_format_t format,
        ///                                     int width,
        ///                                     int height,
        ///                                     int stride);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint cairo_image_surface_create_for_data(int stride, int height, int width, ColorFormat format, uint data);
        
        /// <summary>
        /// cairo_public void
        /// cairo_set_source_rgb(cairo_t* cr, double red, double green, double blue);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_set_source_rgb(double blue, double green, double red, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_source_rgba(cairo_t* cr, double red, double green, double blue,
        ///                        double alpha);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_set_source_rgba(double alpha, double blue, double green, double red, uint cr);

        /// <summary>
        /// cairo_public unsigned char *
        /// cairo_image_surface_get_data(cairo_surface_t* surface);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern uint cairo_image_surface_get_data(uint surface);

        /// <summary>
        /// cairo_public void
        /// cairo_show_text(cairo_t* cr, const char* utf8);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_show_text(uint utf8, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_rectangle(cairo_t* cr,
        ///                 double x, double y,
        ///                 double width, double height);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_rectangle(double height, double width, double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_paint(cairo_t* cr);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_fill(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_stroke(cairo_t* cr);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_stroke(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_destroy(cairo_t* cr);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_destroy(uint cr);

        /// <summary>
        /// cairo_public int
        /// cairo_version(void);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int cairo_version();

        /// <summary>
        /// cairo_public void
        /// cairo_surface_destroy(cairo_surface_t* surface);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_surface_destroy(uint surface);

        /// <summary>
        /// cairo_public int
        /// cairo_format_stride_for_width(cairo_format_t format,
        ///                                int width);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int cairo_format_stride_for_width(int width, ColorFormat format);


        /// <summary>
        /// cairo_public void
        /// cairo_surface_flush(cairo_surface_t* surface);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_surface_flush(uint surface);

        /// <summary>
        /// cairo_public void
        /// cairo_set_line_width(cairo_t* cr, double width);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_set_line_width(double width, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_paint(cairo_t* cr);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_paint(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_fill_preserve(cairo_t* cr);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_fill_preserve(uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_dash(cairo_t* cr,
        ///                 const double* dashes,
        ///                 int num_dashes,
        ///                 double offset);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_set_dash(double offset, int num_dashes, uint dashes, uint cr);

        /// <summary>
        /// cairo_public cairo_status_t
        /// cairo_status(cairo_t* cr);
        /// </summary>
        /// <returns></returns>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Status cairo_status(uint cr);

        /// <summary>
        /// cairo_public cairo_status_t
        /// cairo_surface_write_to_png(cairo_surface_t* surface,
        ///                             const char* filename);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern Status cairo_surface_write_to_png(uint filename, uint surface);

        /// <summary>
        /// cairo_public void
        /// cairo_select_font_face(cairo_t* cr,
        ///                         const char* family,
        ///                         cairo_font_slant_t   slant,
		///                         cairo_font_weight_t weight);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_select_font_face(FontWeight weight, FontSlant slant, uint family, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_operator (cairo_t *cr, cairo_operator_t op);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_set_operator(Operator op, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_set_font_size(cairo_t* cr, double size);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_set_font_size(double size, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_move_to(cairo_t* cr, double x, double y);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_move_to(double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_line_to(cairo_t* cr, double x, double y);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_line_to(double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_rel_line_to(cairo_t* cr, double dx, double dy);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_rel_line_to(double y, double x, uint cr);

        /// <summary>
        /// cairo_public void
        /// cairo_curve_to(cairo_t* cr,
        ///                 double x1, double y1,
        ///                 double x2, double y2,
        ///                 double x3, double y3);
        /// </summary>
        [NoException]
        [DllImport(LIBRARY, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void cairo_curve_to(double y3, double x3, double y2, double x2, double y1, double x1, uint cr);
    }
}