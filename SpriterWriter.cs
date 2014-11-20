using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using SaNi.Spriter.Data;

namespace SaNi.Spriter.Pipeline
{
    [ContentTypeWriter]
    public class SpriterWriter : ContentTypeWriter<SpriterShadowData>
    {
        #region Util
        private bool GetAttributeInt32(XElement p_element, String p_name, out Int32 p_out, Int32 p_default = 0)
        {
            if (p_element.Attribute(p_name) != null)
            {
                return Int32.TryParse(p_element.Attribute(p_name).Value, out p_out);
            }
            p_out = p_default;
            return false;
        }
        private bool GetAttributeFloat(XElement p_element, String p_name, out float p_out, float p_default = 0.0f)
        {
            if (p_element.Attribute(p_name) != null)
            {
                return float.TryParse(p_element.Attribute(p_name).Value, NumberStyles.Any, CultureInfo.InvariantCulture, out p_out);
            }
            p_out = p_default;
            return false;
        }
        private bool GetAttributeBoolean(XElement p_element, String p_name, out bool p_out, bool p_default = true)
        {
            if (p_element.Attribute(p_name) != null)
            {
                return Boolean.TryParse(p_element.Attribute(p_name).Value, out p_out);
            }
            p_out = p_default;
            return false;
        }

        #endregion

        /// <summary>
        /// ÄLÄ KOSKAAN MENE KOSKEMAAN TÄHÄN, ÄLÄ KOSKAAN
        /// Osottaa suoraan spriter API:n .dll, jos vaihdat, vaihda SaNi.Spriterin assembly nimi 
        /// </summary>
        /// <param name="targetPlatform"></param>
        /// <returns></returns>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "SaNi.Spriter.SpriterModel, SaNi.Spriter";
        }

        /// <summary>
        /// ÄLÄ KOSKAAN MENE KOSKEMAAN TÄHÄN, ÄLÄ KOSKAAN
        /// Osottaa suoraan spriter API:n .dll, jos vaihdat, vaihda SaNi.Spriterin assembly nimi 
        /// </summary>
        /// <param name="targetPlatform"></param>
        /// <returns></returns>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "SaNi.Spriter.SpriterReader, SaNi.Spriter";
        }

        protected override void Write(ContentWriter output, SpriterShadowData value)
        {
            XElement root = value.Document.Descendants("spriter_data").First();
            // versio
            output.Write(root.Attribute("scml_version").Value);
            output.Write(root.Attribute("generator").Value);
            output.Write(root.Attribute("generator_version").Value);
            // foldereiden määrä
            List<XElement> folders = value.Document.Root.Descendants("folder").ToList();
            output.Write(folders.Count);
            // entityjen määrä
            List<XElement> entities = value.Document.Root.Descendants("entity").ToList();
            output.Write(entities.Count);
            
            WriteFolders(output, folders);
        }

        private void WriteFolders(ContentWriter output, List<XElement> folders)
        {
            foreach (var folder in folders)
            {
                output.Write(folder.Attribute("name").Value);
                List<XElement> files = folder.Descendants("file").ToList();
                // filujen määrä
                output.Write(files.Count);
                // filut
                WriteFiles(output, files);
            }
        }

        private void WriteFiles(ContentWriter output, List<XElement> files)
        {
            int temp;
            float tempf;
            foreach (var file in files)
            {
                output.Write(file.Attribute("name").Value);

                GetAttributeInt32(file, "width", out temp);
                output.Write(temp);
                GetAttributeInt32(file, "height", out temp);
                output.Write(temp);
                GetAttributeFloat(file, "pivot_x", out tempf);
                output.Write(tempf);
                GetAttributeFloat(file, "pivot_y", out tempf, 1f);
                output.Write(tempf);
            }
        }
    }
}
