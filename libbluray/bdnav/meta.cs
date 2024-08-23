using libbluray.disc;
using libbluray.file;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace libbluray.bdnav {

    public class META_TN {
        public string language_code; // length 4
        public string filename;

        public UInt32 playlist;
        public UInt32 num_chapter;
        public List<string> chapter_name = new();
    }

    public class META_ROOT {
        public byte dl_count;
        public List<META_DL> dl_entries = new();

        public UInt32 tn_count;
        public List<META_TN> tn_entries = new();
    }

    public static class meta {

        const string module = "DBG_DIR";
        const UInt32 MAX_META_FILE_SIZE = 0xFFFFF;
        const string DEFAULT_LANGUAGE = "eng";

        private static void _parseManifestNode(XmlNodeList a_node, META_DL disclib) {
            foreach(XmlNode cur_node in a_node)
            {
                if (cur_node is XmlElement element/*cur_node.NodeType == XmlNodeType.Element*/)
                {
                    if (cur_node.ParentNode.Name == "title")
                    {
                        if (cur_node.Name == "name")
                        {
                            disclib.di_name = cur_node.Value;
                        }
                        if (cur_node.Name == "alternative")
                        {
                            disclib.di_alternative = cur_node.Value;
                        }
                        if (cur_node.Name == "numSets")
                        {
                            disclib.di_num_sets = byte.Parse(cur_node.Value);
                        }
                        if (cur_node.Name == "setNumber")
                        {
                            disclib.di_set_number = byte.Parse(cur_node.Value);
                        }
                    } else if (cur_node.ParentNode.Name == "tableOfContents")
                    {
                        if (cur_node.Name == "titleName" && element.HasAttribute("titleNumber"))
                        {
                            META_TITLE new_entry = new();

                            uint i = disclib.toc_count;
                            disclib.toc_count++;
                            disclib.toc_entries.Add(new_entry);
                            new_entry.title_number = uint.Parse(element.GetAttribute("titleNumber"));
                            new_entry.title_name = cur_node.Value;
                        }
                    } else if(cur_node.ParentNode.Name == "description")
                    {
                        if (cur_node.Name == "thumbnail" && element.HasAttribute("href"))
                        {
                            META_THUMBNAIL new_entry = new();

                            byte i = disclib.thumb_count;
                            disclib.thumb_count++;
                            disclib.thumbnails.Add(new_entry);
                            new_entry.path = element.GetAttribute("href");
                            if (element.HasAttribute("size"))
                            {
                                string[] vals = element.GetAttribute("size").Split('x');
                                new_entry.xres = uint.Parse(vals[0]);
                                new_entry.yres = uint.Parse(vals[1]);
                            } else
                            {
                                new_entry.xres = new_entry.yres = -1;
                            }
                        }
                    }
                }
                _parseManifestNode(cur_node.ChildNodes, disclib);
            }
        }

        private static void _parseTnManifestNode(XmlNodeList a_node, META_TN disclib) {
            foreach(XmlNode cur_node in a_node)
            {
                if (cur_node is XmlElement element)
                {
                    if (element.ParentNode.Name == "chapters")
                    {
                        if (element.Name == "name")
                        {
                            uint i = disclib.num_chapter;
                            disclib.num_chapter++;
                            disclib.chapter_name.Add(element.Value);
                        }
                    }
                }
                _parseTnManifestNode(cur_node.ChildNodes, disclib);
            }
        }

        private static void _findMetaXMLfiles(META_ROOT meta, BD_DISC disc) {
            string? ent;
            bool res;
            BD_DIR_H? dir = disc.open_dir(Path.Combine("BDMV", "META", "DL"));
            if (dir == null)
            {
                Utils.BD_DEBUG(module, "Failed to open meta dir BDMV/META/DL/");
            } else
            {
                for(res = dir.read(out ent); res; res = dir.read(out ent))
                {
                    if (ent[0] == '.')
                    {
                        continue;
                    } else if (ent.ToLower().StartsWith("bdmt_") && (ent.Length == 12))
                    {
                        META_DL new_entry = new();

                        byte i = meta.dl_count;
                        meta.dl_count++;
                        meta.dl_entries.Add(new_entry);

                        new_entry.filename = ent;
                        new_entry.language_code = ent.Substring(5, 3).ToLower();
                    }
                }
                dir.close();
            }

            dir = disc.open_dir(Path.Combine("BDMV", "META", "TN"));
            if (dir == null)
            {
                Utils.BD_DEBUG(module, "Failed to open meta dir BDMV/META/TN/");
            } else
            {
                for(res = dir.read(out ent); res; res = dir.read(out ent))
                {
                    if (ent.ToLower().StartsWith("tnmt_") && ent.Length == 18)
                    {
                        META_TN new_entry = new();

                        uint i = meta.tn_count;
                        meta.tn_count++;
                        meta.tn_entries.Add(new_entry);

                        new_entry.filename = ent;
                        new_entry.language_code = ent.Substring(5, 3).ToLower();
                        new_entry.playlist = uint.Parse(ent.Substring(9));
                    }
                }
                dir.close();
            }
        }

        public static META_ROOT? parse(BD_DISC disc) {
            META_ROOT root = new();
            Int32 i;

            root.dl_count = 0;

            _findMetaXMLfiles(root, disc);

            for(i = 0; i < root.dl_count; i++)
            {
                byte[] data = null;
                UInt64 size = disc.read_file(Path.Combine("BDMV", "META", "DL"), root.dl_entries[i].filename, out data);
                if (data == null || size == 0)
                {
                    Utils.BD_DEBUG(module, $"Failed to read BDMV/META/DL/{root.dl_entries[i].filename}");
                }else
                {
                    XmlDocument doc = new();
                    MemoryStream ms = new MemoryStream(data);
                    doc.Load(ms); // TODO handle exception

                    if (doc == null)
                    {
                        Utils.BD_DEBUG(module, $"Failed to parse BDMV/META/DL/{root.dl_entries[i].filename}");
                    } else
                    {
                        //XmlNode root_element = doc.FirstChild;
                        root.dl_entries[i].di_name = root.dl_entries[i].di_alternative = null;
                        root.dl_entries[i].di_num_sets = root.dl_entries[i].di_set_number = -1;
                        root.dl_entries[i].toc_count = root.dl_entries[i].thumb_count = 0;
                        root.dl_entries[i].toc_entries.Clear();
                        root.dl_entries[i].thumbnails.Clear();
                        _parseManifestNode(doc.ChildNodes, root.dl_entries[i]);
                    }
                }
            }

            for (i = 0; i < root.tn_count; i++)
            {
                byte[] data = null;
                UInt64 size = disc.read_file(Path.Combine("BDMV", "META", "TN"), root.tn_entries[i].filename, out data);
                if (data == null || size == 0)
                {
                    Utils.BD_DEBUG(module, $"Failed to read BDMV/META/TN/{root.tn_entries[i].filename}");
                } else
                {
                    XmlDocument doc = new();
                    MemoryStream ms = new MemoryStream(data);
                    doc.Load(ms); // TODO handle exception

                    if (doc == null)
                    {
                        Utils.BD_DEBUG(module, $"Failed to parse BDMV/META/TN/{root.tn_entries[i].filename}");
                    } else
                    {
                        //XmlNode root_element = doc.FirstChild;
                        _parseTnManifestNode(doc.ChildNodes, root.tn_entries[i]);
                    }
                }
            }

            return root;
        }

        public static META_DL? get(META_ROOT? meta_root, string? language_code) {
            Int32 i;
            if (meta_root == null || meta_root.dl_count == 0)
            {
                Utils.BD_DEBUG(module, $"meta_get not possible, no info available!");
                return null;
            }

            if (language_code != null)
            {
                for(i = 0; i < meta_root.dl_count; i++)
                {
                    if (language_code == meta_root.dl_entries[i].language_code)
                    {
                        return meta_root.dl_entries[i];
                    }
                }
            }

            for (i = 0; i < meta_root.dl_count; i++)
            {
                if (DEFAULT_LANGUAGE == meta_root.dl_entries[i].language_code)
                {
                    Utils.BD_DEBUG(module, $"using default disclib language '{DEFAULT_LANGUAGE}'");
                    return meta_root.dl_entries[i];
                }
            }

            Utils.BD_DEBUG(LogLevel.Warning, module, $"requested disclib language '{language_code}' or default '{DEFAULT_LANGUAGE}' not found, using '{meta_root.dl_entries[0].language_code}' instead");
            return meta_root.dl_entries[0];
        }

        public static META_TN? get_tn(META_ROOT? meta_root, string? language_code, UInt32 playlist) {
            Int32 i;
            META_TN? tn_default = null;
            META_TN? tn_first = null;

            if (meta_root == null || meta_root.tn_count == 0)
            {
                return null;
            }

            for (i = 0; i < meta_root.tn_count; i++)
            {
                if (meta_root.tn_entries[i].playlist == playlist)
                {
                    if (language_code != null && language_code == meta_root.tn_entries[i].language_code)
                    {
                        return meta_root.tn_entries[i];
                    }
                    if (DEFAULT_LANGUAGE == meta_root.tn_entries[i].language_code)
                    {
                        tn_default = meta_root.tn_entries[i];
                    }
                    if (tn_first == null)
                    {
                        tn_first = meta_root.tn_entries[i];
                    }
                }
            }

            if (tn_default != null)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Requested disclib language '{language_code}' not found, using default language '{DEFAULT_LANGUAGE}'");
                return tn_default;
            }
            if(tn_first != null)
            {
                Utils.BD_DEBUG(LogLevel.Warning, module, $"Requested disclib language '{language_code}' or default '{DEFAULT_LANGUAGE}' not found, using '{tn_first.language_code}' instead");
                return tn_first;
            }

            throw new Exception("You shouldn't be here.");
        }

    }
}
