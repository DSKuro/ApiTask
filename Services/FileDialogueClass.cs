using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTask.Services
{
    public static class FileDialogueClass
    {
        public static FilePickerFileType ExcelAll { get; } = new("Excel Files")
        {
            Patterns = new[] { "*.xls", "*.xlsx", "*.xlsm" },
            AppleUniformTypeIdentifiers = new[] { "*.xls", "*.xlsx", "*.xlsm" },
            MimeTypes = null
        };

        public static async Task<IEnumerable<string?>> OpenFileDialogueAsync(this object? context,
            string? title, bool selectMany = false)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            TopLevel? topLevel = DialogueManager.GetTopLevelForContext(context);

            if (topLevel == null)
            {
                throw new ArgumentNullException(nameof(context));
            }


            return await OpenFileDialogueImpl(topLevel, title, selectMany);
        }

        public static async Task<IEnumerable<string?>> OpenFileDialogueImpl(TopLevel topLevel, string? title, bool selectMany)
        {
            IReadOnlyList<IStorageFile> storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(
                    new FilePickerOpenOptions()
                    {
                        AllowMultiple = selectMany,
                        Title = title ?? "Выберите файл(ы)",
                        FileTypeFilter = new[] { ExcelAll }
                    }
                );
            return storageFiles.Select(s => s.Name);
        }
    }
}
