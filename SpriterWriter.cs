using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
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
            output.Write(0);
        }
    }
}
