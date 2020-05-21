using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.ComponentModel;

namespace StarCraftMapBrowser
{

    //make map class
    //name
    //filename
    //newfilename
    //description
    //picture
    public class Map
    {
        private string _orgFilename;
        private string _name;
        private string _description;
        private string _hash;
        private string _tileHash;
        public Map() { }
        
        public Map(string orgFilename, string name, string description, string hash, string tileHash)
        {
            _orgFilename = orgFilename;
            _name = name;
            _description = description;
            _hash = hash;
            _tileHash = tileHash;
        }
        [Browsable(true)]
        public string orgFilename { get { return _orgFilename; } set { _orgFilename = value; } }
        [Browsable(true)]
        public string name { get { return _name; } set { _name = value; } }
        [Browsable(true)]
        public string description { get { return _description; } set { _description = value; } }
        [Browsable(true)]
        public string hash { get { return _hash; } set { _hash = value; } }
        [Browsable(true)]
        public string tileHash { get { return _tileHash; } set { _tileHash = value; } }
    }
}
