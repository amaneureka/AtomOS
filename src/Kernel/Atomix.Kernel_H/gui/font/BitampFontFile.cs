using System;

using Atomix.Kernel_H.core;
using Atomix.Kernel_H.gui.font.BDF;

namespace Atomix.Kernel_H.gui.font
{
    public class BitampFontFile : GenericFont
    {
        protected string FontData;
        protected bool IsValid;

        public BitampFontFile(string aData)
        {
            this.FontData = aData;
            this.IsValid = LoadFontData();
        }

        private bool LoadFontData()
        {
            var strs = FontData.Split('\n');
            int NumberOfLines = strs.Length;

            Glyph CurrentGlyph = null;
            bool ParsingIndividualFont = false, ParsingGlyphTable = false;

            int GlyphWidth = 0, GlyphHeight = 0, GlyphXOffset = 0, GlyphYOffset = 0, GlyphDWidth = 0, GlyphCode = 0;
            for (int index = 0; index < NumberOfLines; index++)
            {
                var xCurrent = strs[index];
                if (ParsingGlyphTable)
                {

                }
                else if (ParsingIndividualFont)
                {
                    if (xCurrent == "BITAMP")
                    {
                        ParsingGlyphTable = true;
                        continue;
                    }
                    var PropertyAndtag = xCurrent.Split(' ');
                    var Tag = PropertyAndtag[0];
                    if (Tag == "ENCODING")
                        GlyphCode = int.Parse(PropertyAndtag[1]);
                    else if (Tag == "DWIDTH")
                        GlyphDWidth = int.Parse(PropertyAndtag[1]);
                    else if (Tag == "BBX")
                    {
                        GlyphWidth = int.Parse(PropertyAndtag[1]);
                        GlyphHeight = int.Parse(PropertyAndtag[2]);
                        GlyphXOffset = int.Parse(PropertyAndtag[3]);
                        GlyphYOffset = int.Parse(PropertyAndtag[4]);
                    }
                }
                else if (xCurrent.StartsWith("STARTCHAR"))
                {
                    CurrentGlyph = null;
                    ParsingIndividualFont = true;
                }
                else if (xCurrent == "ENDCHAR")
                {
                    CurrentGlyph = new Glyph()
                        {
                            Width = GlyphWidth,
                            Height = GlyphHeight,
                            xOffset = GlyphXOffset,
                            yOffset = GlyphYOffset,
                            DWidth = GlyphDWidth,
                            Character = GlyphCode
                        };
                    ParsingIndividualFont = false;
                    ParsingGlyphTable = false;
                }
                Heap.Free(xCurrent);
            }

            Heap.Free((object)strs);
            return true;
        }
    }
}
