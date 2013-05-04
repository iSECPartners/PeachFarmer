using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteHarvesterTest.TestStreams
{
    public class BookmarkableStream : MemoryStream
    {
        private long _bookmarkedPosition;

        public BookmarkableStream()
            :base()
        {
            _bookmarkedPosition = 0;
        }

        public void SetBookmark()
        {
            _bookmarkedPosition = this.Position;
        }

        public void ResetToBookmark()
        {
            this.Position = _bookmarkedPosition;
        }
    }
}
