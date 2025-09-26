using HD.ApplicationCore.Domain;
using HD.ApplicationCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HD.ApplicationCore.Services
{
    public class ServiceComplaintFile : Service<ComplaintFile>, IServiceComplaintFiles
    {
        public ServiceComplaintFile(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public byte[] DownloadFile(string path)
        {
            // Récupérer l'objet fichier depuis la DB
            var file = GetByPath(path);
            if (file == null)
                throw new FileNotFoundException("File not found in database.", path);

            // Construire le chemin complet sur le disque
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var fileName = Path.GetFileName(file.FilePath); // sécurise contre les chemins relatifs malicieux
            var fullPath = Path.Combine(uploadsFolder, fileName);

            // Vérifier que le fichier existe sur le disque
            if (!System.IO.File.Exists(fullPath))
                throw new FileNotFoundException("File not found on disk.", fullPath);

            // Retourner le contenu du fichier
            return System.IO.File.ReadAllBytes(fullPath);

        }

        public ComplaintFile? GetByPath(string path)
        {
            return Get(cf => cf.FilePath == path);
        }

        public bool RemoveFile(string filePath, int complaintId)
        {
            var fileEntity = Get(f => f.ComplaintFK == complaintId && f.FilePath == filePath);

            if (fileEntity == null)
                return false;

            // Supprimer le fichier du disque
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // Supprimer de la DB
            Delete(fileEntity);
            Commit();

            return true;
        }
    }
}
