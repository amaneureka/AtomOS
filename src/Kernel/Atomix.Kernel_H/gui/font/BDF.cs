/*
* PROJECT:          Atomix Development
* LICENSE:          Copyright (C) Atomix Development, Inc - All Rights Reserved
*                   Unauthorized copying of this file, via any medium is
*                   strictly prohibited Proprietary and confidential.
* PURPOSE:          BDF Font Loader
* PROGRAMMERS:      Aman Priyadarshi (aman.eureka@gmail.com)
*/

using Atomix.Kernel_H.IO;
using Atomix.Kernel_H.Lib;
using Atomix.Kernel_H.Core;

namespace Atomix.Kernel_H.Gui.Font
{
    public class BDF : GenericFont
    {
        public BDF(string aFontName, Stream aStream)
            :base(aFontName, aStream)
        {
            if (aStream != null)
                mIsValid = LoadFontFile();
        }

        private unsafe bool LoadFontFile()
        {
            string xData = mStream.ReadToEnd();
            var Lines = xData.Split('\n');
            Heap.Free(xData);
            
            int GlyphLine = 0;
            bool ReadingCharProperties = false, ReadingGlyphTable = false;
            
            Glyph xGlyph = null;
            foreach (var line in Lines)
            {
                if (line.StartsWith("STARTCHAR"))
                {
                    xGlyph = new Glyph();
                    ReadingCharProperties = true;
                }
                else if (line.StartsWith("ENDCHAR"))
                {
                    ReadingGlyphTable = true;
                    ReadingCharProperties = true;
                    Debug.Write("Found %d\n", xGlyph.Unicode);
                }
                else if (ReadingCharProperties)
                {
                    if (line.StartsWith("BITMAP"))
                    {
                        ReadingCharProperties = false;
                        ReadingGlyphTable = true;
                        GlyphLine = 0;
                    }
                    else
                    {
                        var prop = line.Split(' ');
                        var key = prop[0];
                        
                        if (key == "ENCODING")
                            xGlyph.Unicode = uint.Parse(prop[1]);
                        else if (key == "DWIDTH")
                            xGlyph.DWidth = uint.Parse(prop[1]);
                        else if (key == "BBX")
                        {
                            xGlyph.Width = uint.Parse(prop[1]);
                            xGlyph.Height = uint.Parse(prop[2]);
                            xGlyph.xOffset = uint.Parse(prop[3]);
                            xGlyph.yOffset = uint.Parse(prop[4]);
                            xGlyph.Bitmap = (uint*)Heap.kmalloc(xGlyph.Height << 2);
                        }
                        Heap.FreeArray(prop);
                    }
                }
                else if (ReadingGlyphTable)
                {
                    xGlyph.Bitmap[GlyphLine++] = Numerics.ParseHex(line);
                }
            }
            Heap.FreeArray(Lines);
            return true;
        }
    }
}
