using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace SaNi.Spriter.Pipeline
{
    [ContentImporter(".scml", DisplayName = "SaNi.Spriter SCML - Spriter importer", DefaultProcessor = "SpriterProcessor")]
    public class SpriterImporter : ContentImporter<XDocument>
    {
        public override XDocument Import(string filename, ContentImporterContext context)
        {
            return new XDocument();
        }
    }
}
