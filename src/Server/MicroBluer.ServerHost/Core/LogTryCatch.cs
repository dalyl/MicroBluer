using Common.Logging;
using MicroBluer.ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroBluer.ServerHost.Core
{
    public class LogTryCatch: TryCatch
    {
        private static readonly ILog logger = LogManager.GetLogger<LogTryCatch>();

        public LogTryCatch()
        {
            ShowMessage = msg => logger.Info(msg);
        }
    }
}
