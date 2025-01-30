using Tradof.Project.Services.DTOs;

namespace Tradof.Project.Services.Extensions
{
    public static class FileExtensions
    {
        public static FileDto ToDto(this Data.Entities.File file)
        {
            return new FileDto
            {
                FileName = file.FileName,
                FilePath = file.FilePath,
                FileSize = file.FileSize,
                FileType = file.FileType,
                ProjectId = file.ProjectId
            };
        }
    }
}
