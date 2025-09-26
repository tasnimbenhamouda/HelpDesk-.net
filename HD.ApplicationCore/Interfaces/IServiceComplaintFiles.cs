using HD.ApplicationCore.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Interfaces
{
    public interface IServiceComplaintFiles : IService<ComplaintFile>
    {
        public bool RemoveFile(string filePath, int complaintId);
       public ComplaintFile? GetByPath(string path);
        public byte[] DownloadFile(string path);
    }

}
