using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace SaNi.Spriter.Pipeline
{
    [ContentTypeWriter]
    public class SpriterWriter : ContentTypeWriter<SpriterShadowData>
    {
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
            Console.WriteLine("joo");
            XElement root = value.Document.Descendants("spriter_data").First();
            // versio
            output.Write(root.Attribute("scml_version").Value);
            output.Write(root.Attribute("generator").Value);
            output.Write(root.Attribute("generator_version").Value);
            // foldereiden määrä
            /*output.Write(value.Textures.Count);
            // filujen määrä per folder
            for (int i = 0; i < value.Textures.Count; i++)
            {
                output.Write(value.Textures[i].Count);
            }*/

            /*
            // tekstuurit
            for (int i = 0; i < value.Textures.Count; i++)
            {
                for (int j = 0; j < value.Textures[i].Count; j++)
                {
                    output.WriteRawObject<Texture2DContent>(value.Textures[i][j]);
                }
            }
             * */
        }
    }
}
