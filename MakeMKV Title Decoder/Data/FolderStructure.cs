using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Data {

    // Wrapping as a class allows for easier folder renaming later
    public class Folder {
        public Folder? Parent = null;
        public List<Folder> Children = new();
        public string Name = "";

        public bool IsValidFolder => !string.IsNullOrWhiteSpace(Name);
        public IEnumerable<string> GetFullPath {
            get
            {
                if (Parent != null)
                {
                    foreach(string str in Parent.GetFullPath)
                    {
                        yield return str;
                    }
                }

                yield return this.Name;
            }
        }
        public override string ToString() {
            return Path.Combine(GetFullPath.ToArray());
        }

        public void Reset() {
            this.Parent = null;
            this.Children.Clear();
            this.Name = "";
        }
    }
}
