using System;
using System.Collections.Generic;
using System.Text;

namespace RomanPort.PCParadiseBot
{
    public static class PCStatics
    {
        public static PCStaticsEnviornment enviornment = new PCStaticsEnviornment();
    }

    public class PCStaticsEnviornment
    {
        public string access_token;
        public string reddit_secret;
        public ulong guild_id = 246414844851519490;

        public ulong role_admin = 478375138304327691;
        public ulong role_moderator = 263122526505402369;
        public ulong channel_welcome = 672617188003151941;
        public ulong channel_general = 246414844851519490;
        public ulong channel_logs = 344309200069066763;
        public ulong channel_sales = 582712197956763679;
    }
}
