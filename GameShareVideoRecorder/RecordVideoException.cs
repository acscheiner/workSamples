using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CastleHillGaming.GameShare.VideoRecorder
{
    class RecordVideoException : ApplicationException
    {
        public RecordVideoException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RecordVideoException(string message) : base(message)
        {
        }
    }
}
