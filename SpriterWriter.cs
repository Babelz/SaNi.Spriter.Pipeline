using System;
using System.Collections.Generic;
using System.Globalization;
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

        private bool GetAttributeString(XElement e, string name, out string ret, string def)
        {
            if (e.Attribute(name) != null)
            {
                ret = e.Attribute(name).Value;
                return true;
            }
            ret = def;
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
            WriteEntities(output, entities);
        }

        private void WriteEntities(ContentWriter output, List<XElement> entities)
        {
            foreach (var entity in entities)
            {
                
                output.Write(entity.Attribute("name").Value);
                var objInfos = entity.Elements("obj_info").ToList();
                var charMaps = entity.Elements("character_map").ToList();
                var animations = entity.Elements("animation").ToList();
                // obj infojen määrä
                output.Write(objInfos.Count);
                // kirjota charmappejen määr
                output.Write(charMaps.Count);
                output.Write(animations.Count);

                WriteObjectInfos(output, objInfos);
                WriteCharacterMaps(output, charMaps);
                WriteAnimations(output, animations);
            }
        }

        private void WriteAnimations(ContentWriter output, List<XElement> animations)
        {
            int tempi;
            bool tempb;
            foreach (var animation in animations)
            {
                output.Write(animation.Attribute("name").Value);
                GetAttributeInt32(animation, "length", out tempi, 0);
                output.Write(tempi);
                GetAttributeBoolean(animation, "looping", out tempb, true);
                output.Write(tempb);

                // yks mainline?
                var mainlineKeys = animation.Element("mainline").Elements("key").ToList();
                // monta timelineä w/ reference to mainline
                var timelines = animation.Descendants("timeline").ToList();
                
                output.Write(mainlineKeys.Count);
                output.Write(timelines.Count);

                WriteMainlineKeys(output, mainlineKeys);
                WriteTimelines(output, timelines);


            }
        }

        private void WriteTimelines(ContentWriter output, List<XElement> timelines)
        {
            string objectType;
            foreach (var timeline in timelines)
            {
                // id:tä ei tarvi
                // name, object_type, keyCount
                output.Write(timeline.Attribute("name").Value);
                GetAttributeString(timeline, "object_type", out objectType, "sprite");
                output.Write(objectType);

                var keys = timeline.Elements("key").ToList();
                output.Write(keys.Count);
                WriteTimelineKeys(output, keys, objectType);
                // 
            }
        }

        private void WriteTimelineKeys(ContentWriter output, List<XElement> keys, string objtype)
        {
            int temp;
            string temps;
            float tempf;
            foreach (var key in keys)
            {
                // id:tä ei tarvi
                // spin, time, curve_type, c1, c2, c3, c4, spin, 
                GetAttributeInt32(key, "spin", out temp, 1);
                output.Write(temp);
                GetAttributeInt32(key, "time", out temp, 0);
                output.Write(temp);
                GetAttributeString(key, "curve_type", out temps, "linear");
                output.Write(temps);
                GetAttributeFloat(key, "c1", out tempf, 0f);
                output.Write(tempf);
                GetAttributeFloat(key, "c2", out tempf, 0f);
                output.Write(tempf);
                GetAttributeFloat(key, "c3", out tempf, 0f);
                output.Write(tempf);
                GetAttributeFloat(key, "c4", out tempf, 0f);
                output.Write(tempf);

                XElement obj = key.Element("bone");
                if (obj == null) obj = key.Element("object");
                WriteObject(output, obj, objtype);
            }
        }

        private void WriteObject(ContentWriter output, XElement obj, string objtype)
        {
            // jos bone || object
            
            float tempf;
            int tempi;
            // objname, pivot_x, pivot_y, scale_x, scale_y, x, y, angle
            output.Write(obj.Name.LocalName);
            GetAttributeFloat(obj, "pivot_x", out tempf, 0f);
            output.Write(tempf);
            GetAttributeFloat(obj, "pivot_y", out tempf, (objtype == "bone") ? .5f : 1f);
            output.Write(tempf);

            GetAttributeFloat(obj, "scale_x", out tempf, 1f);
            output.Write(tempf);
            GetAttributeFloat(obj, "scale_y", out tempf, 1f);
            output.Write(tempf);

            GetAttributeFloat(obj, "x", out tempf, 0f);
            output.Write(tempf);
            GetAttributeFloat(obj, "y", out tempf, 0f);
            output.Write(tempf);

            GetAttributeFloat(obj, "angle", out tempf, 0f);
            output.Write(tempf);

            // jos object == folder, file, a
            if (obj.Name.LocalName == "object")
            {
                if (objtype == "sprite")
                {
                    GetAttributeInt32(obj, "folder", out tempi, -1);
                    output.Write(tempi);
                    GetAttributeInt32(obj, "file", out tempi, -1);
                    output.Write(tempi);
                    GetAttributeFloat(obj, "a", out tempf, 1f);
                    output.Write(tempf);
                }
            }
        }

        private void WriteMainlineKeys(ContentWriter output, List<XElement> mainlineKeys)
        {
            foreach (var mainlineKey in mainlineKeys)
            {
                WriteMainlineKey(output, mainlineKey);               
            }
        }

        private void WriteMainlineKey(ContentWriter output, XElement key)
        {
            // time
            int tempi;
            string temps;
            float tempf;
            GetAttributeInt32(key, "time", out tempi, 0);
            output.Write(tempi);
            // objectCount, boneCount
            var objectRefs = key.Elements("object_ref").ToList();
            var boneRefs = key.Elements("bone_ref").ToList();
            output.Write(objectRefs.Count);
            output.Write(boneRefs.Count);
            // curve_type
            GetAttributeString(key, "curve_type", out temps, "linear");
            output.Write(temps);
            // c1, c2, c3, c4
            GetAttributeFloat(key, "c1", out tempf, 0f);
            output.Write(tempf);
            GetAttributeFloat(key, "c2", out tempf, 0f);
            output.Write(tempf);
            GetAttributeFloat(key, "c3", out tempf, 0f);
            output.Write(tempf);
            GetAttributeFloat(key, "c4", out tempf, 0f);
            output.Write(tempf);
            // bone, objectref
            WriteBoneRefs(output, boneRefs);
            WriteObjectRefs(output, objectRefs);
            
        }

        private void WriteBoneRefs(ContentWriter output, List<XElement> boneRefs)
        {
            int temp;
            foreach (var boneRef in boneRefs)
            {
                GetAttributeInt32(boneRef, "parent", out temp, -1);
                output.Write(temp);
                GetAttributeInt32(boneRef, "timeline", out temp, 0);
                output.Write(temp);
                GetAttributeInt32(boneRef, "key", out temp, 0);
                output.Write(temp);
            }
        }

        private void WriteObjectRefs(ContentWriter output, List<XElement> objectRefs)
        {
            int temp;
            foreach (var objectRef in objectRefs)
            {
                GetAttributeInt32(objectRef, "parent", out temp, -1);
                output.Write(temp);
                GetAttributeInt32(objectRef, "timeline", out temp, 0);
                output.Write(temp);
                GetAttributeInt32(objectRef, "key", out temp, 0);
                output.Write(temp);
                GetAttributeInt32(objectRef, "z_index", out temp, 0);
                output.Write(temp);
            }
            
        }

        private void WriteCharacterMaps(ContentWriter output, List<XElement> charMaps)
        {
            foreach (var charMap in charMaps)
            {
                output.Write(charMap.Attribute("name").Value);

                var maps = charMap.Elements("map").ToList();
                // kirjotetaan monta mappia
                output.Write(maps.Count);
                WriteMaps(output, maps);
            }
        }

        private void WriteMaps(ContentWriter output, List<XElement> maps)
        {
            int tempi;
            foreach (var map in maps)
            {
                GetAttributeInt32(map, "folder", out tempi, 0);
                output.Write(tempi);
                GetAttributeInt32(map, "file", out tempi, 0);
                output.Write(tempi);
                GetAttributeInt32(map, "target_folder", out tempi, -1);
                output.Write(tempi);
                GetAttributeInt32(map, "target_file", out tempi, -1);
                output.Write(tempi);
            }
        }

        private void WriteObjectInfos(ContentWriter output, List<XElement> objInfos)
        {
            int tempi;
            foreach (var info in objInfos)
            {
                // todo voiko olla null?
                output.Write(info.Attribute("name").Value);
                output.Write(info.Attribute("type").Value);
                GetAttributeInt32(info, "w", out tempi);
                output.Write(tempi);
                GetAttributeInt32(info, "h", out tempi);
                output.Write(tempi);

                // TODO pitääkö kirjottaa jotain muuta näistä? 
                // pitää venata päivitettyä dokkaria
            }
        }

        #region Folders and files

        private void WriteFolders(ContentWriter output, List<XElement> folders)
        {
            foreach (var folder in folders)
            {
                string temp;
                // ei kaaduta jos folderilal ei oo nimeä
                GetAttributeString(folder, "name", out temp, "");
                output.Write(temp);
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

        #endregion
    }
}
