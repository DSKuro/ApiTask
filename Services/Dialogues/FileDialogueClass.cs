using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiTask.Services.Dialogues
{
    public static class FileDialogueHelper
    {
        public static FilePickerFileType ExcelAll { get; } = new("Excel Files")
        {
            Patterns = new[] { "*.xls", "*.xlsx", "*.xlsm" },
            AppleUniformTypeIdentifiers = new[] { "*.xls", "*.xlsx", "*.xlsm" },
            MimeTypes = null
        };

        public static async Task<IEnumerable<IStorageFile?>> OpenFileDialogueAsync(this object? context,
            string? title, bool selectMany = false)
        {
            TopLevel topLevel = context.GetTopLevelForAnyDialogue();
            return await OpenFileDialogueImpl(topLevel, title, selectMany);
        }

        public static async Task<IEnumerable<IStorageFile?>> OpenFileDialogueImpl(TopLevel topLevel, string? title, bool selectMany)
        {
            IReadOnlyList<IStorageFile> storageFiles = await topLevel.StorageProvider.OpenFilePickerAsync(
                    new FilePickerOpenOptions()
                    {
                        AllowMultiple = selectMany,
                        Title = title ?? "Выберите файл(ы)",
                        FileTypeFilter = new[] { ExcelAll }
                    }
                );
            return storageFiles;
        }
    }
}
