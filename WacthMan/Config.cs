using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace WacthMan
{
    public class Config
    {
        public string WatchColor { get; set; }
        public string ActionSeen { get; set; }
        public string ActionLost { get; set; }
        public Config()
        {
            this.WatchColor = "#000000";
            this.ActionLost = "";
            this.ActionSeen = "";
        }
        public Config(string watchColor, string actionSeen, string actionLost)
        {
            this.WatchColor = watchColor;
            this.ActionSeen = actionSeen;
            this.ActionLost = actionLost;
        }
        public static Config Clone(Config cfg)
        {
            return new Config(cfg.WatchColor, cfg.ActionSeen, cfg.ActionLost);
        }
    }
}
