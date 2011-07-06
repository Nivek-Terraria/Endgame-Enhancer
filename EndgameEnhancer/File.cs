using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace EndgameEnhancer
{
    public class File : PropertiesFile
    {
        public File(String propertiesPath) : base(propertiesPath) { }

        public void setPlayerValue(String name, int value)
        {
            setValue(name, value);
        }

        public int getPlayerValue(String name)
        {
            return getValue(name, 0);
        }
    }
}
