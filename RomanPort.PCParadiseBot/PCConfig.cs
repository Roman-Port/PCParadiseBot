using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.PCParadiseBot
{
    public class PCConfig
    {
        public string enviornment_name;
        public string server_name;
        public Dictionary<string, PCStaticsEnviornment> enviornments;

        public int part_sale_module_update_interval_seconds = 3600;
    }
}
